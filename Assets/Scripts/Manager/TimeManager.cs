using System;
using UnityEngine;

class TimeManager : IResetable
{
    private double _msCurrentTime = 0d;

    public float GetNormalizedTime(IPeriod model)
    {
        _msCurrentTime += Time.deltaTime * TimeSpan.FromSeconds(1).TotalMilliseconds;

        // учитываем что замедлили время 
        double normalizedTime = _msCurrentTime / (model.Duration.TotalMilliseconds / (1d / Time.timeScale));

        return (float)normalizedTime;
    }

    public float GetNormalizedTimeOfDay(float normalizedTime, TimeSpan a, TimeSpan b)
    {
        return (float)(Lerp(a, b, normalizedTime).TotalSeconds / TimeSpan.FromDays(1).TotalSeconds);
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

    public void Reset(object o = null)
    {
        _msCurrentTime = 0d;
    }

    public double CurrentTime => _msCurrentTime;
}
