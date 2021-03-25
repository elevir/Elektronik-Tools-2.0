﻿using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;
using Elektronik.RosPlugin.Common.Containers;
using Elektronik.RosPlugin.Ros.Bag.Parsers;
using Elektronik.Settings;

namespace Elektronik.RosPlugin.Ros.Bag
{
    public class RosbagContainerTree : RosContainerTree
    {
        private static readonly Dictionary<string, Type> SupportedMessages = new()
        {
            {"geometry_msgs/PoseStamped", typeof(TrackedObjectsContainer)},
            {"nav_msgs/Odometry", typeof(TrackedObjectsContainer)},
            {"sensor_msgs/PointCloud2", typeof(CloudContainer<SlamPoint>)},
        };
        
        public BagParser? Parser { get; private set; }
        
        public RosbagContainerTree(string displayName) : base(displayName)
        {
        }

        public void Init(FileScaleSettingsBag settings)
        {
            DisplayName = settings.FilePath.Split('/').LastOrDefault(s => !string.IsNullOrEmpty(s)) ?? "Rosbag: /";
            Parser = new BagParser(settings.FilePath);
            ActualTopics = Parser.GetTopics()
                    .Where(t => SupportedMessages.ContainsKey(t.Type))
                    .Select(t => (t.Topic, t.Type))
                    .ToList();
            RebuildTree();
        }

        public override void Reset()
        {
            Parser?.Dispose();
            base.Reset();
        }

        #region Protected

        protected override IContainerTree CreateContainer(string topicName, string topicType)
        {
            return (IContainerTree) Activator.CreateInstance(SupportedMessages[topicType],
                                                             topicName.Split('/').Last());
        }

        #endregion
    }
}