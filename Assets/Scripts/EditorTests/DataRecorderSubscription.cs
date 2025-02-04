using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;
using Elektronik.PluginsSystem;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace Elektronik.EditorTests
{
    public class DataRecorderSubscription
    {
        [Test]
        public void SubscribePointCloudContainer()
        {
            var points = new []
            {
                new SlamPoint(0, Vector3.back, Color.black),
                new SlamPoint(1, Vector3.down, Color.blue),
            };
            var container = new CloudContainer<SlamPoint>();
            var mockedRecorder = new Mock<IDataRecorderPlugin>();
            mockedRecorder.Object.SubscribeOn(container, "");
            
            Assert.AreEqual(3, DataRecorderSubscriber.Subscriptions[mockedRecorder.Object].SelectMany(s => s.Value).Count());

            container.AddRange(points);
            container.Update(points);
            container.Remove(points);
            mockedRecorder.Verify(r => r.OnAdded(It.IsAny<string>(), points), Times.Once());
            mockedRecorder.Verify(r => r.OnUpdated(It.IsAny<string>(), points), Times.Once());
            mockedRecorder.Verify(r => r.OnRemoved<SlamPoint>(It.IsAny<string>(), It.IsAny<List<int>>()),
                                   Times.Once());
            
            mockedRecorder.Object.UnsubscribeFromEverything();

            container.AddRange(points);
            container.Update(points);
            container.Remove(points);
            mockedRecorder.Verify(r => r.OnAdded(It.IsAny<string>(), points), Times.Once());
            mockedRecorder.Verify(r => r.OnUpdated(It.IsAny<string>(), points), Times.Once());
            mockedRecorder.Verify(r => r.OnRemoved<SlamPoint>(It.IsAny<string>(), It.IsAny<List<int>>()),
                                   Times.Once());
        }

        [Test]
        public void SubscribeTrackedCloudContainer()
        {
            var tracked = new []
            {
                new SlamTrackedObject(0, Vector3.back, Quaternion.identity),
                new SlamTrackedObject(1, Vector3.down, Quaternion.identity),
            };
            var container = new TrackedObjectsContainer();
            var mockedRecorder = new Mock<IDataRecorderPlugin>();
            mockedRecorder.Object.SubscribeOn(container, "");

            container.AddRange(tracked);
            container.Update(tracked);
            container.Remove(tracked);
            mockedRecorder.Verify(r => r.OnAdded(It.IsAny<string>(), tracked), Times.Once());
            mockedRecorder.Verify(r => r.OnUpdated(It.IsAny<string>(), tracked), Times.Once());
            mockedRecorder.Verify(r => r.OnRemoved<SlamTrackedObject>(It.IsAny<string>(), It.IsAny<List<int>>()),
                                   Times.Once());
            
            mockedRecorder.Object.UnsubscribeFromEverything();

            container.AddRange(tracked);
            container.Update(tracked);
            container.Remove(tracked);
            mockedRecorder.Verify(r => r.OnAdded(It.IsAny<string>(), tracked), Times.Once());
            mockedRecorder.Verify(r => r.OnUpdated(It.IsAny<string>(), tracked), Times.Once());
            mockedRecorder.Verify(r => r.OnRemoved<SlamTrackedObject>(It.IsAny<string>(), It.IsAny<List<int>>()),
                                   Times.Once());
        }

        [Test]
        public void SubscribeConnectablePointCloudContainer()
        {
            var container = new ConnectableObjectsContainer<SlamPoint>(new CloudContainer<SlamPoint>(),
                                                                       new SlamLinesContainer());
            var mockedRecorder = new Mock<IDataRecorderPlugin>();
            mockedRecorder.Object.SubscribeOn(container, "");

            var points = new []
            {
                new SlamPoint(0, Vector3.back, Color.black),
                new SlamPoint(1, Vector3.down, Color.blue),
            };
            var connections = new List<(int, int)> {(0, 1)};
            container.AddRange(points);
            container.Update(points);
            container.AddConnections(connections);
            container.RemoveConnections(connections);
            container.Remove(points);
            mockedRecorder.Verify(r => r.OnConnectionsUpdated<SlamPoint>(It.IsAny<string>(), connections), Times.Once());
            mockedRecorder.Verify(r => r.OnConnectionsRemoved<SlamPoint>(It.IsAny<string>(), connections), Times.Once());
            mockedRecorder.Verify(r => r.OnAdded(It.IsAny<string>(), points), Times.Once());
            mockedRecorder.Verify(r => r.OnUpdated(It.IsAny<string>(), points), Times.Once());
            mockedRecorder.Verify(r => r.OnRemoved<SlamPoint>(It.IsAny<string>(), It.IsAny<List<int>>()),
                                   Times.Once());
            
            mockedRecorder.Object.UnsubscribeFromEverything();
            
            container.AddRange(points);
            container.Update(points);
            container.AddConnections(connections);
            container.RemoveConnections(connections);
            container.Remove(points);
            mockedRecorder.Verify(r => r.OnConnectionsUpdated<SlamPoint>(It.IsAny<string>(), connections), Times.Once());
            mockedRecorder.Verify(r => r.OnConnectionsRemoved<SlamPoint>(It.IsAny<string>(), connections), Times.Once());
            mockedRecorder.Verify(r => r.OnAdded(It.IsAny<string>(), points), Times.Once());
            mockedRecorder.Verify(r => r.OnUpdated(It.IsAny<string>(), points), Times.Once());
            mockedRecorder.Verify(r => r.OnRemoved<SlamPoint>(It.IsAny<string>(), It.IsAny<List<int>>()),
                                   Times.Once());
        }

        [Test]
        public void SubscribeConnectableObservationCloudContainer()
        {
            var container = new ConnectableObjectsContainer<SlamObservation>(new CloudContainer<SlamObservation>(),
                                                                       new SlamLinesContainer());
            var mockedRecorder = new Mock<IDataRecorderPlugin>();
            mockedRecorder.Object.SubscribeOn(container, "");

            var points = new []
            {
                new SlamObservation(new SlamPoint(0, Vector3.back, Color.black), Quaternion.identity, "", ""),
                new SlamObservation(new SlamPoint(1, Vector3.down, Color.blue), Quaternion.identity, "", ""),
            };
            var connections = new List<(int, int)> {(0, 1)};
            container.AddRange(points);
            container.Update(points);
            container.AddConnections(connections);
            container.RemoveConnections(connections);
            container.Remove(points);
            mockedRecorder.Verify(r => r.OnConnectionsUpdated<SlamObservation>(It.IsAny<string>(), connections), Times.Once());
            mockedRecorder.Verify(r => r.OnConnectionsRemoved<SlamObservation>(It.IsAny<string>(), connections), Times.Once());
            mockedRecorder.Verify(r => r.OnAdded(It.IsAny<string>(), points), Times.Once());
            mockedRecorder.Verify(r => r.OnUpdated(It.IsAny<string>(), points), Times.Once());
            mockedRecorder.Verify(r => r.OnRemoved<SlamObservation>(It.IsAny<string>(), It.IsAny<List<int>>()),
                                   Times.Once());
            
            mockedRecorder.Object.UnsubscribeFromEverything();
            
            container.AddRange(points);
            container.Update(points);
            container.AddConnections(connections);
            container.RemoveConnections(connections);
            container.Remove(points);
            mockedRecorder.Verify(r => r.OnConnectionsUpdated<SlamObservation>(It.IsAny<string>(), connections), Times.Once());
            mockedRecorder.Verify(r => r.OnConnectionsRemoved<SlamObservation>(It.IsAny<string>(), connections), Times.Once());
            mockedRecorder.Verify(r => r.OnAdded(It.IsAny<string>(), points), Times.Once());
            mockedRecorder.Verify(r => r.OnUpdated(It.IsAny<string>(), points), Times.Once());
            mockedRecorder.Verify(r => r.OnRemoved<SlamObservation>(It.IsAny<string>(),  It.IsAny<List<int>>()),
                                   Times.Once());
        }
    }
}