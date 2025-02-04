﻿using System;
using System.Linq;
using Elektronik.PluginsSystem;
using Elektronik.RosPlugin.Common;
using Elektronik.RosPlugin.Common.RosMessages;
using Elektronik.RosPlugin.Ros2.Bag.Containers;
using Elektronik.Threading;
using UnityEngine;

namespace Elektronik.RosPlugin.Ros2.Bag
{
    public class Rosbag2Reader : DataSourcePluginBase<Rosbag2Settings>, IDataSourcePluginOffline
    {
        public Rosbag2Reader()
        {
            _data = new Rosbag2ContainerTree("TMP");
            Data = _data;
        }

        #region IDataSourceOffline implementation

        public override string DisplayName => "ROS2 bag";

        public override string Description => "This plugins allows Elektronik to read data saved from " +
                "<#7f7fe5><u><link=\"https://docs.ros.org/en/foxy/index.html\">ROS2</link></u></color> using " +
                "<#7f7fe5><u><link=\"https://docs.ros.org/en/foxy/Tutorials/Ros2bag/Recording-And-Playing-Back-Data.html\">" +
                "rosbag2</link></u></color>.";

        public override void Start()
        {
            _threadWorker = new ThreadQueueWorker();
            _data.Init(TypedSettings);

            _actualTimestamps = _data.Timestamps.Values
                    .SelectMany(l => l)
                    .OrderBy(i => i)
                    .ToArray();

            Converter = new RosConverter();
            Converter.SetInitTRS(Vector3.zero, Quaternion.identity);
            RosMessageConvertExtender.Converter = Converter;
        }

        public override void Stop()
        {
            _data.Reset();
            _threadWorker?.Dispose();
        }

        public override void Update(float delta)
        {
            if (_threadWorker == null) return;
            if (_playing)
            {
                if (CurrentPosition == AmountOfFrames - 1)
                {
                    MainThreadInvoker.Enqueue(() => Finished?.Invoke());
                    _playing = false;
                    return;
                }

                NextKeyFrame();
            }
            else if (_rewindPlannedPos > 0)
            {
                Rewind?.Invoke(true);
                _currentPosition = _rewindPlannedPos;
                _rewindPlannedPos = -1;
                _threadWorker?.Enqueue(() =>
                {
                    _data.ShowAt(_actualTimestamps![_currentPosition], true);
                    Rewind?.Invoke(false);
                });
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
                _currentPosition = 0;
                Data.Clear();
            });
        }

        public void PreviousKeyFrame()
        {
            _threadWorker?.Enqueue(() =>
            {
                if (CurrentPosition == 0) return;
                _currentPosition--;
                _data.ShowAt(_actualTimestamps![CurrentPosition]);
            });
        }

        public void NextKeyFrame()
        {
            if ((_threadWorker?.ActiveActions ?? 1) > 0) return;
            _threadWorker?.Enqueue(() =>
            {
                if (CurrentPosition == AmountOfFrames - 1) return;

                _currentPosition++;
                _data.ShowAt(_actualTimestamps![CurrentPosition]);
            });
        }

        public void SetFileName(string filename)
        {
            TypedSettings.FilePath = filename;
        }

        public int AmountOfFrames => _actualTimestamps?.Length ?? 0;

        public string CurrentTimestamp =>
                $"{(_actualTimestamps?[CurrentPosition] - _actualTimestamps?[0] ?? 0) / 1000000000f:F3}";

        public string[] SupportedExtensions { get; } = {".db3"};

        public int CurrentPosition
        {
            get => _currentPosition;
            set
            {
                if (value < 0 || value >= AmountOfFrames || _currentPosition == value) return;
                _rewindPlannedPos = value;
                _playing = false;
            }
        }

        public int DelayBetweenFrames { get; set; }

        public event Action<bool>? Rewind;

        public event Action? Finished;

        #endregion

        #region Private definitions

        private readonly Rosbag2ContainerTree _data;
        private ThreadQueueWorker? _threadWorker;
        private bool _playing;
        private int _currentPosition;
        private long[]? _actualTimestamps;
        private int _rewindPlannedPos;

        #endregion
    }
}