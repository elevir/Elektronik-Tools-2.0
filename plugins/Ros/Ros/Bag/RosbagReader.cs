﻿using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Offline;
using Elektronik.PluginsSystem;
using Elektronik.RosPlugin.Common;
using Elektronik.RosPlugin.Common.RosMessages;
using Elektronik.RosPlugin.Ros.Bag.Parsers;
using Elektronik.Settings;
using UnityEngine;

namespace Elektronik.RosPlugin.Ros.Bag
{
    public class RosbagReader : DataSourceBase<FileScaleSettingsBag>, IDataSourceOffline
    {
        #region IDataSourceOffline

        public RosbagReader()
        {
            _container = new RosbagContainerTree("TMP");
            Data = _container;
            Finished += () => _playing = false;
        }

        public override string DisplayName => "ROS bag";
        public override string Description => "This plugins allows Elektronik to read data saved from ROS.";

        public int AmountOfFrames => _frames?.CurrentSize ?? 0;

        public int CurrentTimestamp
        {
            get
            {
                // ReSharper disable once ConstantConditionalAccessQualifier
                var t = _frames?.Current?.Timestamp ?? 0;
                var s = _startTimestamp;
                var ts = (int) t; // secs
                var tn = (int) (t >> 32); // nanosecs
                var ss = (int) s; // secs
                var sn = (int) (s >> 32); // nanosecs
                var rs = ts - ss;
                var rn = tn - sn;
                return rs * 1000000 + rn / 1000;
            }
        }

        public int CurrentPosition
        {
            get => _frames?.CurrentIndex ?? 0;
            set
            {
                if (value < 0 || value >= AmountOfFrames || CurrentPosition == value) return;
                _rewindAt = value;
            }
        }

        public event Action<bool>? Rewind;
        public event Action? Finished;

        public override void Start()
        {
            _container.Init(TypedSettings);
            _startTimestamp = _container.Parser?.ReadMessages().FirstOrDefault()?.Timestamp ?? 0;
            _frames = new FramesCollection<Frame>(ReadNext);
            Converter = new RosConverter();
            Converter.SetInitTRS(Vector3.zero, Quaternion.identity, Vector3.one * TypedSettings.Scale);
            RosMessageConvertExtender.Converter = Converter;
            _threadWorker = new ThreadWorker();
        }

        public override void Stop()
        {
            _container.Reset();
            _threadWorker?.Dispose();
        }

        public override void Update(float delta)
        {
            if (_playing)
            {
                if (_threadWorker != null && _threadWorker.QueuedActions != 0) return;
                NextKeyFrame();
            }
            else if (_rewindAt > 0)
            {
                _playing = false;
                Rewind?.Invoke(true);
                if (CurrentPosition - (_threadWorker?.QueuedActions ?? 0) > _rewindAt)
                {
                    PreviousKeyFrame();
                }
                else if (CurrentPosition + (_threadWorker?.QueuedActions ?? 0) < _rewindAt)
                {
                    NextKeyFrame();
                }
                else
                {
                    _rewindAt = -1;
                    Rewind?.Invoke(false);
                }
            }
        }

        public void Play()
        {
            _playing = true;
        }

        public void Pause()
        {
            _playing = false;
        }

        public void StopPlaying()
        {
            _playing = false;
            _threadWorker?.Enqueue(() =>
            {
                Data.Clear();
                // ReSharper disable once ConstantConditionalAccessQualifier
                PresentersChain?.Clear();
                _frames?.SoftReset();
            });
        }

        public void PreviousKeyFrame()
        {
            _threadWorker?.Enqueue(() =>
            {
                // ReSharper disable once ConstantConditionalAccessQualifier
                _frames?.Current?.Rewind();
                if (!_frames?.MovePrevious() ?? false) _frames?.SoftReset();
            });
        }

        public void NextKeyFrame()
        {
            _threadWorker?.Enqueue(() =>
            {
                if (_frames?.MoveNext() ?? false)
                {
                    // ReSharper disable once ConstantConditionalAccessQualifier
                    _frames.Current?.Show();
                }
                else
                {
                    MainThreadInvoker.Instance.Enqueue(Finished!);
                }
            });
        }

        #endregion

        #region Private

        private readonly RosbagContainerTree _container;
        private FramesCollection<Frame>? _frames;
        private ThreadWorker? _threadWorker;
        private bool _playing;
        private long _startTimestamp = 0;
        private int _rewindAt = -1;

        private IEnumerator<Frame> ReadNext()
        {
            return _container.Parser!
                    .ReadMessages()
                    .Where(m => m.TopicName is not null && m.TopicType is not null)
                    .Where(m => _container.RealChildren.Keys.Contains(m.TopicName))
                    .Select(m => (MessageParser.Parse(m.Data, m.TopicType!, false),
                                  _container.RealChildren[m.TopicName!], m.Timestamp))
                    .Where(data => data.Item1 is not null)
                    .Select(data => new Frame(data.Timestamp, data.Item1!.ToCommand(data.Item2)!))
                    .GetEnumerator();
        }

        #endregion
    }
}