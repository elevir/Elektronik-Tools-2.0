﻿using System;
using Elektronik.Data.PackageObjects;
using Elektronik.Extensions;
using UnityEngine;

namespace Elektronik.Clouds
{
    public struct GPUItem
    {
        public const int Size = sizeof(float) * 4;
            
        public Vector3 Position;
        public uint Color;
        
        public GPUItem(SlamPoint cp)
        {
            Position = cp.Position;
            Color = EncodeColor(cp.Color);
        }

        public GPUItem(Vector3 offset, Color color)
        {
            Position = offset;
            Color = EncodeColor(color);
        }

        public GPUItem((Vector3 offset, Color color) vert)
        {
            Position = vert.offset;
            Color = EncodeColor(vert.color);
        }

        public GPUItem(Quaternion rotation)
        {
            Position = new Vector3(rotation.x, rotation.y, rotation.z);
            Color = BitConverterEx.ToUInt32(BitConverter.GetBytes(rotation.w), 0);
        }
            
        static uint EncodeColor(Color c)
        {
            const float kMaxBrightness = 16;

            var y = Mathf.Max(Mathf.Max(c.r, c.g), c.b);
            y = Mathf.Clamp(Mathf.Ceil(y * 255 / kMaxBrightness), 1, 255);

            var rgb = new Vector3(c.r, c.g, c.b);
            rgb *= 255 * 255 / (y * kMaxBrightness);

            return ((uint)rgb.x      ) |
                   ((uint)rgb.y <<  8) |
                   ((uint)rgb.z << 16) |
                   ((uint)y     << 24);
        }
    }
}