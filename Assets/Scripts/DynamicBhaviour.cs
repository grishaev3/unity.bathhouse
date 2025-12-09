using System.Collections.Generic;
using Assets;
using UnityEngine;

public class DynamicBehaviour : MonoBehaviour
{
    [Header("Настройки древесины")]
    public float woodDensity = 500f; // кг/м³ (сосна: 400-600)

    [Header("Solver Iterations")]
    public int SolverIterations = 20;

    [Header("Solver Velocity Iterations")]
    public int SolverVelocityIterations = 8;


    [Header("Debug Info")]
    public bool debug = false;

    private PhysicsMaterial _physicsMaterial = null;
    private readonly Dictionary<string, List<Transform>> _joinOn = new();
    private bool IsMesh(Transform child) => child.name.ToLowerInvariant().Contains("mesh");

    void Awake()
    {
        _physicsMaterial = CreateDefault();

        if (debug)
        {
            Dictionary<string, List<Vector3>> info = GetChildPositions(transform);
            IO.CollectSceneInfo(info);
        }

        CollectJoinsOn(transform);

        AddComponentsToChildren(transform);

        AttachJoinsFor(transform);
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

            var parentName = GetParentGroupName(child, 1);
            if (!parentName.Contains("joinon"))
            {
                continue;
            }

            string parentGroupName = GetParentGroupName(child, int.MaxValue);
            string key = GetKey("joinon", parentName, parentGroupName);

            if (!_joinOn.TryGetValue(key, out List<Transform> list))
            {
                list = new List<Transform>();
                _joinOn[key] = list;
            }

            list.Add(child);
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

            float mass = 0f;
            Vector3 size = Vector3.zero;
            MeshFilter meshFilter = child.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                Mesh yourMesh = meshFilter.sharedMesh;
                MeshCollider сollider = child.gameObject.AddComponent<MeshCollider>();
                сollider.sharedMesh = yourMesh;
                сollider.material = _physicsMaterial;
                сollider.convex = true;

                size = сollider.bounds.size;
                mass = (woodDensity * size.x * size.y * size.z);
            }

            Renderer renderer = child.GetComponent<Renderer>();
            if (child.GetComponent<Rigidbody>() == null && renderer != null)
            {
                Rigidbody rigidbody = child.gameObject.AddComponent<Rigidbody>();

                //rigidbody.mass = mass;
                rigidbody.useGravity = true;
                rigidbody.solverIterations = SolverIterations;
                rigidbody.solverVelocityIterations = SolverVelocityIterations;
                rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                //rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            }
        }
    }

    private void AttachJoinsFor(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (!IsMesh(child))
            {
                AttachJoinsFor(child);
                continue;
            }

            string parentName = GetParentGroupName(child, 1);
            if (parentName.Contains("joinfor"))
            {
                string parentGroupName = GetParentGroupName(child, int.MaxValue);
                string key = GetKey("joinfor", parentName, parentGroupName);
                Transform[] transforms = _joinOn[key].ToArray();
                foreach (Transform target in transforms)
                {
                    FixedJoint joint = child.gameObject.AddComponent<FixedJoint>();

                    Rigidbody connectedBody = target.GetComponent<Rigidbody>();
                    Debug.Assert(connectedBody != null);

                    joint.connectedBody = connectedBody;
                    joint.breakForce = Mathf.Infinity;
                    joint.breakTorque = Mathf.Infinity;
                    joint.enableCollision = false;
                }
            }
        }
    }

    private static string GetKey(string prefix, string parentName, string parentGroupName)
    {
        switch (parentName.Length)
        {
            // JoinOn3
            case 7:
                break;
            // joinOn1A
            case 8:
                break;
            // joinOn1A1
            default:
                parentName = parentName[..8];
                break;

        }

        string index = parentName.Replace(prefix, "");
        if (char.IsLetter(index[^1]))
        {
            index = index[..^1];
        }

        return $"{parentGroupName}:{index}";
    }

    private string GetParentGroupName(Transform current, int level)
    {
        if (level == 1)
        {
            return current.parent.name.ToLowerInvariant();
        }

        if (current.name.ToLowerInvariant().Contains("group"))
        {
            return current.name.ToLowerInvariant();
        }

        return GetParentGroupName(current.parent, int.MaxValue);
    }

    private static PhysicsMaterial CreateDefault()
    {
        return new()
        {
            dynamicFriction = 0.7f,
            staticFriction = 0.8f,
            bounciness = 0.05f,

            frictionCombine = PhysicsMaterialCombine.Average,
            bounceCombine = PhysicsMaterialCombine.Average
        };
    }

    private Dictionary<string, List<Vector3>> GetChildPositions(Transform transform)
    {
        var childTransforms = transform.GetComponentsInChildren<Transform>();

        var result = new Dictionary<string, List<Vector3>>();

        foreach (Transform child in childTransforms)
        {
            if (child == transform || !IsMesh(child)) // Исключаем родителя
            {
                continue;
            }

            MeshFilter meshFilter = child.GetComponent<MeshFilter>();
            if (meshFilter == null || meshFilter.mesh == null)
            {
                Debug.LogError("MeshFilter или mesh не найден!");
                continue;
            }

            if (!result.TryGetValue(child.name, out List<Vector3> list))
            {
                list = new List<Vector3>();
                result[child.name] = list;
            }

            list.AddRange(meshFilter.mesh.vertices);
        }

        return result;
    }
}
