using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class DynamicBehaviour : MonoBehaviour
{
    [Header("Настройки древесины")]
    public float woodDensity = 500f; // кг/м³ (сосна: 400-600)

    [Header("Solver Iterations")]
    public int SolverIterations = 20;

    private const int _inMM = 1000;

    private PhysicsMaterial _physicsMaterial = null;
    private readonly SortedDictionary<float, Geom> _info = new();


    private readonly Dictionary<string, Transform> _joinOn = new();


    private bool IsMesh(Transform child) => child.name.ToLowerInvariant().Contains("mesh");

    void Awake()
    {
        CollectJoinsOn(transform);

        AddComponentsToChildren(transform);

        _physicsMaterial = CreateDefault();

        CollectInfo();
    }

    private void CollectJoinsOn(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (!IsMesh(child))
            {
                CollectJoinsOn(child);
                continue;
            }

            var parentName = child.parent.name.ToLowerInvariant();
            if (parentName.Contains("joinon"))
            {
                var index = parentName.Replace("joinon", "");
                _joinOn[index] = child;
            }
        }
    }

    private void AddComponentsToChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (!IsMesh(child))
            {
                AddComponentsToChildren(child);
                continue;
            }

            MeshCollider сollider = child.gameObject.AddComponent<MeshCollider>();
            сollider.sharedMesh = child.GetComponent<MeshFilter>().sharedMesh;
            сollider.material = _physicsMaterial;
            сollider.convex = true;

            Vector3 size = сollider.bounds.size;
            float mass = woodDensity * size.x * size.y * size.z;

            Renderer renderer = child.GetComponent<Renderer>();
            if (child.GetComponent<Rigidbody>() == null && renderer != null)
            {
                Rigidbody rigidbody = child.gameObject.AddComponent<Rigidbody>();

                rigidbody.mass = mass;
                rigidbody.useGravity = true;
                rigidbody.solverIterations = SolverIterations;
                rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

                _info[mass] = new Geom(parent.name, size);
            }

            var parentName = child.parent.name.ToLowerInvariant();
            if (parentName.Contains("jointo"))
            {
                var index = parentName.Replace("jointo", "");
                FixedJoint joint = child.gameObject.AddComponent<FixedJoint>();

                Transform defaultTransform = _joinOn[index];

                joint.connectedBody = defaultTransform.GetComponent<Rigidbody>();
                joint.breakForce = 10000f;              // Сила разрыва (0 = бесконечно)
                joint.breakTorque = 10000f;             // Момент разрыва
            }
        }
    }

    private static PhysicsMaterial CreateDefault()
    {
        return new()
        {
            dynamicFriction = 0.6f,
            staticFriction = 0.7f,
            bounciness = 0.1f,

            frictionCombine = PhysicsMaterialCombine.Average,
            bounceCombine = PhysicsMaterialCombine.Average
        };
    }

    private void CollectInfo()
    {
        StringBuilder builder = new();
        foreach (float key in _info.Keys)
        {
            int x = (int)Math.Floor(_info[key].Size.x * _inMM);
            int y = (int)Math.Floor(_info[key].Size.y * _inMM);
            int z = (int)Math.Floor(_info[key].Size.z * _inMM);

            builder.AppendLine($"{key} : {_info[key].Name} - ( {x} {y} {z})");
        }

        File.WriteAllText("info.txt", builder.ToString());
    }
}
