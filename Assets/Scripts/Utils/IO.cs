using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Assets
{
    public static class IO
    {
        private static readonly int _inMM = 1000;

        public static Bounds CalculateBounds(List<Vector3> points)
        {
            if (points == null || points.Count == 0)
                return new Bounds(Vector3.zero, Vector3.zero);

            Vector3 min = points[0];
            Vector3 max = points[0];

            foreach (Vector3 point in points)
            {
                min = Vector3.Min(min, point);
                max = Vector3.Max(max, point);
            }

            return new Bounds((min + max) * 0.5f, max - min);
        }

        public static void CollectSceneInfo(IDictionary<string, List<Vector3>> source)
        {
            StringBuilder builder = new();
            foreach (string name in source.Keys)
            {
                builder.AppendLine($"{name}");
                Bounds bounds = CalculateBounds(source[name]);

                builder.AppendLine($"(min: {bounds.min.x} {bounds.min.y} {bounds.min.z})");
                builder.AppendLine($"(max: {bounds.max.x} {bounds.max.y} {bounds.max.z})");

                foreach (Vector3 vertex in source[name])
                {
                    int x = (int)Math.Floor(vertex.x * _inMM);
                    int y = (int)Math.Floor(vertex.y * _inMM);
                    int z = (int)Math.Floor(vertex.z * _inMM);

                    builder.AppendLine($"({x} {y} {z})");


                }
            }
            File.WriteAllText("scene.txt", builder.ToString());
        }
    }
}
