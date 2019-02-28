﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.Data
{
    public class Package
    {
        public bool IsKeyEvent { get; private set; }
        public int Timestamp { get; private set; }
        public List<SlamObservation> Observations { get; private set; }
        public List<SlamPoint> Points { get; private set; }
        public List<SlamLine> Lines { get; private set; }
        public string EventType { get; private set; }

        private string m_summary;
        private void EvaluateSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[Package]")
              .AppendLine()
              .AppendFormat("{0}", EventType)
              .AppendLine()
              .AppendFormat("Timestamp: {0}", Timestamp)
              .AppendLine()
              .AppendFormat("Is key: {0}", IsKeyEvent)
              .AppendLine()
              .AppendFormat("New points count: {0}", Points.Count(p => p.isNew))
              .AppendLine()
              .AppendFormat("Tinted points count: {0}", Points.Count(p => p.justColored))
              .AppendLine()
              .AppendFormat("Removed points count: {0}", Points.Count(p => p.isRemoved))
              .AppendLine()
              .AppendFormat("Total count of points: {0}", Points.Count)
              .AppendLine()
              .AppendFormat("New observations count: {0}", Observations.Count(o => o.Point.isNew))
              .AppendLine()
              .AppendFormat("Removed observations count: {0}", Observations.Count(o => o.Point.isRemoved))
              .AppendLine()
              .AppendFormat("Total count of observations: {0}", Observations.Count);
            m_summary = sb.ToString();
        }

        private Package()
        {
            Observations = new List<SlamObservation>(20);
            Points = new List<SlamPoint>(5000);
            Lines = new List<SlamLine>(1000);
        }

        public static Package Parse(byte[] rawPackage)
        {
            Package result = new Package();
            int offset = 0;
            result.Timestamp = BitConverter.ToInt32(rawPackage, 0);
            offset += sizeof(int);

            int sizeInBytesOfEventType = BitConverter.ToInt32(rawPackage, offset);
            offset += sizeof(int);

            result.EventType = sizeInBytesOfEventType > 0 ?
                Encoding.ASCII.GetString(rawPackage, offset, sizeInBytesOfEventType) :
                "";
            offset += sizeInBytesOfEventType;
            int countOfObjects = BitConverter.ToInt32(rawPackage, offset);
            offset += sizeof(int);

            for (int i = 0; i < countOfObjects; ++i)
            {
                int objectId = BitConverter.ToInt32(rawPackage, offset);
                offset += sizeof(int);
                byte objectType = rawPackage[offset++];
                int actionsCount = rawPackage[offset++];
                int actionsSize = 0;
                for (int actionIdx = 0; actionIdx < actionsCount; ++actionIdx)
                {
                    actionsSize +=
                        objectType == 0
                        ? SlamPointsPackageObject.GetSizeOfActionInBytes((ActionType)rawPackage[offset + actionsSize])
                        : SlamObservationPackageObject.GetSizeOfActionInBytes((ActionType)rawPackage[offset + actionsSize]);
                    ++actionsSize; // type byte
                }
                byte[] actions = new byte[actionsSize];
                Debug.AssertFormat(
                    offset + actionsSize <= rawPackage.Length,
                    "[Package.Parse] Wrong size of action. actionSize + offset = {0}, but size of package is {1}",
                    offset + actionsSize, rawPackage.Length);
                Array.Copy(rawPackage, offset, actions, 0, actionsSize);
                offset += actionsSize;
                SlamLine? line = null;
                if (objectType == 0)
                {
                    SlamPoint point = new SlamPoint();
                    SlamPointsPackageObject.ParseActions(actions, objectId, out point, out line);
                    result.Points.Add(point);
                }
                else
                {
                    SlamObservation observation = null;
                    SlamObservationPackageObject.ParseActions(actions, objectId, out observation);
                    result.Observations.Add(observation);
                }
                if (line != null)
                    result.Lines.Add(line.Value);
            }
            if (result.Observations.Count > 0)
            {
                if (result.Observations[0].Point.id != -1)
                    result.IsKeyEvent = true;
            }
            result.EvaluateSummary();
            return result;
        }

        public string Summary()
        {
            return m_summary;
        }
    }
}
