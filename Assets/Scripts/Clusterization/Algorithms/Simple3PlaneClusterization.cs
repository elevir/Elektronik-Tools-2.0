﻿using System.Collections.Generic;
using System.Linq;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Clusterization.Algorithms
{
    public class Simple3PlaneClusterization : IClusterizationAlgorithm
    {
        public Simple3PlaneClusterization(Vector3 offset)
        {
            _offset = offset;
        }

        public List<List<SlamPoint>> Compute(IList<SlamPoint> items)
        {
            var l1 = items.Where(i => i.Position.x > _offset.x
                                         && i.Position.y > _offset.y
                                         && i.Position.z > _offset.z).ToList();
            var l2 = items.Where(i => i.Position.x > _offset.x
                                         && i.Position.y > _offset.y
                                         && i.Position.z < _offset.z).ToList();
            var l3 = items.Where(i => i.Position.x > _offset.x
                                         && i.Position.y < _offset.y
                                         && i.Position.z > _offset.z).ToList();
            var l4 = items.Where(i => i.Position.x > _offset.x
                                         && i.Position.y < _offset.y
                                         && i.Position.z < _offset.z).ToList();
            var l5 = items.Where(i => i.Position.x < _offset.x
                                         && i.Position.y > _offset.y
                                         && i.Position.z > _offset.z).ToList();
            var l6 = items.Where(i => i.Position.x < _offset.x
                                         && i.Position.y > _offset.y
                                         && i.Position.z < _offset.z).ToList();
            var l7 = items.Where(i => i.Position.x < _offset.x
                                         && i.Position.y < _offset.y
                                         && i.Position.z > _offset.z).ToList();
            var l8 = items.Where(i => i.Position.x < _offset.x
                                         && i.Position.y < _offset.y
                                         && i.Position.z < _offset.z).ToList();

            var res = new List<List<SlamPoint>> {l1, l2, l3, l4, l5, l6, l7, l8};

            return res;
        }

        private readonly Vector3 _offset;
    }
}