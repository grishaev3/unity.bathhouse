using System.Linq;
using UnityEngine;

struct MeshData
{
    public Transform transform;
    public Vector3[] vertices;
    public int[] triangles;

    public MeshData(Transform t, Mesh m)
    {
        transform = t;
        vertices = m.vertices.Select(v => t.TransformPoint(v)).ToArray();
        triangles = m.triangles;
    }
}
