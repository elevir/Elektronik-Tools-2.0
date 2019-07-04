﻿using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Packages;
using System.Linq;

namespace Elektronik.Common.PackageViewUpdateCommandPattern.Slam
{
    public class AddCommand : IPackageViewUpdateCommand
    {
        private readonly SlamLine[] m_addedLines;
        private readonly SlamPoint[] m_addedPoints;
        private readonly SlamObservation[] m_addedObservations;

        private readonly ICloudObjectsContainer<SlamObservation> m_observationsContainer;
        private readonly ICloudObjectsContainer<SlamLine> m_linesContainer;
        private readonly ICloudObjectsContainer<SlamPoint> m_pointsContainer;

        public AddCommand(
            ICloudObjectsContainer<SlamPoint> pointsContainer,
            ICloudObjectsContainer<SlamLine> linesContainer,
            ICloudObjectsContainer<SlamObservation> observationsContainer,
            SlamPackage slamEvent)
        {
            m_pointsContainer = pointsContainer;
            m_linesContainer = linesContainer;
            m_observationsContainer = observationsContainer;

            if (slamEvent.Points != null)
            {
                m_addedPoints = slamEvent.Points.Where(p => p.isNew).ToArray();
            }
            if (slamEvent.Lines != null)
            {
                m_addedLines = slamEvent.Lines.Where(l => !m_linesContainer.Exists(l)).ToArray();
                for (int i = 0; i < m_addedLines.Length; ++i)
                {
                    m_addedLines[i].isRemoved = true;
                    m_addedLines[i].vert1 = m_pointsContainer[m_addedLines[i].pointId1].position;
                    m_addedLines[i].vert2 = m_pointsContainer[m_addedLines[i].pointId2].position;
                }
            }
            if (slamEvent.Observations != null)
            {
                m_addedObservations = slamEvent.Observations.Where(o => o.Point.isNew && o.Point.id != -1).ToArray();
            }
        }

        public void Execute()
        {
            if (m_addedPoints != null)
            {
                foreach (var point in m_addedPoints)
                {
                    m_pointsContainer.Add(point);
                }
            }
            if (m_addedLines != null)
            {
                foreach (var line in m_addedLines)
                {
                    m_linesContainer.Add(line);
                }
            }
            if (m_addedObservations != null)
            {
                foreach (var observation in m_addedObservations)
                {
                    m_observationsContainer.Add(observation);
                }
            }
        }

        public void UnExecute()
        {
            if (m_addedPoints != null)
            {
                foreach (var point in m_addedPoints)
                {
                    m_pointsContainer.Remove(point);
                }
            }
            if (m_addedLines != null)
            {
                foreach (var line in m_addedLines)
                {
                    m_linesContainer.Remove(line);
                }
            }
            if (m_addedObservations != null)
            {
                foreach (var observation in m_addedObservations)
                {
                    m_observationsContainer.Remove(observation);
                }
            }
        }
    }
}
