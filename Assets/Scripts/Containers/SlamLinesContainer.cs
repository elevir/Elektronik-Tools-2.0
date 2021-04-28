﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Clouds;
using Elektronik.Containers.EventArgs;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Containers
{
    public class SlamLinesContainer : IContainer<SlamLine>, ISourceTree, ILookable, IVisible, ISnapshotable
    {
        public SlamLinesContainer(string displayName = "")
        {
            DisplayName = string.IsNullOrEmpty(displayName) ? "Lines" : displayName;
        }

        #region IContaitner implementation

        public event Action<IContainer<SlamLine>, AddedEventArgs<SlamLine>> OnAdded;

        public event Action<IContainer<SlamLine>, UpdatedEventArgs<SlamLine>> OnUpdated;

        public event Action<IContainer<SlamLine>, RemovedEventArgs> OnRemoved;

        public int Count
        {
            get
            {
                lock (_connections)
                {
                    return _connections.Count;
                }
            }
        }

        public bool IsReadOnly => false;


        public IEnumerator<SlamLine> GetEnumerator()
        {
            lock (_connections)
            {
                return _connections.Select(kv => kv.Value).GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Contains(SlamLine item)
        {
            lock (_connections)
            {
                return _connectionsIndices.ContainsKey(item);
            }
        }

        public void CopyTo(SlamLine[] array, int arrayIndex)
        {
            lock (_connections)
            {
                _connections.Values.CopyTo(array, arrayIndex);
            }
        }

        public int IndexOf(SlamLine item)
        {
            lock (_connections)
            {
                foreach (var c in _connections)
                {
                    if (c.Value.Point1.Id == item.Point1.Id && c.Value.Point2.Id == item.Point2.Id ||
                        c.Value.Point2.Id == item.Point1.Id && c.Value.Point1.Id == item.Point2.Id)
                    {
                        return c.Key;
                    }
                }

                return -1;
            }
        }

        public SlamLine this[int index]
        {
            get
            {
                lock (_connections)
                {
                    return _connections[index];
                }
            }
            set => Update(value);
        }

        public void Add(SlamLine obj)
        {
            lock (_connections)
            {
                obj.Id = _freeIds.Count > 0 ? _freeIds.Dequeue() : _maxId++;
                _connectionsIndices[obj] = obj.Id;
                _connections[obj.Id] = obj;
            }
            if (IsVisible)
            {
                OnAdded?.Invoke(this, new AddedEventArgs<SlamLine>(new[] {obj}));
            }
        }

        public void Insert(int index, SlamLine item) => Add(item);

        public void AddRange(IEnumerable<SlamLine> items)
        {
            var buffer = new List<SlamLine>();
            lock (_connections)
            {
                foreach (var obj in items)
                {
                    var line = obj;
                    line.Id = _freeIds.Count > 0 ? _freeIds.Dequeue() : _maxId++;
                    _connections[line.Id] = line;
                    _connectionsIndices[line] = line.Id;
                    buffer.Add(line);
                }
            }

            if (IsVisible)
            {
                OnAdded?.Invoke(this, new AddedEventArgs<SlamLine>(buffer));
            }
        }

        public void Update(SlamLine item)
        {
            lock (_connections)
            {
                int index = _connectionsIndices[item];
                item.Id = index;
                _connections[item.Id] = item;
            }
            if (IsVisible)
            {
                OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamLine>(new[] {item}));
            }
        }

        public void Update(IEnumerable<SlamLine> items)
        {
            var buffer = new List<SlamLine>();
            lock (_connections)
            {
                foreach (var obj in items)
                {
                    var line = obj;
                    line.Id = _connectionsIndices[line];
                    _connections[line.Id] = line;
                    buffer.Add(line);
                }
            }

            if (IsVisible)
            {
                OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamLine>(buffer));
            }
        }

        public bool Remove(SlamLine obj)
        {
            lock (_connections)
            {
                if (!_connectionsIndices.ContainsKey(obj)) return false;
                var index = _connectionsIndices[obj];
                _connections.Remove(index);
                _freeIds.Enqueue(index);
            }
            if (IsVisible)
            {
                OnRemoved?.Invoke(this, new RemovedEventArgs(new[] {obj.Id}));
            }
            return true;
        }

        public void Remove(IEnumerable<SlamLine> items)
        {
            var ids = new List<int>();
            lock (_connections)
            {
                foreach (var line in items)
                {
                    if (!_connectionsIndices.ContainsKey(line)) continue;
                    var index = _connectionsIndices[line];
                    _connectionsIndices.Remove(line);
                    if (!_connections.ContainsKey(index)) continue;
                    ids.Add(index);
                    _connections.Remove(index);
                    _freeIds.Enqueue(index);
                }
            }

            if (IsVisible)
            {
                OnRemoved?.Invoke(this, new RemovedEventArgs(ids));
            }
        }

        public void RemoveAt(int index)
        {
            SlamLine line;
            lock (_connections)
            {
                line = _connections[index];
            }
            Remove(line);
        }

        public void Clear()
        {
            int[] ids;
            lock (_connections)
            {
                ids = _connections.Keys.ToArray();
                _connections.Clear();
                _connectionsIndices.Clear();
                _freeIds.Clear();
                _maxId = 0;
            }
            if (IsVisible)
            {
                OnRemoved?.Invoke(this, new RemovedEventArgs(ids));
            }
        }

        #endregion

        #region ISourceTree implementation

        public string DisplayName { get; set; }

        public IEnumerable<ISourceTree> Children => Enumerable.Empty<ISourceTree>();

        public void SetRenderer(object renderer)
        {
            if (renderer is ICloudRenderer<SlamLine> typedRenderer)
            {
                OnAdded += typedRenderer.OnItemsAdded;
                OnUpdated += typedRenderer.OnItemsUpdated;
                OnRemoved += typedRenderer.OnItemsRemoved;
                if (Count > 0)
                {
                    OnAdded?.Invoke(this, new AddedEventArgs<SlamLine>(this));
                }
            }
        }

        #endregion

        #region ILookable implementation

        public (Vector3 pos, Quaternion rot) Look(Transform transform)
        {
            lock (_connections)
            {
                if (_connectionsIndices.Count == 0) return (transform.position, transform.rotation);
            
                Vector3 min = Vector3.positiveInfinity;
                Vector3 max = Vector3.negativeInfinity;
                foreach (var point in _connectionsIndices
                        .Keys
                        .SelectMany(l => new[] {l.Point1.Position, l.Point2.Position}))
                {
                    min = new Vector3(Mathf.Min(min.x, point.x), Mathf.Min(min.y, point.y), Mathf.Min(min.z, point.z));
                    max = new Vector3(Mathf.Max(max.x, point.x), Mathf.Max(max.y, point.y), Mathf.Max(max.z, point.z));
                }

                var bounds = max - min;
                var center = (max + min) / 2;

                return (center + bounds / 2 + bounds.normalized, Quaternion.LookRotation(-bounds));
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

                if (_isVisible)
                {
                    OnAdded?.Invoke(this, new AddedEventArgs<SlamLine>(this));
                    return;
                }

                int[] ids;
                lock (_connections)
                {
                    ids = _connections.Keys.ToArray();
                }

                OnRemoved?.Invoke(this, new RemovedEventArgs(ids));
            }
        }

        public event Action<bool> OnVisibleChanged;

        public bool ShowButton => true;

        #endregion

        #region ISnapshotable
        
        public ISnapshotable TakeSnapshot()
        {
            var res = new SlamLinesContainer(DisplayName);
            List<SlamLine> list;
            lock (_connections)
            {
                list = _connections.Values.ToList();
            }
            res.AddRange(list);
            return res;
        }

        #endregion
        
        #region Private definitions
        
        private bool _isVisible = true;

        private readonly IDictionary<int, SlamLine> _connections = new SortedDictionary<int, SlamLine>();
        private readonly IDictionary<SlamLine, int> _connectionsIndices = new SortedDictionary<SlamLine, int>();

        private int _maxId = 0;
        private readonly Queue<int> _freeIds = new Queue<int>();

        #endregion
    }
}