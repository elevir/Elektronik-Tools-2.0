﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Elektronik.Containers.EventArgs;
using Elektronik.Data.PackageObjects;
using UnityEngine;
using Grid = Elektronik.Cameras.Grid;

namespace Elektronik.Clouds
{
    /// <summary> Base class for rendering object clouds. Such as point cloud, line cloud, etc. </summary>
    /// <typeparam name="TCloudItem"></typeparam>
    /// <typeparam name="TCloudBlock"></typeparam>
    public abstract class CloudRenderer<TCloudItem, TCloudBlock>
            : CloudRendererComponent<TCloudItem>
            where TCloudItem : struct, ICloudItem
            where TCloudBlock : CloudBlock
    {
        public const string GridMessage = "Grid";
        public Grid Grid;
        public Shader CloudShader;

        public float ItemSize;

        public IEnumerable<Vector3> GetPoints() => _pointPlaces.Values
                .Select(v => _blocks[v / CloudBlock.Capacity].GetItems()[v % CloudBlock.Capacity])
                .Select(i => i.Position);

        public override int ItemsCount => _amountOfItems;

        public void SetSize(float newSize)
        {
            ItemSize = newSize;

            foreach (var block in _blocks)
            {
                block.ItemSize = ItemSize;
            }
        }

        #region Unity events

        private void Start()
        {
            CreateNewBlock();
        }

        private void Update()
        {
            foreach (var block in _blocks)
            {
                block.ItemSize = ItemSize;
            }
        }

        #endregion

        #region ICloudRenderer implementation

        public override void OnItemsAdded(object sender, AddedEventArgs<TCloudItem> e)
        {
            if (!IsSenderVisible(sender)) return;
            var list = e.AddedItems.ToList();
            if (CheckAndCreateReserves(sender, list)) return;
            AddItems(sender, list);
        }

        public override void OnItemsUpdated(object sender, UpdatedEventArgs<TCloudItem> e)
        {
            if (!IsSenderVisible(sender)) return;
            lock (_pointPlaces)
            {
                foreach (var item in e.UpdatedItems)
                {
                    if (item.Message == GridMessage && item is SlamInfinitePlane) continue;
                    var index = _pointPlaces[(sender.GetHashCode(), item.Id)];
                    int layer = index / CloudBlock.Capacity;
                    int inLayerId = index % CloudBlock.Capacity;
                    lock (_blocks[layer])
                    {
                        ProcessItem(_blocks[layer], item, inLayerId);
                        _blocks[layer].Updated = true;
                    }
                }
            }
        }

        public override void OnItemsRemoved(object sender, RemovedEventArgs e)
        {
            lock (_pointPlaces)
            {
                foreach (var itemId in e.RemovedIds)
                {
                    if (!_pointPlaces.ContainsKey((sender.GetHashCode(), itemId))) continue;

                    var index = _pointPlaces[(sender.GetHashCode(), itemId)];
                    _pointPlaces.Remove((sender.GetHashCode(), itemId));
                    if (index == _maxPlace - 1) _maxPlace--;
                    else _freePlaces.Enqueue(index);

                    int layer = index / CloudBlock.Capacity;
                    int inLayerId = index % CloudBlock.Capacity;
                    lock (_blocks[layer])
                    {
                        RemoveItem(_blocks[layer], inLayerId);
                        _blocks[layer].Updated = true;
                    }
                }

                _amountOfItems -= e.RemovedIds.Count();
            }
        }

        public override void ShowItems(object sender, IEnumerable<TCloudItem> items)
        {
            if (!IsSenderVisible(sender)) return;
            OnClear(sender);
            var list = items.ToList();
            if (CheckAndCreateReserves(sender, list)) return;
            AddItems(sender, list);
        }

        public override void OnClear(object sender)
        {
            lock (_pointPlaces)
            {
                var keys = _pointPlaces.Keys.Where(k => k.Item1 == sender.GetHashCode()).ToList();
                foreach (var key in keys)
                {
                    var index = _pointPlaces[key];
                    _pointPlaces.Remove(key);
                    if (index == _maxPlace - 1) _maxPlace--;
                    else _freePlaces.Enqueue(index);

                    int layer = index / CloudBlock.Capacity;
                    int inLayerId = index % CloudBlock.Capacity;
                    lock (_blocks[layer])
                    {
                        RemoveItem(_blocks[layer], inLayerId);
                        _blocks[layer].Updated = true;
                    }
                }

                _amountOfItems -= keys.Count;
            }
        }

        #endregion

        #region Protected definitions

        protected abstract void ProcessItem(TCloudBlock block, TCloudItem item, int inBlockId);

        protected abstract void RemoveItem(TCloudBlock block, int inBlockId);

        #endregion

        #region Private definitions

        private readonly List<TCloudBlock> _blocks = new List<TCloudBlock>();
        private readonly Dictionary<(int, int), int> _pointPlaces = new Dictionary<(int, int), int>();
        private readonly Queue<int> _freePlaces = new Queue<int>();
        private int _maxPlace = 0;
        private int _amountOfItems;

        private void CreateNewBlock()
        {
            var go = new GameObject($"{GetType().Name} {_blocks.Count}");
            go.transform.SetParent(transform);
            var block = go.AddComponent<TCloudBlock>();
            block.CloudShader = CloudShader;
            block.Updated = true;
            _blocks.Add(block);
        }

        private void AddItems(object sender, IList<TCloudItem> items, bool takeLock = true)
        {
            if (takeLock) Monitor.Enter(_pointPlaces);
            foreach (var item in items)
            {
                if (item.Message == GridMessage && item is SlamInfinitePlane plane)
                {
                    MainThreadInvoker.Instance.Enqueue(() => Grid.SetPlane(plane));
                    continue;
                }

                var index = _freePlaces.Count > 0 ? _freePlaces.Dequeue() : _maxPlace++;
                _pointPlaces.Add((sender.GetHashCode(), item.Id), index);
                int layer = index / CloudBlock.Capacity;
                int inLayerId = index % CloudBlock.Capacity;
                lock (_blocks[layer])
                {
                    ProcessItem(_blocks[layer], item, inLayerId);
                    _blocks[layer].Updated = true;
                }
            }

            _amountOfItems += items.Count;
            if (takeLock) Monitor.Exit(_pointPlaces);
        }

        private bool CheckAndCreateReserves(object sender, IList<TCloudItem> items)
        {
            var newAmountOfItems = _amountOfItems + items.Count;
            if (newAmountOfItems > _blocks.Count * CloudBlock.Capacity)
            {
                // reserves overloaded
                MainThreadInvoker.Instance.Enqueue(() =>
                {
                    lock (_pointPlaces)
                    {
                        var requiredSpace = (newAmountOfItems - _blocks.Count * CloudBlock.Capacity) +
                                CloudBlock.Capacity;
                        var requiredBlocks = requiredSpace / CloudBlock.Capacity;
                        if (requiredBlocks < 0)
                        {
                            throw new ArgumentOutOfRangeException(nameof(requiredBlocks),
                                                                  "Can't be negative required blocks");
                        }

                        for (int i = 0; i < requiredBlocks; i++)
                        {
                            CreateNewBlock();
                        }

                        AddItems(sender, items, false);
                    }
                });
                return true;
            }

            if (newAmountOfItems > (_blocks.Count - 1) * CloudBlock.Capacity)
            {
                // add reserves
                MainThreadInvoker.Instance.Enqueue(CreateNewBlock);
            }

            return false;
        }

        #endregion
    }
}