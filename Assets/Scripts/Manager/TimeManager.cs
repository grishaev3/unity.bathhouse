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

    public void Reset(object o = null)
    {
        _msCurrentTime = 0d;
    }

    public double CurrentTime => _msCurrentTime;
}
