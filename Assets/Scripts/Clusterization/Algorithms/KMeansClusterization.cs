﻿using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Elektronik.Data.PackageObjects;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace Elektronik.Clusterization.Algorithms
{
    public class KMeansClusterization : IClusterizationAlgorithm
    {
        public KMeansClusterization(int k)
        {
            _k = k;
        }

        public List<List<SlamPoint>> Compute(IList<SlamPoint> items)
        {
            var res = Enumerable.Range(0, _k).Select(_ => new List<SlamPoint>()).ToList();
            var clusters = ComputeClusters(items);
            for (int i = 0; i < clusters.Count; i++)
            {
                res[clusters[i]].Add(items[i]);
            }

            return res;
        }

        private List<int> ComputeClusters(IList<SlamPoint> points)
        {
            File.WriteAllLines("tmp.csv", points
                                       .Select(p => $"{p.Position.x.ToString(CultureInfo.InvariantCulture)};" +
                                                       $"{p.Position.y.ToString(CultureInfo.InvariantCulture)};" +
                                                       $"{p.Position.z.ToString(CultureInfo.InvariantCulture)}"));
            var mlContext = new MLContext(seed: 0);
            var reader = mlContext.Data.CreateTextLoader(
                new[]
                {
                    new TextLoader.Column("X", DataKind.Single, 0),
                    new TextLoader.Column("Y", DataKind.Single, 1),
                    new TextLoader.Column("Z", DataKind.Single, 2),
                },
                hasHeader: false,
                separatorChar: ';'
            );

            var dataView = reader.Load("tmp.csv");
            var pipeline = mlContext.Transforms
                    .Concatenate("Features", "X", "Y", "Z")
                    .Append(mlContext.Clustering.Trainers.KMeans(numberOfClusters: _k));

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
    }
}