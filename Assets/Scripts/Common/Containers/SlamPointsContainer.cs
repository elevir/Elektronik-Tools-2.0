﻿using Elektronik.Common.Clouds;
using Elektronik.Common.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public class SlamPointsContainer : ICloudObjectsContainer<SlamPoint>
    {
        private readonly SortedDictionary<int, SlamPoint> m_points;
        private readonly IFastPointsCloud m_pointsCloud;

        private int m_added = 0;
        private int m_removed = 0;
        private int m_diff = 0;

        public SlamPointsContainer(IFastPointsCloud cloud)
        {
            m_points = new SortedDictionary<int, SlamPoint>();
            m_pointsCloud = cloud;
        }

        public int Add(SlamPoint point)
        {
            //Debug.Assert(
            //    !m_points.ContainsKey(point.id),
            //    $"[SlamPointsContainer.Add] Point with id {point.id} already in dictionary!");
            if (m_points.ContainsKey(point.id))
                throw new InvalidSlamContainerOperationException($"[SlamPointsContainer.Add] Point with id {point.id} already in dictionary!");
            ++m_diff;
            ++m_added;
            m_pointsCloud.Set(point.id, Matrix4x4.Translate(point.position), point.color);
            m_points.Add(point.id, point);
            return point.id;
        }

        public void AddRange(SlamPoint[] points)
        {
            foreach (var point in points)
            {
                Add(point);
            }
        }

        public void Update(SlamPoint point)
        {
            //Debug.Assert(
            //    m_points.ContainsKey(point.id),
            //    $"[SlamPointsContainer.Update] Container doesn't contain point with id {point.id}");
            if (!m_points.ContainsKey(point.id))
                throw new InvalidSlamContainerOperationException($"[SlamPointsContainer.Update] Container doesn't contain point with id {point.id}");
            Matrix4x4 to = Matrix4x4.Translate(point.position);
            SlamPoint currentPoint = m_points[point.id];
            currentPoint.position = point.position;
            currentPoint.color = point.color;
            m_points[point.id] = currentPoint;
            m_pointsCloud.Set(point.id, to, point.color);
        }

        public void ChangeColor(SlamPoint point)
        {
            //Debug.Assert(
            //    m_points.ContainsKey(point.id),
            //    $"[SlamPointsContainer.ChangeColor] Container doesn't contain point with id {point.id}");
            if (!m_points.ContainsKey(point.id))
                throw new InvalidSlamContainerOperationException($"[SlamPointsContainer.ChangeColor] Container doesn't contain point with id {point.id}");
            m_pointsCloud.Set(point.id, point.color);
            SlamPoint currentPoint = m_points[point.id];
            currentPoint.color = point.color;
            m_points[point.id] = currentPoint;
        }

        public void Remove(int pointId)
        {
            --m_diff;
            ++m_removed;
            //Debug.Assert(
            //    m_points.ContainsKey(pointId),
            //    $"[SlamPointsContainer.Remove] Container doesn't contain point with id {pointId}");
            if (!m_points.ContainsKey(pointId))
                throw new InvalidSlamContainerOperationException($"[SlamPointsContainer.Remove] Container doesn't contain point with id {pointId}");
            m_pointsCloud.Set(pointId, Matrix4x4.identity, new Color(0, 0, 0, 0));
            m_points.Remove(pointId);
        }

        public void Remove(SlamPoint point) => Remove(point.id);

        public void Clear()
        {
            int[] pointsIds = m_points.Keys.ToArray();
            for (int i = 0; i < pointsIds.Length; ++i)
                Remove(pointsIds[i]);
            m_points.Clear();
            m_pointsCloud.Clear();
            Repaint();
            Debug.Log($"[SlamPointsContainer.Clear] Added points: {m_added}; Removed points: {m_removed}; Diff: {m_diff}");
            m_added = 0;
            m_removed = 0;
        }

        public SlamPoint[] GetAll() => m_points.Select(kv => kv.Value).ToArray();

        public SlamPoint this[SlamPoint obj]
        {
            get => this[obj.id];
            set => this[obj.id] = value;
        }

        public SlamPoint this[int id]
        {
            get
            {
                //Debug.Assert(
                //    m_points.ContainsKey(id),
                //    $"[SlamPointsContainer.Get] Container doesn't contain point with id {id}");
                if (!m_points.ContainsKey(id))
                    throw new InvalidSlamContainerOperationException($"[SlamPointsContainer.Get] Container doesn't contain point with id {id}");
                return m_points[id];
            }
            set
            {
                if (!TryGet(id, out _)) Add(value); else Update(value);
            }
        }

        public bool Exists(int pointId) => m_points.ContainsKey(pointId);

        public bool Exists(SlamPoint point) => Exists(point.id);

        public bool TryGet(int idx, out SlamPoint current)
        {
            current = new SlamPoint();
            if (m_pointsCloud.Exists(idx))
            {
                current = this[idx];
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryGet(SlamPoint point, out SlamPoint current) => TryGet(point.id, out current);

        public void Repaint() => m_pointsCloud.Repaint();

        public IEnumerator<SlamPoint> GetEnumerator() => m_points.Select(kv => kv.Value).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => m_points.Select(kv => kv.Value).GetEnumerator();
    }
}
