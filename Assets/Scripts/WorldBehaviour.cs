using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldBehaviour : MonoBehaviour
{
    [Header("Настройки FPS")]
    public int targetFPS = 60;

    [Header("Настройки рестарта")]
    public float fadeTime = 1f;

    [Header("UI (опционально)")]
    public CanvasGroup fadePanel;

    void Start()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = targetFPS;

        Time.timeScale = 0.2f;

        // Fixed Timestep 0.02 => 0.01
        Time.fixedDeltaTime = 1 / 100f;

    //    // Default Contact Offset: 0.01 → 0.001 (точнее контакты)
    //    Physics.defaultContactOffset = 0.001f;

    //    // Default Solver Iterations 6 => 20
    //    Physics.defaultSolverIterations = 20;

    //    // Default Solver Velocity Iterations 1 => 8
    //    Physics.defaultSolverVelocityIterations = 8;
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
        if (fadePanel != null)
        {
            // Fade out
            float elapsed = 0f;
            while (elapsed < fadeTime)
            {
                elapsed += Time.unscaledDeltaTime;
                fadePanel.alpha = Mathf.Clamp01(elapsed / fadeTime);
                yield return null;
            }
        }

        // Жесткий сброс
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
