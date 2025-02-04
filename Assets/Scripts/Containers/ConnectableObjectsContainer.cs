﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers.EventArgs;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using Elektronik.PluginsSystem;
using UnityEngine;

namespace Elektronik.Containers
{
    public class ConnectableObjectsContainer<TCloudItem> : IConnectableObjectsContainer<TCloudItem>, ISourceTree,
                                                           ILookable, IVisible, ISnapshotable
            where TCloudItem : struct, ICloudItem
    {
        public ConnectableObjectsContainer(IContainer<TCloudItem> objects,
                                           IContainer<SlamLine> connects,
                                           string displayName = "",
                                           SparseSquareMatrix<bool> table = null)
        {
            _connects = connects;
            _objects = objects;
            _table = table ?? new SparseSquareMatrix<bool>();
            Children = new[]
            {
                (ISourceTree) _connects,
                (ISourceTree) _objects,
            };

            DisplayName = string.IsNullOrEmpty(displayName) ? GetType().Name : displayName;
        }

        #region IContainer implementation

        public event EventHandler<AddedEventArgs<TCloudItem>> OnAdded;

        public event EventHandler<UpdatedEventArgs<TCloudItem>> OnUpdated;

        public event EventHandler<RemovedEventArgs> OnRemoved;

        public int Count => _objects.Count;

        public bool IsReadOnly => false;

        
        public bool Contains(int id) => _objects.Contains(id);
        
        public bool Contains(TCloudItem obj) => Contains(obj.Id);

        public IEnumerator<TCloudItem> GetEnumerator() => _objects.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _objects.GetEnumerator();

        public int IndexOf(TCloudItem item) => _objects.IndexOf(item);

        public void CopyTo(TCloudItem[] array, int arrayIndex) => _objects.CopyTo(array, arrayIndex);

        public TCloudItem this[int id]
        {
            get => _objects[id];
            set => _objects[id] = value;
        }

        public void Add(TCloudItem obj)
        {
            lock (_table)
            {
                _objects.Add(obj);
            }

            OnAdded?.Invoke(this, new AddedEventArgs<TCloudItem>(new[] {obj}));
        }

        public void Insert(int index, TCloudItem item)
        {
            lock (_table)
            {
                _objects.Insert(index, item);
            }

            OnAdded?.Invoke(this, new AddedEventArgs<TCloudItem>(new[] {item}));
        }

        public void AddRange(IEnumerable<TCloudItem> items)
        {
            if (items is null) return;
            var list = items.ToList();
            lock (_table)
            {
                _objects.AddRange(list);
            }

            OnAdded?.Invoke(this, new AddedEventArgs<TCloudItem>(list));
        }

        public void Update(TCloudItem item)
        {
            lock (_table)
            {
                _objects.Update(item);
                foreach (var secondId in _table.GetColIndices(item.Id))
                {
                    _connects.Update(new SlamLine(item.Id, secondId));
                }
            }

            OnUpdated?.Invoke(this, new UpdatedEventArgs<TCloudItem>(new[] {item}));
        }

        public void Update(IEnumerable<TCloudItem> items)
        {
            if (items is null) return;
            var list = items.ToList();
            lock (_table)
            {
                _objects.Update(list);
                _linesBuffer.Clear();
                foreach (var pt1 in list)
                {
                    foreach (var pt2Id in _table.GetColIndices(pt1.Id))
                    {
                        _linesBuffer.Add(new SlamLine(pt1.AsPoint(), _objects[pt2Id].AsPoint()));
                    }
                }

                _connects.Update(_linesBuffer);
            }

            OnUpdated?.Invoke(this, new UpdatedEventArgs<TCloudItem>(list));
        }

        public void RemoveAt(int id)
        {
            lock (_table)
            {
                RemoveConnections(id);
                _objects.RemoveAt(id);
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs(new[] {id}));
        }

        public bool Remove(TCloudItem obj)
        {
            lock (_table)
            {
                int index = _objects.IndexOf(obj);
                if (index != -1)
                {
                    RemoveConnections(index);
                    _objects.Remove(obj);
                    OnRemoved?.Invoke(this, new RemovedEventArgs(new[] {obj.Id}));
                    return true;
                }

                return false;
            }
        }

        public void Remove(IEnumerable<TCloudItem> items)
        {
            if (items is null) return;
            var list = items.ToList();
            _linesBuffer.Clear();
            lock (_table)
            {
                foreach (var obj in list)
                {
                    int id = _objects.IndexOf(obj);
                    if (id != -1)
                    {
                        foreach (var col in _table.GetColIndices(id))
                        {
                            _linesBuffer.Add(
                                new SlamLine(
                                    new SlamPoint(id, default, default),
                                    new SlamPoint(col, default, default)));
                            _table.Remove(col, id);
                        }

                        _table.RemoveRow(id);
                    }
                }

                _connects.Remove(_linesBuffer);
                _objects.Remove(list);
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs(list.Select(i => i.Id).ToList()));
        }

        public IEnumerable<TCloudItem> Remove(IEnumerable<int> items)
        {
            if (items is null) return new List<TCloudItem>();
            var list = items.ToList();
            _linesBuffer.Clear();
            List<TCloudItem> removed;
            lock (_table)
            {
                foreach (var id in list)
                {
                    if (id != -1)
                    {
                        foreach (var col in _table.GetColIndices(id))
                        {
                            _linesBuffer.Add(
                                new SlamLine(
                                    new SlamPoint(id, default, default),
                                    new SlamPoint(col, default, default)));
                            _table.Remove(col, id);
                        }

                        _table.RemoveRow(id);
                    }
                }

                _connects.Remove(_linesBuffer);
                removed = _objects.Remove(list).ToList();
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs(list));
            return removed;
        }

        public void Clear()
        {
            var list = _objects.ToList();
            lock (_table)
            {
                _table.Clear();
                _connects.Clear();
                _objects.Clear();
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs(list.Select(i => i.Id).ToList()));
        }

        #endregion

        #region IConnectableObjectsContainer implementation

        public void AddConnections(IEnumerable<(int id1, int id2)> connections)
        {
            if (connections is null) return;
            _linesBuffer.Clear();
            var list = connections.ToList();
            foreach (var c in list)
            {
                AddConnection(c.id1, c.id2, line => _linesBuffer.Add(line));
            }

            _connects.AddRange(_linesBuffer);
            OnConnectionsUpdated?.Invoke(this, new ConnectionsEventArgs(list));
        }

        public void RemoveConnections(IEnumerable<(int id1, int id2)> connections)
        {
            if (connections is null) return;
            _linesBuffer.Clear();
            var list = connections.ToList();
            foreach (var c in list)
            {
                RemoveConnection(c.id1, c.id2, _linesBuffer.Add);
            }

            _connects.Remove(_linesBuffer);
            OnConnectionsRemoved?.Invoke(this, new ConnectionsEventArgs(list));
        }

        public IEnumerable<(int, int)> GetAllConnections(int id)
        {
            foreach (var col in _table.GetColIndices(id))
            {
                yield return (id, col);
            }
        }

        public IEnumerable<(int, int)> GetAllConnections(TCloudItem obj)
        {
            int idx = _objects.IndexOf(obj);
            if (idx != -1)
            {
                return GetAllConnections(idx);
            }

            return Enumerable.Empty<(int, int)>();
        }

        public event EventHandler<ConnectionsEventArgs> OnConnectionsUpdated;

        public event EventHandler<ConnectionsEventArgs> OnConnectionsRemoved;

        #endregion

        #region ISourceTree imlementation

        public string DisplayName { get; set; }

        public IEnumerable<ISourceTree> Children { get; }

        public void SetRenderer(ISourceRenderer renderer)
        {
            foreach (var child in Children)
            {
                child.SetRenderer(renderer);
            }
        }

        #endregion

        #region ILookable implementation

        public (Vector3 pos, Quaternion rot) Look(Transform transform)
        {
            return (_objects as ILookable)?.Look(transform) ?? (transform.position, transform.rotation);
        }

        #endregion

        #region IVisible

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                foreach (var child in Children.OfType<IVisible>())
                {
                    child.IsVisible = value;
                }

                _isVisible = value;
                OnVisibleChanged?.Invoke(_isVisible);
            }
        }

        public event Action<bool> OnVisibleChanged;

        public bool ShowButton => true;

        #endregion

        #region ISnapshotable

        public ISnapshotable TakeSnapshot()
        {
            var objects = (_objects as ISnapshotable)!.TakeSnapshot() as IContainer<TCloudItem>;
            var connects = (_connects as ISnapshotable)!.TakeSnapshot() as IContainer<SlamLine>;
            return new ConnectableObjectsContainer<TCloudItem>(objects, connects, DisplayName, _table.DeepCopy());
        }

        public void WriteSnapshot(IDataRecorderPlugin recorder)
        {
            recorder.OnAdded(DisplayName, _objects.ToList());
            recorder.OnConnectionsUpdated<TCloudItem>(DisplayName, _connects.Select(l => (l.Point1.Id, l.Point2.Id)).ToList());
        }

        #endregion

        #region Private definitions

        private readonly List<SlamLine> _linesBuffer = new List<SlamLine>();
        private readonly SparseSquareMatrix<bool> _table;
        private readonly IContainer<SlamLine> _connects;
        private readonly IContainer<TCloudItem> _objects;
        private bool _isVisible = true;

        public IEnumerable<SlamLine> Connections => _connects;

        private bool AddConnection(int id1, int id2)
        {
            if (_table[id1, id2].HasValue) return false;

            _table[id1, id2] = _table[id2, id1] = true;
            return true;
        }

        private bool AddConnection(int id1, int id2, Action<SlamLine> adding)
        {
            var res = AddConnection(id1, id2);
            if (res && _objects.Contains(new TCloudItem {Id = id1}) && _objects.Contains(new TCloudItem {Id = id2}))
            {
                adding(new SlamLine(_objects[id1].AsPoint(), _objects[id2].AsPoint()));
            }

            return res;
        }

        private void RemoveConnections(int id)
        {
            foreach (var col in _table.GetColIndices(id))
            {
                _connects.Remove(new SlamLine(id, col));
                _table.Remove(col, id);
            }

            _table.RemoveRow(id);
        }

        private bool RemoveConnection(int id1, int id2, Action<SlamLine> removing)
        {
            if (id1 != -1 && id2 != -1)
            {
                if (_table.Contains(id1, id2) && _table[id1, id2].HasValue && _table[id1, id2].Value)
                {
                    _table.Remove(id1, id2);
                    _table.Remove(id2, id1);
                    removing(new SlamLine(_objects[id1].AsPoint(), _objects[id2].AsPoint()));
                    return true;
                }

                Debug.LogWarning($"Connection {id1} - {id2} not exists.");
            }

            return false;
        }

        #endregion
    }
}