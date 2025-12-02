using UnityEngine;

public class DynamicBehaviour : MonoBehaviour
{
    [Header("Настройки древесины")]
    public float woodDensity = 500f; // кг/м³ (сосна: 400-600)

    void Awake()
    {
        AddComponentsToChildren(transform);
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

            if (child.GetComponent<MeshCollider>() == null)
            {
                PhysicsMaterial physMat = new()
                {
                    dynamicFriction = 0.6f,
                    staticFriction = 0.7f,
                    bounciness = 0.1f,

                    frictionCombine = PhysicsMaterialCombine.Average,
                    bounceCombine = PhysicsMaterialCombine.Average
                };

                MeshCollider boxCollider = child.gameObject.AddComponent<MeshCollider>();
                boxCollider.convex = true;
                boxCollider.material = physMat;
            }

            Renderer renderer = child.GetComponent<Renderer>();
            if (child.GetComponent<Rigidbody>() == null && renderer != null)
            {
                Bounds bounds = renderer.bounds;
                float volume = bounds.size.x * bounds.size.y * bounds.size.z;
                float mass = woodDensity * volume;

                Rigidbody rigidbody = child.gameObject.AddComponent<Rigidbody>();
                rigidbody.mass = mass;
                rigidbody.solverIterations = 20;
                rigidbody.useGravity = true;
                rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                
            }

            
        }
    }
}
