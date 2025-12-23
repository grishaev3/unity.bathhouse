using Assets.Scripts.Types;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldBehaviour : MonoBehaviour
{
    private Settings _settings = SettingsManager.Current;

    void Start()
    {
        QualitySettings.vSyncCount = _settings.SyncCount;
        Application.targetFrameRate = _settings.TargetFPS;

        Time.timeScale = 0.2f;

        // Fixed Timestep 0.02 => 0.01
        Time.fixedDeltaTime = 1 / 100f;

        UnityEngine.Physics.sleepThreshold = _settings.Physics.SleepThreshold;
        UnityEngine.Physics.defaultSolverIterations = _settings.Physics.DefaultSolverIterations;
        UnityEngine.Physics.defaultSolverVelocityIterations = _settings.Physics.DefaultSolverVelocityIterations;
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
