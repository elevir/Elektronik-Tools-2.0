﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common
{
    public struct SlamLine
    {
        public Vector3 vert1;
        public Vector3 vert2;
        public int pointId1;
        public int pointId2;
        public Color color;
        public bool isRemoved;

        public long GenerateLongId()
        {
            return pointId1 * UInt32.MaxValue + pointId2;
        }

    }
}
