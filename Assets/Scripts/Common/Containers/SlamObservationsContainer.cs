﻿using Elektronik.Common.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public class SlamObservationsContainer : ICloudObjectsContainer<SlamObservation>
    {
        private readonly List<SlamObservation> m_nodes;
        private readonly Dictionary<int, GameObject> m_gameObjects;

        private struct Connection
        {
            public readonly SlamObservation first;
            public readonly SlamObservation second;
            public readonly int lineId;
            public readonly int obsId1;
            public readonly int obsId2;

            public Connection(int obsId1, int obsId2, SlamObservation first = null, SlamObservation second = null, int lineId = -1)
            {
                this.first = first;
                this.second = second;
                this.lineId = lineId;
                this.obsId1 = obsId1;
                this.obsId2 = obsId2;
            }
        }
        private readonly List<Connection> m_connections;

        private readonly ObjectPool m_observationsPool;
        private readonly ICloudObjectsContainer<SlamLine> m_lines;

        /// <summary>
        /// Get clone of node or Set obj with same id as argument id
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Clone of SlamObservation from graph</returns>
        public SlamObservation this[SlamObservation obj]
        {
            get
            {
                if (obj == null)
                    Debug.LogWarning("[SlamObservationsContainer.Get] Null observation");
                return this[obj.Point.id];
            }
            set
            {
                if (obj == null)
                    Debug.LogWarning("[SlamObservationsContainer.Get] Null observation");
                this[obj.Point.id] = value;
            }
        }

        /// <summary>
        /// Get clone of node or Set obj with same id as argument id
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Clone of SlamObservation from graph</returns>
        public SlamObservation this[int id]
        {
            get
            {
                Debug.AssertFormat(
                Exists(id),
                "[SlamObservationsContainer.Get] Graph doesn't contain observation with id {0}", id);
                return new SlamObservation(m_nodes.Find(obs => obs.Point.id == id));
            }
            set
            {
                if (!TryGet(id, out _)) Add(value); else Update(value);
            }
        }

        private void DisconnectFromAll(SlamObservation observation)
        {
            foreach (var connection in m_connections)
            {
                if ((connection.first == observation || connection.second == observation) && connection.lineId != -1)
                {
                    m_lines.Remove(connection.lineId);
                }
            }
            m_connections.RemoveAll(c => c.first == observation || c.second == observation);
        }

        private void MakeConnectionsAndConnectionPlaceholdersFor(SlamObservation observation)
        {
            foreach (var covisible in observation.CovisibleInfos)
            {
                int connectionIdx = m_connections.FindIndex(c =>
                    c.obsId1 == observation.Point.id && c.obsId2 == covisible.id ||
                    c.obsId2 == observation.Point.id && c.obsId1 == covisible.id);
                if (connectionIdx == -1)
                {
                    connectionIdx = m_connections.Count;
                    m_connections.Add(new Connection(observation.Point.id, covisible.id, observation));
                }

                var connection = m_connections[connectionIdx];
                SlamObservation covisibleObs = m_nodes.FirstOrDefault(node => node.Point.id == covisible.id);
                if (
                    /*already in map*/ covisibleObs != null &&
                    /*is placeholder*/ covisibleObs.Point.id == connection.obsId2 && connection.lineId == -1)
                {
                    SlamLine line = new SlamLine()
                    {
                        vert1 = observation.Point.position,
                        vert2 = covisibleObs.Point.position,
                        pointId1 = observation.Point.id,
                        pointId2 = covisibleObs.Point.id,
                        color1 = observation.Point.color,
                        color2 = covisibleObs.Point.color,
                        isRemoved = false
                    };
                    m_connections[connectionIdx] = new Connection(
                        connection.obsId1, connection.obsId2,
                        observation, covisibleObs,
                        m_lines.Add(line));
                }
                Debug.AssertFormat(m_connections[connectionIdx].first != null || m_connections[connectionIdx].second != null,
                    "[SlamObservationsContainer.UpdateConnectionsOf] connection.first == connection.second == null for id1 = {0}, id2 = {1}",
                    m_connections[connectionIdx].obsId1, m_connections[connectionIdx].obsId2);
            }
        }

        private void ReplaceConnectionPlaceholdersFor(SlamObservation observation)
        {
            for (int i = 0; i < m_connections.Count; ++i)
            {
                if (m_connections[i].obsId2 == observation.Point.id && m_connections[i].second == null)
                {
                    SlamLine line = new SlamLine()
                    {
                        vert1 = m_connections[i].first.Point.position,
                        vert2 = observation.Point.position,
                        pointId1 = m_connections[i].first.Point.id,
                        pointId2 = observation.Point.id,
                        color1 = m_connections[i].first.Point.color,
                        color2 = observation.Point.color,
                        isRemoved = false
                    };
                    m_connections[i] = new Connection(
                            m_connections[i].obsId1, m_connections[i].obsId2,
                            m_connections[i].first, observation,
                            m_lines.Add(line));
                }
            }
        }

        private void UpdateConnectionVerticesFor(SlamObservation observation)
        {
            var allConnectionsOfArg = m_connections.Where(con => (con.first == observation || con.second == observation) && con.lineId != -1);
            foreach (var connection in allConnectionsOfArg)
            {
                SlamLine line = m_lines[connection.lineId];
                if (connection.first == observation)
                {
                    line.vert1 = connection.first.Point.position;
                    line.vert2 = connection.second.Point.position;
                }
                else
                {
                    line.vert2 = connection.second.Point.position;
                    line.vert1 = connection.first.Point.position;
                }
                m_lines.Update(line);
            }
        }

        private void UpdateConnectionsFor(SlamObservation observation)
        {
            MakeConnectionsAndConnectionPlaceholdersFor(observation);
            ReplaceConnectionPlaceholdersFor(observation);
            UpdateConnectionVerticesFor(observation);
        }

        private void UpdateGameobjectFor(SlamObservation observation)
        {
            var objectTransform = m_gameObjects[observation.Point.id].transform;
            objectTransform.position = observation.Point.position;
            objectTransform.rotation = observation.Orientation;
        }

        /// <param name="prefab">Desired prefab of observation</param>
        /// <param name="lines">Lines cloud objects for connections drawing</param>
        public SlamObservationsContainer(GameObject prefab, ICloudObjectsContainer<SlamLine> lines)
        {
            m_nodes = new List<SlamObservation>();
            m_gameObjects = new Dictionary<int, GameObject>();
            m_observationsPool = new ObjectPool(prefab);
            m_lines = lines;
            m_connections = new List<Connection>();
        }

        /// <summary>
        /// Add ref of observation into the graph. Make sure that you don't use it anywhere!
        /// Pass the clone of observation if you are not sure.
        /// </summary>
        /// <param name="observation"></param>
        /// <returns>Id of observation</returns>
        public int Add(SlamObservation observation)
        {
            if (observation == null)
                Debug.LogWarning("[SlamObservationsContainer.Add] Null observation");
            Debug.AssertFormat(
                !Exists(observation),
                "[SlamObservationsContainer.Add] Graph already contains observation with id {0}", observation.Point.id);
            int last = m_nodes.Count;
            m_nodes.Add(new SlamObservation(observation));
            m_gameObjects[m_nodes[last].Point.id] = m_observationsPool.Spawn(observation.Point.position, observation.Orientation);
            UpdateConnectionsFor(m_nodes[last]);
            Debug.LogFormat(
                "[SlamObservationsContainer.Add] Added observation with id {0}; count of covisible nodes {1}",
                m_nodes[last].Point.id, m_nodes[last].CovisibleInfos.Count);
            return observation.Point.id;
        }

        /// <summary>
        /// Look at the summary of Add
        /// </summary>
        /// <param name="objects"></param>
        public void AddRange(SlamObservation[] objects)
        {
            foreach (var observation in objects)
            {
                Add(observation);
            }
        }

        /// <summary>
        /// Just change color field of SlamObservation point now
        /// </summary>
        /// <param name="obj"></param>
        public void ChangeColor(SlamObservation obj)
        {
            if (obj == null)
                Debug.LogWarning("[SlamObservationsContainer.ChangeColor] Null observation");
            Debug.AssertFormat(
                Exists(obj),
                "[SlamObservationsContainer.ChangeColor] Graph doesn't contain observation with id {0}", obj.Point.id);
            SlamObservation node = m_nodes.First(n => obj.Point.id == n.Point.id);
            SlamPoint point = node.Point;
            point.color = obj.Point.color;
            node.Point = point;
        }

        /// <summary>
        /// Clear graph
        /// </summary>
        public void Clear()
        {
            m_observationsPool.DespawnAllActiveObjects();
            m_lines.Clear();
            m_connections.Clear();
            m_nodes.Clear();
        }

        /// <summary>
        /// Check node existence by SlamObservation object
        /// </summary>
        /// <param name="objId"></param>
        /// <returns>true if exists, otherwise false</returns>
        public bool Exists(SlamObservation obj)
        {
            if (obj == null)
                Debug.LogWarning("[SlamObservationsContainer.Exists] Null observation");
            return Exists(obj.Point.id);
        }

        /// <summary>
        /// Check existing of node by id
        /// </summary>
        /// <param name="objId"></param>
        /// <returns>true if exists, otherwise false</returns>
        public bool Exists(int objId) => m_nodes.Any(obs => obs.Point.id == objId);

        /// <summary>
        /// Get clones of observations from graph
        /// </summary>
        /// <returns></returns>
        public SlamObservation[] GetAll() => m_nodes.Select(node => new SlamObservation(node)).ToArray();

        /// <summary>
        /// Remove by id
        /// </summary>
        /// <param name="id"></param>
        public void Remove(int id)
        {
            Debug.AssertFormat(
                Exists(id),
                "[SlamObservationsContainer.Remove] Graph doesn't contain observation with id {0}", id);

            var node = m_nodes.First(obs => obs.Point.id == id);
            Debug.LogFormat(
                "[SlamObservationsContainer.Remove] Removing observation with id {0}; count of covisible nodes {1}",
                id, node.CovisibleInfos.Count);
            DisconnectFromAll(node);
            m_observationsPool.Despawn(m_gameObjects[id]);
            m_gameObjects.Remove(id);
            m_nodes.Remove(node);
        }

        /// <summary>
        /// Remove by SlamObservation object
        /// </summary>
        /// <param name="obj"></param>
        public void Remove(SlamObservation obj)
        {
            if (obj == null)
                Debug.LogWarning("[SlamObservationsContainer.Remove] Null observation");
            Remove(obj.Point.id);
        }

        /// <summary>
        /// Repaint connections
        /// </summary>
        public void Repaint() => m_lines.Repaint();

        /// <summary>
        /// Try get observation from graph
        /// </summary>
        /// <param name="obj">Slam observation you want to find</param>
        /// <param name="current">Clone of observation in graph or null if doesn't exist</param>
        /// <returns>Returns false if observation with given id from obj exists, otherwise true.</returns>
        public bool TryGet(SlamObservation obj, out SlamObservation current)
        {
            if (obj == null)
                Debug.LogWarning("[SlamObservationsContainer.TryGet] Null observation");
            return TryGet(obj.Point.id, out current);
        }

        public bool TryGet(int idx, out SlamObservation current)
        {
            current = new SlamObservation(m_nodes.FirstOrDefault(o => o.Point.id == idx));
            return current == null;
        }

        /// <summary>
        /// Copy data from obj.
        /// </summary>
        /// <param name="obj"></param>
        public void Update(SlamObservation obj)
        {
            if (obj == null)
                Debug.LogWarning("[SlamObservationsContainer.Update] Null observation");
            Debug.AssertFormat(
                Exists(obj.Point.id),
                "[SlamObservationsContainer.Update] Graph doesn't contain observation with id {0}", obj.Point.id);
            SlamObservation node = m_nodes.First(obs => obs.Point.id == obj.Point.id);

            node.Statistics = obj.Statistics;
            node.Point = obj.Point;
            node.Orientation = obj.Orientation;
            UpdateGameobjectFor(node);
            UpdateConnectionsFor(node);

            Debug.LogFormat(
                "[SlamObservationsContainer.Update] Updated observation with id {0}; count of covisible nodes {1}",
                node.Point.id, node.CovisibleInfos.Count);
        }

        public IEnumerator<SlamObservation> GetEnumerator() => m_nodes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => m_nodes.GetEnumerator();
    }
}
