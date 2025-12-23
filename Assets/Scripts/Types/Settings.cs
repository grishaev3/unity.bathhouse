using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Types
{
    internal enum VendorQLevel
    {
        Low,
        Medium,
        Hight
    }

    internal class Physics
    {
        public bool EnableSimulation = false;
        public float SleepThreshold => 0.005f;
        public int DefaultSolverIterations => 20;
        public int DefaultSolverVelocityIterations => 8;

        public float DynamicFriction => 0.7f;
        public float StaticFriction => 0.8f;
        public float Bounciness => 0.05f;
    }

    internal class Timer
    {
        public int HourStart = 0;
        public int HourEnd = 23;

        public TimeSpan CameraModelDuration => TimeSpan.FromSeconds(20);
        public TimeSpan SunPeriodDuration => TimeSpan.FromSeconds(3);
    }

    class Settings
    {
        public Settings() { }

        public int TargetFPS { get; set; }
        public int SyncCount { get; set; }

        public Physics Physics { get; } = new Physics();

        public Timer Timer { get; } = new Timer();

        public string VolumeName => "Глобальный обём";
    }

    class SettingsManager
    {
        private static VendorQLevel _current = DetectVendor();

        /// <summary>
        ///  TODO
        /// </summary>
        /// <returns></returns>
        public static VendorQLevel DetectVendor()
        {
            var vendor = SystemInfo.graphicsDeviceName;
            if (vendor.Contains("AMD") || vendor.Contains("ATI"))
            {
                return VendorQLevel.Low;
            }
            else if (vendor.Contains("3060"))
            {
                return VendorQLevel.Medium;
            }
            else
            {
                return VendorQLevel.Hight;
            }
        }

        private static readonly Lazy<Dictionary<VendorQLevel, Settings>> _settings
            = new(() => new Dictionary<VendorQLevel, Settings>
            {
                [VendorQLevel.Low] = new Settings
                {
                    TargetFPS = 40,
                    SyncCount = 1
                },
                [VendorQLevel.Medium] = new Settings
                {
                    TargetFPS = 60,
                    SyncCount = 0
                },
                [VendorQLevel.Hight] = new Settings
                {
                    TargetFPS = 100,
                    SyncCount = 0
                }
            });

        public static Settings Current =>
            _settings.Value.TryGetValue(_current, out var settings) ? settings : throw new ArgumentException($"Неизвестный VendorType");
    }
}