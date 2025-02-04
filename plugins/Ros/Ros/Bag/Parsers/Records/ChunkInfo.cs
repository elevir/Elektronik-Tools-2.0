﻿using System;
using System.Collections.Generic;
using System.IO;
using Elektronik.RosPlugin.Common;

namespace Elektronik.RosPlugin.Ros.Bag.Parsers.Records
{
    [Serializable]
    public class ChunkInfo : Record
    {
        public const byte OpCode = 0x06;

        public readonly int Version;
        public readonly long ChunkPos;
        public readonly long StartTime;
        public readonly long EndTime;
        public readonly int ConnectionCount;
        
        public ChunkInfo(Dictionary<string, byte[]> header) : base(header)
        {
            if (Op != OpCode) throw new ParsingException("Can't read ChunkInfo");

            Version = BitConverter.ToInt32(Header["ver"], 0);
            ChunkPos = BitConverter.ToInt64(Header["chunk_pos"], 0);
            StartTime = BitConverter.ToInt64(Header["start_time"], 0);
            EndTime = BitConverter.ToInt64(Header["end_time"], 0);
            ConnectionCount = BitConverter.ToInt32(Header["count"], 0);
        }

        public IEnumerable<int> GetIds()
        {
            using var stream = new MemoryStream(Data ?? Array.Empty<byte>());
            while (stream.Position < stream.Length)
            {
                var conn = stream.ReadInt32();
                var _ = stream.ReadInt32(); // count
                yield return conn;
            }
        }
    }
}