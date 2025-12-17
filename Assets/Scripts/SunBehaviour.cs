using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using static UnityEngine.Rendering.HighDefinition.CameraSettings;

public class SunBehaviour : MonoBehaviour
{
    private Light _sunLight;
    private Light _areaLight;
    private Volume _volume;

    private TimeManager _timeManager = new();

    private int _currentHour = 0;
    private IPeriod _period = new Period { Duration = TimeSpan.FromSeconds(5) };

    private Vector2 _altitude;
    private Vector2 _azimuth;

    private (TimeSpan A, TimeSpan B) _currentTimeRange;

    (TimeSpan timeOfDay, float azimuth, float altitude)[] _solarData = new[]
    {
        (new TimeSpan(00, 00, 0), 352.84f, -10.61f),
        (new TimeSpan(01, 00, 0), 6.83f,   -10.64f),
        (new TimeSpan(02, 00, 0), 20.63f,  -8.64f),
        (new TimeSpan(03, 00, 0), 33.92f,  -4.65f),
        (new TimeSpan(04, 00, 0), 46.56f,  +1.14f),
        (new TimeSpan(05, 00, 0), 58.61f,  +7.56f),
        (new TimeSpan(06, 00, 0), 70.31f,  +15.11f),
        (new TimeSpan(07, 00, 0), 82.03f,  +23.27f),
        (new TimeSpan(08, 00, 0), 94.31f,  +31.67f),
        (new TimeSpan(09, 00, 0), 107.9f,  +39.93f),
        (new TimeSpan(10, 00, 0), 123.85f, +47.51f),
        (new TimeSpan(11, 00, 0), 143.34f, +53.63f),
        (new TimeSpan(12, 00, 0), 166.88f, +57.19f),
        (new TimeSpan(13, 00, 0), 192.42f, +57.24f),
        (new TimeSpan(14, 00, 0), 216.05f, +53.76f),
        (new TimeSpan(15, 00, 0), 235.66f, +47.71f),
        (new TimeSpan(16, 00, 0), 251.69f, +40.15f),
        (new TimeSpan(17, 00, 0), 265.33f, +31.91f),
        (new TimeSpan(18, 00, 0), 277.63f, +23.50f),
        (new TimeSpan(19, 00, 0), 289.36f, +15.33f),
        (new TimeSpan(20, 00, 0), 301.05f, +7.76f),
        (new TimeSpan(21, 00, 0), 313.09f, +1.29f),
        (new TimeSpan(22, 00, 0), 352.72f, -4.50f),
        (new TimeSpan(23, 00, 0), 338.99f, -8.57f)
    };

    private void Awake()
    {
        _sunLight = GameObject.FindWithTag("Sun").GetComponent<Light>();
        _areaLight = GameObject.FindWithTag("AreaLight").GetComponent<Light>();
        _volume = GameObject.FindWithTag("Volume").GetComponent<Volume>();

        InitFromCurrentHour(_currentHour);
    }

    void LateUpdate()
    {
        float normalizedTime = _timeManager.GetNormalizedTime(_period);

        float altitude = CorrectAltitude(Mathf.LerpAngle(_altitude.x, _altitude.y, normalizedTime));

        float azimuth = CorrectAzimuth(Mathf.LerpAngle(_azimuth.x, _azimuth.y, normalizedTime));

        transform.rotation = Quaternion.Euler(altitude, azimuth, 0f);

        _sunLight.intensity = GetIntensity(altitude);

        float normalizedTimeOfDay = _timeManager.GetNormalizedTimeOfDay(normalizedTime, _currentTimeRange.A, _currentTimeRange.B);

        Debug.Log($"{_currentHour % 24} {normalizedTimeOfDay:F2} vec2({altitude:F2}, {azimuth:F2})");

        CheckAndReset(normalizedTime);
    }


    private void InitFromCurrentHour(int currentHour)
    {
        var a = Array.Find(_solarData, x => x.timeOfDay == new TimeSpan((currentHour % 24) + 0, 0, 0));
        var b = Array.Find(_solarData, x => x.timeOfDay == new TimeSpan((currentHour % 24) + 1, 0, 0));

        _currentTimeRange.A = a.timeOfDay;
        _currentTimeRange.B = b.timeOfDay;

        _altitude = new Vector2(a.altitude, b.altitude);
        _azimuth = new Vector2(a.azimuth, b.azimuth);
    }

    private void CheckAndReset(float normalizedTime)
    {
        if (normalizedTime < 0.999f)
        {
            return;
        }

        _currentHour += 1;

        if (_currentHour < 3 || _currentHour > 22)
        {
            // ночь
            if (_volume.profile.TryGet<Exposure>(out Exposure exposure))
            {
                exposure.mode.value = ExposureMode.Automatic;
            }
            _areaLight.enabled = true;
        }
        else
        {
            if (_volume.profile.TryGet<Exposure>(out Exposure exposure))
            {
                exposure.mode.value = ExposureMode.UsePhysicalCamera;
            }
            _areaLight.enabled = false;
        }

        InitFromCurrentHour(_currentHour);

        _timeManager.Reset();
    }

    public static float CorrectAltitude(float val)
    {
        return val;
    }

    public static float CorrectAzimuth(float val)
    {
        // левая стена обращена на север
        // баню нужно повернуть +90f
        // но чтобы не морочится с объёмами камерами делаем -90f
        return val - 90f;

    }

    public static float GetIntensity(float altitude)
    {
        // в зените 58 должно быть 32
        return altitude switch
        {
            < 0 => 0.5f,    // moon
            _ => 100000f    // hight sun
        };
    }
}
