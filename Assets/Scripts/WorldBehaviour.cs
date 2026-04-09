using System.Collections;
using Assets.Scripts.Types;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Zenject;

[DefaultExecutionOrder(-1000)]
public class WorldBehaviour : MonoBehaviour
{
    [Inject] private readonly Settings _settings;

    void Awake()
    {
        QualitySettings.vSyncCount = _settings.SyncCount;
        Application.targetFrameRate = _settings.TargetFPS;

        Time.timeScale = 0.2f;

        // Fixed Timestep 0.02 => 0.01
        Time.fixedDeltaTime = 1 / 100f;

        UnityEngine.Physics.sleepThreshold = _settings.Physics.SleepThreshold;
        UnityEngine.Physics.defaultSolverIterations = _settings.Physics.DefaultSolverIterations;
        UnityEngine.Physics.defaultSolverVelocityIterations = _settings.Physics.DefaultSolverVelocityIterations;

        MeshRenderer[] renderers = FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (MeshRenderer renderer in renderers)
        {
            // Cast Shadows: On
            renderer.shadowCastingMode = ShadowCastingMode.On;

            // Ray Tracing Mode: Static
            renderer.rayTracingMode = RayTracingMode.Static;


            // Motion Vectors: Camera Motion Only
            renderer.motionVectorGenerationMode = MotionVectorGenerationMode.Camera;

            // Dynamic Occlusion: true
            renderer.allowOcclusionWhenDynamic = true;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(ResetWithFade());
        }
    }

    private IEnumerator ResetWithFade()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        yield return null;
    }
}
