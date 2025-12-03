using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class DynamicBehaviour : MonoBehaviour
{
    [Header("Настройки древесины")]
    public float woodDensity = 500f; // кг/м³ (сосна: 400-600)

    [Header("Solver Iterations")]
    public int SolverIterations = 20;

    private PhysicsMaterial _physicsMaterial = null;

    private Dictionary<float, string> _info = new();

    void Awake()
    {
        AddComponentsToChildren(transform);

        _physicsMaterial = new()
        {
            dynamicFriction = 0.6f,
            staticFriction = 0.7f,
            bounciness = 0.1f,

            frictionCombine = PhysicsMaterialCombine.Average,
            bounceCombine = PhysicsMaterialCombine.Average
        };
    }

    private void AddComponentsToChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Contains("Group"))
            {
                AddComponentsToChildren(child);
                continue;
            }

            //if (child.name.Contains("187"))
            //{
            //    Debug.Break();
            //}


            float mass = 0f;
            var ss = child.GetComponent<MeshCollider>();
            if (ss == null)
            {

                MeshCollider сollider = child.gameObject.AddComponent<MeshCollider>();
                сollider.sharedMesh = child.GetComponent<MeshFilter>().sharedMesh;
                сollider.material = _physicsMaterial;
                сollider.convex = true;

                Vector3 size = сollider.bounds.size;
                float volume = size.x * size.y * size.z;
                mass = woodDensity * volume;
            }
            else
            {
                Vector3 size = ss.bounds.size;
                float volume = size.x * size.y * size.z;
                mass = woodDensity * volume;

            }

            Renderer renderer = child.GetComponent<Renderer>();
            if (child.GetComponent<Rigidbody>() == null && renderer != null)
            {
                Bounds bounds = renderer.bounds;
                float volume = bounds.size.x * bounds.size.y * bounds.size.z;

                _info[mass] = parent.name;

                Rigidbody rigidbody = child.gameObject.AddComponent<Rigidbody>();
                rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                rigidbody.mass = mass;
                rigidbody.solverIterations = 20;
                rigidbody.useGravity = true;
                rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            }
        }

        CollectInfo();
    }

    private void CollectInfo()
    {
        StringBuilder builder = new();
        foreach (float key in _info.Keys)
        {
            builder.AppendLine($"{key} : {_info[key]}");
        }

        File.WriteAllText("info.txt", builder.ToString());
    }
}
