﻿using Elektronik.Common.Clouds;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Pb;
using Elektronik.Common.Extensions;
using UnityEngine;

namespace Elektronik.Common.Maps
{
    public class SlamMap : MonoBehaviour
    {
        public Helmet helmetPrefab;

        public GameObject observationPrefab;
        public FastPointCloud fastPointCloud;
        public FastLinesCloud pointsConnectionsCloud;
        public FastLinesCloud observationsConnectionsCloud;
        public FastLinesCloud trackedObjsConnectionsCloud;
        public FastLinesCloud linesCloud;

        public IConnectableObjectsContainer<SlamPoint> Points { get; private set; }
        public GameObjectsContainer<SlamObservation> ObservationsGO { get; private set; }
        public GameObjectsContainer<TrackedObjPb> TrackedObjsGO { get; private set; }

        public IConnectableObjectsContainer<SlamObservation> Observations { get; private set; }
        public IConnectableObjectsContainer<TrackedObjPb> TrackedObjs { get; private set; }

        public ILinesContainer<SlamLine> Lines { get; private set; }

        private void Awake()
        {
            var trackedObjs = new TrackedObjectsContainer(helmetPrefab);
            TrackedObjs = new ConnectableObjectsContainer<TrackedObjPb>(
                trackedObjs,
                new SlamLinesContainer(trackedObjsConnectionsCloud));
            TrackedObjsGO = trackedObjs;

            var observations = new SlamObservationsContainer(observationPrefab);
            Observations = new ConnectableObjectsContainer<SlamObservation>(
                observations,
                new SlamLinesContainer(observationsConnectionsCloud));
            ObservationsGO = observations;

            Points = new ConnectableObjectsContainer<SlamPoint>(
                new SlamPointsContainer(fastPointCloud),
                new SlamLinesContainer(pointsConnectionsCloud));

            Lines = new SlamLinesContainer(linesCloud);
        }

        public void SetActivePointCloud(bool value)
        {
            fastPointCloud.SetActive(value);
            pointsConnectionsCloud.SetActive(value);
        }
        public void SetActiveObservationsGraph(bool value)
        {
            ObservationsGO.ObservationsPool.SetActive(value);
            observationsConnectionsCloud.SetActive(value);
        }

        public void Clear()
        {
            TrackedObjs.Clear();
            Observations.Clear();
            Points.Clear();
        }
    }
}
