using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldBehaviour : MonoBehaviour
{
    private Settings _settings = new();

    void Start()
    {
        QualitySettings.vSyncCount = _settings.SyncCount;
        Application.targetFrameRate = _settings.TargetFPS;

        Time.timeScale = 0.2f;

        // Fixed Timestep 0.02 => 0.01
        Time.fixedDeltaTime = 1 / 100f;

        Physics.sleepThreshold = _settings.SleepThreshold;
        Physics.defaultSolverIterations = _settings.DefaultSolverIterations;
        Physics.defaultSolverVelocityIterations = _settings.DefaultSolverVelocityIterations;
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
