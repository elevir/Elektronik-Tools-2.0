﻿using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers.EventArgs;
using Elektronik.Data.PackageObjects;
using JetBrains.Annotations;

namespace Elektronik.Containers
{
    /// <summary>
    /// Interface of container that allows batched adding, updating, and removing of its elements.
    /// Also rises events on adding, updating, and removing.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IContainer<T> : IList<T> where T : ICloudItem
    {
        [CanBeNull] event EventHandler<AddedEventArgs<T>> OnAdded;

        [CanBeNull] event EventHandler<UpdatedEventArgs<T>> OnUpdated;

        [CanBeNull] event EventHandler<RemovedEventArgs> OnRemoved;

        void AddRange(IEnumerable<T> items);
        void Remove(IEnumerable<T> items);
        
        /// <summary> Removes items by their ids. </summary>
        /// <param name="itemIds"></param>
        /// <returns> List of removed items. </returns>
        IEnumerable<T> Remove(IEnumerable<int> itemIds);
        void Update(T item);
        void Update(IEnumerable<T> items);
        bool Contains(int id);
    }

    public static class ContainerDiffExt
    {
        public static void Add<TCloudItem, TCloudItemDiff>(this IContainer<TCloudItem> container, TCloudItemDiff diff)
                where TCloudItem : ICloudItem
                where TCloudItemDiff : ICloudItemDiff<TCloudItem>
        {
            container.Add(diff.Apply());
        }

        public static void AddRange<TCloudItem, TCloudItemDiff>(this IContainer<TCloudItem> container,
                                                                IEnumerable<TCloudItemDiff> diffs)
                where TCloudItem : ICloudItem
                where TCloudItemDiff : ICloudItemDiff<TCloudItem>
        {
            container.AddRange(diffs.Select(d => d.Apply()));
        }

        public static void Remove<TCloudItem, TCloudItemDiff>(this IContainer<TCloudItem> container,
                                                              TCloudItemDiff diff)
                where TCloudItem : ICloudItem
                where TCloudItemDiff : ICloudItemDiff<TCloudItem>
        {
            container.Remove(diff.Apply());
        }

        public static void Remove<TCloudItem, TCloudItemDiff>(this IContainer<TCloudItem> container,
                                                              IEnumerable<TCloudItemDiff> diffs)
                where TCloudItem : ICloudItem
                where TCloudItemDiff : ICloudItemDiff<TCloudItem>
        {
            container.Remove(diffs.Select(d => d.Apply()));
        }

        public static void Update<TCloudItem, TCloudItemDiff>(this IContainer<TCloudItem> container,
                                                              TCloudItemDiff diff)
                where TCloudItem : ICloudItem
                where TCloudItemDiff : ICloudItemDiff<TCloudItem>
        {
            container.Update(diff.Apply(container[diff.Id]));
        }

        public static void Update<TCloudItem, TCloudItemDiff>(this IContainer<TCloudItem> container,
                                                              IEnumerable<TCloudItemDiff> diff)
                where TCloudItem : ICloudItem
                where TCloudItemDiff : ICloudItemDiff<TCloudItem>
        {
            container.Update(diff.Select(d => d.Apply(container[d.Id])).ToArray());
        }
    }
}