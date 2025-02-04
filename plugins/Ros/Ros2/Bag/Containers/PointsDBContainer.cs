﻿using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Clouds;
using Elektronik.Containers;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data.PackageObjects;
using Elektronik.PluginsSystem;
using Elektronik.RosPlugin.Common.RosMessages;
using Elektronik.RosPlugin.Ros2.Bag.Data;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using SQLite;
using UnityEngine;

namespace Elektronik.RosPlugin.Ros2.Bag.Containers
{
    public class PointsDBContainer : DBContainer<PointCloud2, SlamPoint[]>, ILookable, ISnapshotable
    {
        public PointsDBContainer(string displayName, List<SQLiteConnection> dbModels, Topic topic,
                                 List<long> actualTimestamps)
                : base(displayName, dbModels, topic, actualTimestamps)
        {
        }

        #region DBContainer

        public override bool ShowButton { get; } = true;

        public override void Clear()
        {
            _center = new Vector3(float.NaN, float.NaN, float.NaN);
            _bounds = new Vector3(float.NaN, float.NaN, float.NaN);
            OnClear?.Invoke(this);
        }

        public override void SetRenderer(ISourceRenderer renderer)
        {
            if (renderer is not ICloudRenderer<SlamPoint> pointRenderer) return;
            OnShow += pointRenderer.ShowItems;
            OnClear += pointRenderer.OnClear;
        }

        public override bool IsVisible
        {
            get => base.IsVisible;
            set
            {
                lock (this)
                {
                    base.IsVisible = value;
                    if (!base.IsVisible) Clear();
                    else SetData();
                }
            }
        }

        protected override void SetData()
        {
            base.SetData();
            if (Current is null) return;
            CalculateBounds(Current);
            OnShow?.Invoke(this, Current);
        }

        protected override SlamPoint[] ToRenderType(PointCloud2 message)
        {
            return message.ToSlamPoints();
        }

        #endregion

        #region ILookable

        public (Vector3 pos, Quaternion rot) Look(Transform transform)
        {
            if (float.IsNaN(_bounds.x)) return (transform.position, transform.rotation);

            return (_center + _bounds / 2 + _bounds.normalized, Quaternion.LookRotation(-_bounds));
        }

        #endregion

        #region ISnapshotable

        public ISnapshotable TakeSnapshot()
        {
            var res = new CloudContainer<SlamPoint>(DisplayName);
            res.AddRange(Current);
            return res;
        }

        public void WriteSnapshot(IDataRecorderPlugin recorder)
        {
            recorder.OnAdded(DisplayName, Current);
        }

        #endregion

        #region Private definitinons

        private event Action<object, IEnumerable<SlamPoint>>? OnShow;
        private event Action<object>? OnClear;
        private Vector3 _center = new(float.NaN, float.NaN, float.NaN);
        private Vector3 _bounds = new(float.NaN, float.NaN, float.NaN);

        private void CalculateBounds(SlamPoint[] points)
        {
            Vector3 min = Vector3.positiveInfinity;
            Vector3 max = Vector3.negativeInfinity;
            foreach (var point in points.Select(p => p.Position))
            {
                min = new Vector3(Mathf.Min(min.x, point.x), Mathf.Min(min.y, point.y), Mathf.Min(min.z, point.z));
                max = new Vector3(Mathf.Max(max.x, point.x), Mathf.Max(max.y, point.y), Mathf.Max(max.z, point.z));
            }

            _bounds = max - min;
            _center = (max + min) / 2;
        }

        #endregion
    }
}