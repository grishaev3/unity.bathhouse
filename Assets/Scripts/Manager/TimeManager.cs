using Assets.Scripts.Types;
using System;
using UnityEngine;

class TimeManager : IResetable
{
    internal enum SunCircle
    {
        Day,
        Night
    }

    private double _msCurrentTime = 0d;
    private double _normalizedTime = 0d;
    private int _currentHour;

    private Settings _settings = SettingsManager.Current;

    public float NormalizedTime => (float)_normalizedTime;

    public int CurrentHour => _currentHour;

    public TimeManager()
    {
        _currentHour = _settings.Timer.HourStart;
    }

    public void UpdateNormalizedTime(IPeriod model, out float normalizedTime)
    {
        _msCurrentTime += Time.deltaTime * TimeSpan.FromSeconds(1).TotalMilliseconds;

        // учитываем что замедлили время 
        _normalizedTime = _msCurrentTime / (model.Duration.TotalMilliseconds / (1d / Time.timeScale));

        normalizedTime = (float)_normalizedTime;
    }

    public float GetNormalizedTimeOfDay(float normalizedTime, TimeSpan a, TimeSpan b)
    {
        return (float)(Lerp(a, b, normalizedTime).TotalSeconds / TimeSpan.FromDays(1).TotalSeconds);
    }

    public SunCircle GetSunCircle(float altitude)
    {
        return _currentHour switch
        {
            >= 5 and <= 19 => SunCircle.Day,
            _ => SunCircle.Night
        };
    }

    public bool IsPeriodEnded(float normalizedTime)
    {
        if (normalizedTime < 0.999f)
        {
            return false;
        }

        if ((_currentHour + 1) >= _settings.Timer.HourEnd)
        {
            _currentHour = _settings.Timer.HourStart;
        }
        else
        {
            _currentHour += 1;
        }

        Reset();

        return true;
    }

    private static TimeSpan Lerp(TimeSpan start, TimeSpan end, float t)
    {
        // Преобразование TimeSpan в общее количество тиков (100-наносекундные интервалы)
        long startTicks = start.Ticks;
        long endTicks = end.Ticks;

        // Линейная интерполяция тиков
        long lerpedTicks = (long)(startTicks + (endTicks - startTicks) * t);

        // Возврат нового TimeSpan
        return new TimeSpan(lerpedTicks);
    }

    public void Reset(object o = null)
    {
        _msCurrentTime = 0d;
    }
}
