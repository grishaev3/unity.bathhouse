using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Types;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Zenject;

[DefaultExecutionOrder(-1000)]
public class WorldBehaviour : MonoBehaviour
{
    [Inject] private readonly Settings _settings;

    private List<float> fpsBuffer = new List<float>();
    private bool isRecording = true;

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

        StartCoroutine(TimerCoroutine());
    }

    void Update()
    {
        if (isRecording)
        {
            // Считаем текущий кадр (1 / время кадра)
            float currentFPS = 1.0f / Time.unscaledDeltaTime;
            fpsBuffer.Add(currentFPS);
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

        isRecording = false;

        SaveResultAndExit();
    }

    void SaveResultAndExit()
    {
        float averageFPS = fpsBuffer.Count > 0 ? fpsBuffer.Average() : 0f;

        string path = Path.Combine(Directory.GetCurrentDirectory(), "fps_report.txt");

        string report = $"\nAverage FPS over 1 minute: {averageFPS:F2}\n" +
                        $"Total frames: {fpsBuffer.Count}\n" +
                        $"Date: {System.DateTime.Now}";

        File.AppendAllText(path, report);

        Debug.Log($"Отчет сохранен: {path}");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
