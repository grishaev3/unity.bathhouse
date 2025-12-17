using System;
using UnityEngine;

public class SunBehaviour : MonoBehaviour
{
    private Light _sunLight;
    private Light _areaLight;

    private TimeManager _timeManager = new();

    private IPeriod _period = new Period { Duration = TimeSpan.FromSeconds(10) };

    private Vector2 _altitude;
    private Vector2 _azimuth;

    private (TimeSpan A, TimeSpan B) _currentTimeRange;

    private int _currentHour = 0;

    private readonly (float timeOfDay, float intensity)[] intensity =
    {
        (0.00f, 0.5f),      // moon
        (0.25f, 5000f),     // low sun
        (0.54f, 100000f),   // 13:00 hight sun
        (0.75f, 5000f),     // low sun
        (1.00f, 0.5f)       // moon
    };

    (TimeSpan timeOfDay, float azimuth, float altitude)[] _solarData = new[]
    {
        (new TimeSpan(00, 00, 0), 352.84f, -10.61f),
        (new TimeSpan(01, 00, 0), 6.83f, -10.64f),
        (new TimeSpan(02, 00, 0), 20.63f, -8.64f),
        (new TimeSpan(03, 00, 0), 33.92f, -4.65f),
        (new TimeSpan(04, 00, 0), 46.56f, +1.14f),
        (new TimeSpan(05, 00, 0), 58.61f, +7.56f),
        (new TimeSpan(06, 00, 0), 70.31f, +15.11f),
        (new TimeSpan(07, 00, 0), 82.03f, +23.27f),
        (new TimeSpan(08, 00, 0), 94.31f, +31.67f),
        (new TimeSpan(09, 00, 0), 107.9f, 39.93f),
        (new TimeSpan(10, 00, 0), 123.85f, 47.51f),
        (new TimeSpan(11, 00, 0), 143.34f, 53.63f),
        (new TimeSpan(12, 00, 0), 166.88f, 57.19f),
        (new TimeSpan(13, 00, 0), 192.42f, 57.24f),
        (new TimeSpan(14, 00, 0), 216.05f, 53.76f),
        (new TimeSpan(15, 00, 0), 235.66f, 47.71f),
        (new TimeSpan(16, 00, 0), 251.69f, 40.15f),
        (new TimeSpan(17, 00, 0), 265.33f, 31.91f),
        (new TimeSpan(18, 00, 0), 277.63f, 23.50f),
        (new TimeSpan(19, 00, 0), 289.36f, 15.33f),
        (new TimeSpan(20, 00, 0), 301.05f, +7.76f),
        (new TimeSpan(21, 00, 0), 313.09f, +1.29f),
        (new TimeSpan(22, 00, 0), 352.72f, -4.50f),
        (new TimeSpan(23, 00, 0), 338.99f, -8.57f)
    };

    private void Awake()
    {
        _sunLight = GameObject.FindWithTag("Sun").GetComponent<Light>();
        _areaLight = GameObject.FindWithTag("AreaLight").GetComponent<Light>();

        InitFromCurrentHour(_currentHour);
    }

    void LateUpdate()
    {
        float normalizedTime = _timeManager.GetNormalizedTime(_period);

        float altitude = 0f, azimuth = 0f;
        altitude = Mathf.LerpAngle(_altitude.x, _altitude.y, normalizedTime);
        azimuth = Mathf.LerpAngle(_azimuth.x, _azimuth.y, normalizedTime);

        transform.rotation = Quaternion.Euler(altitude, azimuth, 0f);

        float normalizedTimeOfDay = (float)(
            Lerp(_currentTimeRange.A, _currentTimeRange.B, normalizedTime).TotalSeconds / 
            TimeSpan.FromDays(1).TotalSeconds);

        float intensity = GetIntensity(normalizedTimeOfDay);
        _sunLight.intensity = intensity;

        Debug.Log($"{(_currentHour % 24)} {normalizedTimeOfDay:F2} vec2({altitude:F2}, {azimuth:F2})");

        CheckAndReset(normalizedTime);
    }

    private void InitFromCurrentHour(int currentHour)
    {
        var A = Array.Find(_solarData, x => x.timeOfDay == new TimeSpan((currentHour % 24) + 0, 00, 00));
        var B = Array.Find(_solarData, x => x.timeOfDay == new TimeSpan((currentHour % 24) + 1, 00, 00));

        _currentTimeRange.A = A.timeOfDay;
        _currentTimeRange.B = B.timeOfDay;

        _altitude = new Vector2(A.altitude, B.altitude);
        _azimuth = new Vector2(A.azimuth, B.azimuth);
    }

    private void CheckAndReset(float normalizedTime)
    {
        if (normalizedTime < 0.999f)
        {
            return;
        }

        _currentHour += 1;

        _areaLight.enabled = (_currentHour < 4 || _currentHour > 22);

        InitFromCurrentHour(_currentHour);

        _timeManager.Reset();
    }

    public float GetIntensity(float timeOfDay)
    {
        if (timeOfDay <= 0f) return 0f;
        if (timeOfDay >= 1f) return 0f;

        for (int i = 0; i < intensity.Length - 1; i++)
        {
            var t1 = intensity[i].timeOfDay;
            var k1 = intensity[i].intensity;
            var t2 = intensity[i + 1].timeOfDay;
            var k2 = intensity[i + 1].intensity;

            if (timeOfDay >= t1 && timeOfDay <= t2)
            {
                float ratio = (timeOfDay - t1) / (t2 - t1);
                return Mathf.Lerp(k1, k2, ratio);
            }
        }
        return 0f; // Fallback
    }

    public static TimeSpan Lerp(TimeSpan start, TimeSpan end, float t)
    {
        // Преобразование TimeSpan в общее количество тиков (100-наносекундные интервалы)
        long startTicks = start.Ticks;
        long endTicks = end.Ticks;

        // Линейная интерполяция тиков
        long lerpedTicks = (long)(startTicks + (endTicks - startTicks) * t);

        // Возврат нового TimeSpan
        return new TimeSpan(lerpedTicks);
    }
}
