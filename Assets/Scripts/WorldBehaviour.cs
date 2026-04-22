using Assets.Scripts.Types;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

[DefaultExecutionOrder(-1000)]
public class WorldBehaviour : MonoBehaviour
{
    [Inject] private readonly Settings _settings;

    private List<float> _fpsBuffer = new();

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

        //MeshRenderer[] renderers = FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        //foreach (MeshRenderer renderer in renderers)
        //{
        //    // Cast Shadows: On
        //    renderer.shadowCastingMode = ShadowCastingMode.On;

        //    // Ray Tracing Mode: Static
        //    renderer.rayTracingMode = RayTracingMode.Static;


        //    // Motion Vectors: Camera Motion Only
        //    renderer.motionVectorGenerationMode = MotionVectorGenerationMode.Camera;

        //    // Dynamic Occlusion: true
        //    renderer.allowOcclusionWhenDynamic = true;
        //}

        NumberProviderFactory.Configure(_settings.IsBenchmarking);
        if (_settings.IsBenchmarking)
        {
            StartCoroutine(TimerCoroutine());
        }
    }

    void Update()
    {
        if (_settings.IsBenchmarking)
        {
            // Считаем текущий кадр (1 / время кадра)
            float currentFPS = 1.0f / Time.unscaledDeltaTime;
            _fpsBuffer.Add(currentFPS);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(ResetWithFadeCoroutine());
        }
    }

    private IEnumerator ResetWithFadeCoroutine()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        yield return null;
    }

    IEnumerator TimerCoroutine()
    {
        yield return new WaitForSecondsRealtime(60f);

        _settings.IsBenchmarking = false;

        SaveResultAndExit();
    }

    void SaveResultAndExit()
    {
        float averageFPS = _fpsBuffer.Count > 0 ? _fpsBuffer.Average() : 0f;

        string path = Path.Combine(Directory.GetCurrentDirectory(), "fps_report.txt");

        string report = $"\nAverage FPS over 1 minute: {averageFPS:F2}\n" +
                        $"Total frames: {_fpsBuffer.Count}\n" +
                        $"Date: {System.DateTime.Now}";

        File.AppendAllText(path, report);

#if UNITY_EDITOR
        Debug.Log($"Отчет сохранен: {path}");

        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif

    }
}
