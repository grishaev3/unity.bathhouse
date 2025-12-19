using System;

class Settings
{
    public int TargetFPS => 100;
    public int SyncCount => 0;

    public float SleepThreshold => 0.005f;
    public int DefaultSolverIterations => 20;
    public int DefaultSolverVelocityIterations => 8;

    public int HourStart = 4;
    public int HourEnd = 10;

    public TimeSpan CameraModelDuration => TimeSpan.FromSeconds(20);
    public TimeSpan SunPeriodDuration => TimeSpan.FromSeconds(3);

    public string VolumeName => "Глобальный обём";
}
