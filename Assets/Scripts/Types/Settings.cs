using System;
using System.Collections.Generic;

namespace Assets.Scripts.Types
{
    internal enum PresetLevel
    {
        Low,
        Medium,
        Hight
    }

    [Flags]
    internal enum CameraMode
    {
        None = 0,
        Static = 1,
        Dynamic = 2,
        All = Static | Dynamic
    }

    internal class Physics
    {
        public bool EnableSimulation => false;
        public float SleepThreshold => 0.005f;
        public int DefaultSolverIterations => 20;
        public int DefaultSolverVelocityIterations => 8;

        public float DynamicFriction => 0.7f;
        public float StaticFriction => 0.8f;
        public float Bounciness => 0.05f;
    }

    internal class Timer
    {
        public int HourStart = 4;
        public int HourEnd = 23;

        public TimeSpan CameraModelDuration => TimeSpan.FromSeconds(10);
        public TimeSpan NightPeriodDuration => TimeSpan.FromSeconds(3);
        public TimeSpan SunPeriodDuration => TimeSpan.FromSeconds(12);
        public TimeSpan DawnSunPeriodDuration => TimeSpan.FromSeconds(36);
        public TimeSpan DuskSunPeriodDuration => TimeSpan.FromSeconds(24);
        
    }

    internal class Camera
    {
        public CameraMode Mode = CameraMode.Dynamic;
    }

    class Settings
    {
        public int TargetFPS { get; set; }
        public int SyncCount { get; set; }

        public Physics Physics { get; } = new Physics();

        public Timer Timer { get; } = new Timer();

        public Camera Camera { get; } = new Camera();

        public string VolumeName => null;
    }

    class SettingsManager
    {
        private PresetLevel _current;

        public SettingsManager(PresetLevel presetLevel)
        {
            _current = presetLevel;
        }

        private static readonly Dictionary<PresetLevel, Settings> _settings
            = new()
            {
                [PresetLevel.Low] = new Settings
                {
                    TargetFPS = 40,
                    SyncCount = 0
                },
                [PresetLevel.Medium] = new Settings
                {
                    TargetFPS = 60,
                    SyncCount = 0
                },
                [PresetLevel.Hight] = new Settings
                {
                    TargetFPS = 100,
                    SyncCount = 0
                }
            };

        public Settings Current
        {
            get
            {
                if (_settings.TryGetValue(_current, out var settings) && settings != null)
                {
                    return settings;
                }
                else
                {
                    throw new ArgumentException($"Неизвестный VendorType");
                }
            }
        }
    }
}