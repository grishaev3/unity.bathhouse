using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Types
{
    public class OverlapChecker
    {
        public void CheckOverlaps(Transform transform)
        {
            List<MeshData> allMeshes = new List<MeshData>();
            CollectMeshes(transform, allMeshes);

            for (int i = 0; i < allMeshes.Count; i++)
            {
                for (int j = i + 1; j < allMeshes.Count; j++)
                {
                    CheckMeshPair(allMeshes[i], allMeshes[j]);
                }
            }
        }

        void CollectMeshes(Transform parent, List<MeshData> meshes)
        {
            MeshFilter mf = parent.GetComponent<MeshFilter>();
            if (mf && mf.sharedMesh)
            {
                meshes.Add(new MeshData(parent, mf.sharedMesh));
            }
            foreach (Transform child in parent)
            {
                CollectMeshes(child, meshes);
            }
        }
        void CheckMeshPair(MeshData m1, MeshData m2)
        {
            Vector3[] v1 = m1.vertices;
            int[] t1 = m1.triangles;
            Vector3[] v2 = m2.vertices;
            int[] t2 = m2.triangles;

            for (int i = 0; i < t1.Length; i += 3)
            {
                Triangle tri1 = new Triangle(v1[t1[i]], v1[t1[i + 1]], v1[t1[i + 2]]);
                for (int j = 0; j < t2.Length; j += 3)
                {
                    Triangle tri2 = new Triangle(v2[t2[j]], v2[t2[j + 1]], v2[t2[j + 2]]);
                    if (TrianglesIntersect(tri1, tri2))
                    {
                        Debug.Log($"Пересечение: {m1.transform.name} три {i / 3} и {m2.transform.name} три {j / 3}", m1.transform.gameObject);
                    }
                }
            }
        }

        bool TrianglesIntersect(Triangle t1, Triangle t2)
        {
            // Простая проверка AABB
            Bounds b1 = new Bounds((t1.a + t1.b + t1.c) / 3, Vector3.zero);
            b1.Encapsulate(t1.a); b1.Encapsulate(t1.b); b1.Encapsulate(t1.c);
            Bounds b2 = new Bounds((t2.a + t2.b + t2.c) / 3, Vector3.zero);
            b2.Encapsulate(t2.a); b2.Encapsulate(t2.b); b2.Encapsulate(t2.c);
            if (!b1.Intersects(b2)) return false;

            // Полная проверка пересечения (упрощенная, используйте SAT или raycast для точности)
            return CheckEdgeTriangle(t1.a, t1.b, t2) || CheckEdgeTriangle(t1.b, t1.c, t2) || CheckEdgeTriangle(t1.c, t1.a, t2) ||
                   CheckEdgeTriangle(t2.a, t2.b, t1) || CheckEdgeTriangle(t2.b, t2.c, t1) || CheckEdgeTriangle(t2.c, t2.a, t1);
        }

        bool CheckEdgeTriangle(Vector3 a, Vector3 b, Triangle t)
        {
            // Проверка пересечения ребра (a,b) с треугольником t (упрощено)
            Plane p = new Plane(t.b - t.a, t.a);
            float d1 = p.GetDistanceToPoint(a);
            float d2 = p.GetDistanceToPoint(b);
            if (d1 * d2 > 0) return false; // Не пересекает плоскость
                                           // Дополнительно: barycentric coords или moller-trumbore
            return true; // Заглушка, реализуйте полную логику
        }
    }
}
