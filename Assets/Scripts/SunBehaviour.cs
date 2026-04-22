using System;
using Assets.Scripts.Types;
using UnityEngine;
using Zenject;

public class SunBehaviour : MonoBehaviour
{
    private Light _sunLight;
    private Light _areaLight;
    private UnityEngine.Camera _camera;

    [Inject] private readonly TimeManager _timeManager;
    [Inject] private readonly Settings _settings;
    private SolarData[] _solarData;

    private Vector2 _altitude;
    private Vector2 _azimuth;
    private IPeriod _period;

    // lux рассчитан как 35000 * sin(altitude) / sin(57.24°), симметрично по высоте солнца
    private readonly float[] sunIntensityTable = new float[] {
        0,     0,     0,     0,     800,   5500,  // 00-05
        11000, 16500, 22000, 27000, 31000, 34000, // 06-11
        35000, 35000, 34000, 31000, 27000, 22000, // 12-17
        17000, 11000, 5500,  1000,  0,     0      // 18-23
    };

    // ISO и диафрагма подобраны к lux-значениям, симметричны утро/вечер
    private readonly float[] isoTable = new float[] {
        12800, 12800, 12800, 12800, 3200,  800,   // 00-05
        400,   200,   100,   100,   100,   100,   // 06-11
        100,   100,   100,   100,   100,   100,   // 12-17
        200,   400,   800,   3200,  12800, 12800  // 18-23
    };

    private readonly float[] apertureTable = new float[] {
        2.8f, 2.8f, 2.8f, 2.8f, 3.2f, 4.5f,     // 00-05
        6.3f, 8.0f, 8.0f, 8.0f, 8.0f, 8.0f,     // 06-11
        8.0f, 8.0f, 8.0f, 8.0f, 8.0f, 8.0f,     // 12-17
        8.0f, 6.3f, 4.5f, 3.2f, 2.8f, 2.8f      // 18-23
    };


    private (TimeSpan A, TimeSpan B) _currentTimeRange;

    void Start()
    {
        var sunPeriod = new Period { Duration = _settings.Timer.SunPeriodDuration };
        var nightPeriod = new Period { Duration = _settings.Timer.NightPeriodDuration };
        var dawnPeriod = new Period { Duration = _settings.Timer.DawnSunPeriodDuration };
        var duskSetPeriod = new Period { Duration = _settings.Timer.DawnSunPeriodDuration };

        _solarData = new SolarData[]
       {
            new(new TimeSpan(00, 00, 0), 352.84f, -10.61f, nightPeriod),
            new(new TimeSpan(01, 00, 0), 6.83f,   -10.64f, nightPeriod),
            new(new TimeSpan(02, 00, 0), 20.63f,  -8.64f, nightPeriod),
            new(new TimeSpan(03, 00, 0), 33.92f,  -4.65f, dawnPeriod),
            new(new TimeSpan(04, 00, 0), 46.56f,  +1.14f, dawnPeriod),
            new(new TimeSpan(05, 00, 0), 58.61f,  +7.56f, dawnPeriod),
            new(new TimeSpan(06, 00, 0), 70.31f,  +15.11f, sunPeriod),
            new(new TimeSpan(07, 00, 0), 82.03f,  +23.27f, sunPeriod),
            new(new TimeSpan(08, 00, 0), 94.31f,  +31.67f, sunPeriod),
            new(new TimeSpan(09, 00, 0), 107.9f,  +39.93f, sunPeriod),
            new(new TimeSpan(10, 00, 0), 123.85f, +47.51f, sunPeriod),
            new(new TimeSpan(11, 00, 0), 143.34f, +53.63f, sunPeriod),
            new(new TimeSpan(12, 00, 0), 166.88f, +57.19f, sunPeriod),
            new(new TimeSpan(13, 00, 0), 192.42f, +57.24f, sunPeriod),
            new(new TimeSpan(14, 00, 0), 216.05f, +53.76f, sunPeriod),
            new(new TimeSpan(15, 00, 0), 235.66f, +47.71f, sunPeriod),
            new(new TimeSpan(16, 00, 0), 251.69f, +40.15f, sunPeriod),
            new(new TimeSpan(17, 00, 0), 265.33f, +31.91f, sunPeriod),
            new(new TimeSpan(18, 00, 0), 277.63f, +23.50f, sunPeriod),
            new(new TimeSpan(19, 00, 0), 289.36f, +15.33f, duskSetPeriod),
            new(new TimeSpan(20, 00, 0), 301.05f, +7.76f, duskSetPeriod),
            new(new TimeSpan(21, 00, 0), 313.09f, +1.29f, duskSetPeriod),
            new(new TimeSpan(22, 00, 0), 352.72f, -4.50f, nightPeriod),
            new(new TimeSpan(23, 00, 0), 338.99f, -8.57f, nightPeriod)
       };

        _sunLight = GetComponent<Light>("Sun");
        _areaLight = GetComponent<Light>("AreaLight");
        _camera = GetComponent<UnityEngine.Camera>("MainCamera");

        InitFromCurrentHour(_timeManager.CurrentHour);
    }

    void FixedUpdate()
    {
        _timeManager.UpdateNormalizedTime(_period, out float normalizedTime);

        float normalizedTimeOfDay = _timeManager.GetNormalizedTimeOfDay(normalizedTime, _currentTimeRange.A, _currentTimeRange.B);
        float altitude = CorrectAltitude(Mathf.LerpAngle(_altitude.x, _altitude.y, normalizedTime));
        float azimuth = CorrectAzimuth(Mathf.LerpAngle(_azimuth.x, _azimuth.y, normalizedTime));
        transform.rotation = Quaternion.Euler(altitude, azimuth, 0f);

        if (_timeManager.OnPeriodEnd(normalizedTime))
        {
            InitFromCurrentHour(_timeManager.CurrentHour);

            SwitchLighting();
        }

        int hourIndex = Mathf.FloorToInt(_timeManager.CurrentHour);
        int nextHourIndex = (hourIndex + 1) % 24;

        // Физические параметры камеры (через актуальные свойства)
        int iso = Mathf.FloorToInt(Mathf.Lerp(isoTable[hourIndex], isoTable[nextHourIndex], normalizedTime));
        float aperture = Mathf.Lerp(apertureTable[hourIndex], apertureTable[nextHourIndex], normalizedTime);

        _camera.iso = iso;
        _camera.aperture = aperture;
        _camera.shutterSpeed = 0.01f; // Фиксируем 1/100

        // Интенсивность солнца (Lux)
        float intensity = Mathf.Lerp(sunIntensityTable[hourIndex], sunIntensityTable[nextHourIndex], normalizedTime);
        _sunLight.intensity = intensity;

        // Оптимизация теней (выключаем ночью для экономии ресурсов)
        _sunLight.shadows = (intensity > 0.1f) ? LightShadows.Soft : LightShadows.None;

        Debug.Log($"current_hour: {_timeManager.CurrentHour} iso: {iso} aperture: {aperture} intensity:{intensity}");
    }

    private void SwitchLighting()
    {
        //_volume.profile.TryGet(out Exposure exposure);
        //exposure.mode.value = ExposureMode.UsePhysicalCamera;
        //_sunLight.intensity = 100000f;

        switch (_timeManager.GetSunCircle())
        {
            case TimeManager.SunCircle.Night:
                _areaLight.enabled = true;
                break;

            case TimeManager.SunCircle.Day:
                _areaLight.enabled = false;
                break;
        }
    }

    private void InitFromCurrentHour(int currentHour)
    {
        var a = Array.Find(_solarData, x => x.timeOfDay == new TimeSpan((currentHour % 24) + 0, 0, 0));
        var b = Array.Find(_solarData, x => x.timeOfDay == new TimeSpan((currentHour + 1) % 24, 0, 0));

        _currentTimeRange.A = a.timeOfDay;
        _currentTimeRange.B = b.timeOfDay;

        _altitude = new Vector2(a.altitude, b.altitude);
        _azimuth = new Vector2(a.azimuth, b.azimuth);

        _period = a.period;
    }

    public static float CorrectAltitude(float altitude)
    {
        return altitude;
    }

    public static float CorrectAzimuth(float azimuth)
    {
        // левая стена обращена на север
        // баню нужно повернуть +90f
        // но чтобы не морочится с объёмами камерами делаем -90f
        return azimuth - 90f;
    }

    private T GetComponent<T>(string tag)
    {
        GameObject gameObject = GameObject.FindWithTag(tag);
        if (gameObject.TryGetComponent<T>(out T component))
        {
            return component;
        }

        return default;
    }
}
