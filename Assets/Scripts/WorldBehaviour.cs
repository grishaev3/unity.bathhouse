using UnityEngine;

public class WorldBehaviour : MonoBehaviour
{
    // Settings
    // Fixed Timestep 0.02 => 0.01
    // Default Solver Iterations 6 => 20
    // Default Solver Velocity Iterations 1 => 8
    // Default Contact Offset: 0.01 → 0.001 (точнее контакты)

    [Header("Настройки FPS")]
    public int targetFPS = 60;

    void Start()
    {
        // Отключаем VSync для полного контроля
        QualitySettings.vSyncCount = 0;

        // Ограничиваем FPS
        Application.targetFrameRate = targetFPS;

        Time.timeScale = 0.2f;
        Time.fixedDeltaTime = 0.01f * Time.timeScale;
    }
}
