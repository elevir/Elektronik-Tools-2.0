﻿using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Elektronik.Data.PackageObjects;
using Microsoft.ML;
using UnityEngine;

namespace Elektronik.Clusterization.Algorithms
{
    public class KMeansClusterization : IClusterizationAlgorithm
    {
        public KMeansClusterization(int k)
        {
            _k = k;
            for (int i = _colors.Count; i < k; i++)
            {
                _colors.Add(new Color(Random.value, Random.value, Random.value));
            }
        }

        public List<List<SlamPoint>> Compute(IList<SlamPoint> items)
        {
            var res = Enumerable.Range(0, _k).Select(_ => new List<SlamPoint>()).ToList();
            var clusters = ComputeClusters(items);
            for (int i = 0; i < clusters.Count; i++)
            {
                var point = items[i];
                point.Color = _colors[clusters[i]];
                res[clusters[i]].Add(point);
            }

            return res;
        }

        private List<int> ComputeClusters(IList<SlamPoint> points)
        {
            File.WriteAllLines("tmp.csv", points
                                       .Select(p => $"{p.Position.x.ToString(CultureInfo.InvariantCulture)}\t" +
                                                       $"{p.Position.y.ToString(CultureInfo.InvariantCulture)}\t" +
                                                       $"{p.Position.z.ToString(CultureInfo.InvariantCulture)}\t"));
            var mlContext = new MLContext(seed: 0);
            var dataView = mlContext.Data.LoadFromTextFile("tmp.csv");

            var pipeline = mlContext.Clustering.Trainers.KMeans(numberOfClusters: _k);
            var transformed = pipeline.Fit(dataView).Transform(dataView);
            var clusterColumn = transformed.Schema.GetColumnOrNull("PredictedLabel")!.Value;
            var cursor = transformed.GetRowCursor(new[] {clusterColumn});
            var clusterizatorResult = cursor.GetGetter<uint>(clusterColumn);

            var res = new List<int>();
            while (cursor.MoveNext())
            {
                uint cluster = 0;
                clusterizatorResult(ref cluster);
                res.Add((int) cluster - 1);
            }

            File.Delete("tmp.csv");

            return res;
        }

        private readonly int _k;

        private readonly List<Color> _colors = new List<Color>
        {
            Color.blue,
            Color.red,
            Color.green,
            Color.yellow,
            Color.magenta,
            new Color(0.5f, 0.5f, 1f),
            new Color(1f, 0.5f, 0),
            new Color(0.5f, 0, 1)
        };
    }
}