﻿using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using Elektronik.Threading;
using UnityEngine;

namespace Elektronik.Mesh
{
    public class MeshReconstructor : IMeshContainer
    {
        public MeshReconstructor(IContainer<SlamPoint> points, IContainer<SlamObservation> observations,
            string displayName = "Mesh")
        {
            _points = points;
            _observations = observations;
            DisplayName = displayName;

            _points.OnAdded += (_, __) => RequestCalculation();
            _points.OnUpdated += (_, __) => RequestCalculation();
            _points.OnRemoved += (_, __) => RequestCalculation();
            _observations.OnAdded += (_, __) => RequestCalculation();
            _observations.OnUpdated += (_, __) => RequestCalculation();
            _observations.OnRemoved += (_, __) => RequestCalculation();
        }

        public event EventHandler<MeshUpdatedEventArgs> OnMeshUpdated;

        #region ISourceTree

        public string DisplayName { get; set; }

        public IEnumerable<ISourceTree> Children { get; } = new ISourceTree[0];

        public void Clear()
        {
            OnMeshUpdated?.Invoke(this, new MeshUpdatedEventArgs(new Vector3[0], new int[0]));
        }

        public void SetRenderer(ISourceRenderer renderer)
        {
            if (renderer is IMeshRenderer meshRenderer)
            {
                OnMeshUpdated += meshRenderer.OnMeshUpdated;
            }
        }

        #endregion

        #region IVisible

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value) return;

                _isVisible = value;
                OnVisibleChanged?.Invoke(_isVisible);

                if (_isVisible) RequestCalculation();
                else Clear();
            }
        }

        public event Action<bool> OnVisibleChanged;

        public bool ShowButton => true;

        #endregion

        #region Private

        private bool _isVisible = false;
        private readonly IContainer<SlamPoint> _points;
        private readonly IContainer<SlamObservation> _observations;
        private readonly ThreadWorkerSingleAwaiter _threadWorker = new ThreadWorkerSingleAwaiter();

        private void RequestCalculation()
        {
            if (!_isVisible) return;

            _threadWorker.Enqueue(CalculateMesh);
        }

        private static NativeVector ToNative(SlamPoint point) =>
            new NativeVector(point.Position.x, point.Position.y, point.Position.z);

        private static NativeTransform ToNative(SlamObservation observation)
        {
            var m = Matrix4x4.Rotate(observation.Rotation);
            return new NativeTransform
            {
                position = ToNative(observation.Point),
                r11 = m.m00,
                r12 = m.m01,
                r13 = m.m02,
                r21 = m.m10,
                r22 = m.m11,
                r23 = m.m12,
                r31 = m.m20,
                r32 = m.m21,
                r33 = m.m22,
            };
        }

        private static Vector3 ToUnity(NativeVector vector) => new Vector3(vector.x, vector.y, vector.z);

        private void CalculateMesh()
        {
            var points = _points.ToArray();
            var observations = _observations.ToArray();
            var obsId2Index = new Dictionary<int, int>();
            for (int i = 0; i < observations.Length; i++)
            {
                obsId2Index[observations[i].Id] = i;
            }

            var connectionsNotSet = true;
            var pointsViewsArr = new Dictionary<int, List<int>>();
            foreach (var observation in observations)
            {
                foreach (var id in observation.ObservedPoints.ToArray())
                {
                    if (!_points.Contains(id)) continue;
                    if (!pointsViewsArr.ContainsKey(id)) pointsViewsArr.Add(id, new List<int>());
                    pointsViewsArr[id].Add(obsId2Index[observation.Point.Id]);
                    connectionsNotSet = false;
                }
            }

            if (connectionsNotSet) return;

            var cPoints = new vectorv(points.Select(ToNative));
            var cViews = new vectori2d(points.Select(p => pointsViewsArr.ContainsKey(p.Id)
                ? new vectori(pointsViewsArr[p.Id].OrderBy(i => i))
                : new vectori()));
            var cObservations = new vectort(observations.Select(ToNative));

            var builder = new MeshBuilder();
            var output = builder.FromPointsAndObservations(cPoints, cViews, cObservations);

            var vertices = output.points.Select(ToUnity).ToArray();

            if (_isVisible)
            {
                OnMeshUpdated?.Invoke(this, new MeshUpdatedEventArgs(vertices, output.triangles.ToArray()));
            }
        }

        #endregion
    }
}