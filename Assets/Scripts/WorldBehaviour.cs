using UnityEngine;

public class WorldBehaviour : MonoBehaviour
{
    [Header("Настройки FPS")]
    public int targetFPS = 60;

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
}
