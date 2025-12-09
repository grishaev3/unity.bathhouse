using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;

abstract class CameraBase
{
    public int Index { get; set; }

    public string Name { get; set; }

    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Default
    /// </summary>
    public Bounds Bounds { get; set; } = new Bounds()
    {
        min = new Vector3 { x = 8f, y = 0.25f, z = 6f },
        max = new Vector3 { x = 8f, y = 8.0f, z = 6f }
    };

    public Func<float, CameraBase, Vector3> Func { get; set; }

    public abstract void Reset();
}

class SphereModel : CameraBase
{
    public float Radius { get; set; } = 6f;

    public override void Reset() { }
}

class LinearBase : CameraBase
{
    public LinearBase()
    {
        Reset();
    }

    public Vector3 A { get; set; }

    public Vector3 B { get; set; }

    public override void Reset()
    {
    }
}

class LinearRandom : LinearBase
{
    public LinearRandom()
    {
        base.Reset();
    }

    public override void Reset()
    {

        A = Vector3Extender.Random(Bounds);

        B = Vector3Extender.Random(Bounds);
    }
}

public class CameraBehaviour : MonoBehaviour
{
    private double _msCurrentTime = 0d;

    private readonly Vector3 _range = new(6f, 7f, 6f);
    private readonly Vector3 _center = new(0, 2f, 0);

    private List<CameraBase> _modes = null;
    private UniqueRandom _uniqueRandom = null;

    private int _currentMode;

    void Awake()
    {
        _modes = new()
        {
            //new SphereModel { Duration = TimeSpan.FromSeconds(20), Func = Sphere, Name = "Sphere"},
            new LinearRandom { Duration = TimeSpan.FromSeconds(10), Func = Linear, Name = "Random"},
#region
            new LinearBase { Duration = TimeSpan.FromSeconds(12), A = new Vector3(-_range.x, 2f, +_range.z), B = new Vector3(+_range.x, 2f, +_range.z), Func = Linear, Name = "LeftToRight" },
            new LinearBase { Duration = TimeSpan.FromSeconds(12), A = new Vector3(-_range.x, 2f, -_range.z), B = new Vector3(+_range.x, 2f, -_range.z), Func = Linear, Name = "LeftToRight" },

            new LinearBase { Duration = TimeSpan.FromSeconds(12), A = new Vector3(+_range.x, 2f, _range.z), B = new Vector3(-_range.x, 2f, _range.z), Func = Linear, Name = "RightToLeft" },
            new LinearBase { Duration = TimeSpan.FromSeconds(12), A = new Vector3(+_range.x, 2f, -_range.z), B = new Vector3(-_range.x, 2f, -_range.z), Func = Linear, Name = "RightToLeft" },
#endregion

#region
            new LinearBase { Duration = TimeSpan.FromSeconds(14), A = new Vector3(-_range.x, 0.25f, _range.z), B = new Vector3(-_range.x, _range.y, _range.z), Func = Linear, Name = "DownToUp" },
            new LinearBase { Duration = TimeSpan.FromSeconds(14), A = new Vector3(+_range.x, 0.25f, _range.z), B = new Vector3(+_range.x, _range.y, _range.z), Func = Linear, Name = "DownToUp" },

            new LinearBase { Duration = TimeSpan.FromSeconds(14), A = new Vector3(-_range.x, _range.y, -_range.z), B = new Vector3(-_range.x, 0.25f, -_range.z), Func = Linear, Name = "UpToDown" },
            new LinearBase { Duration = TimeSpan.FromSeconds(14), A = new Vector3(+_range.x, _range.y, -_range.z), B = new Vector3(+_range.x, 0.25f, -_range.z), Func = Linear, Name = "UpToDown" },
#endregion
        };

        for (int i = 0; i < _modes.Count; i++)
        {
            _modes[i].Index = i;
        }

        _uniqueRandom = new(0, _modes.Count);

        SelectUniqueModeAndLog();
    }

    void LateUpdate()
    {
        if (_modes == null)
        {
            return;
        }

        CameraBase model = _modes.Find(x => x.Index == _currentMode);
        float normalizedTime = GetNormalizedTime(model);
        Vector3 position = model.Func(normalizedTime, model);

        transform.position = position;
        transform.LookAt(_center);

        CheckAndReset(normalizedTime);
    }

    private void CheckAndReset(float normalizedTime)
    {
        if (normalizedTime < 0.999f)
        {
            return;
        }

        _msCurrentTime = 0d;

        SelectUniqueModeAndLog();

        _modes.ForEach(x =>
        {
            x.Reset();
        });
    }

    private float GetNormalizedTime(CameraBase model)
    {
        _msCurrentTime += Time.deltaTime * TimeSpan.FromSeconds(1).TotalMilliseconds;

        // учитываем что замедлили время 
        double normalizedTime = _msCurrentTime / (model.Duration.TotalMilliseconds / (1d / Time.timeScale));

        return (float)normalizedTime;
    }

    private Vector3 Sphere(float normalizedTime, CameraBase model)
    {
        float radius = (model as SphereModel).Radius;

        // Угол в радианах для кругового движения
        float angle = normalizedTime * Mathf.PI;

        // Позиция на полусфере (Y >= 0): сферические координаты
        float x = radius * Mathf.Sin(angle);
        float y = model.Bounds.min.y + (radius * Mathf.Abs(Mathf.Sin(angle))); // Полусфера (верхняя)
        float z = radius * Mathf.Cos(angle);

        return new Vector3(x, y, z);
    }

    private Vector3 Linear(float normalizedTime, CameraBase model)
    {
        LinearBase linearBase = (model as LinearBase);

        return Vector3.Lerp(linearBase.A, linearBase.B, normalizedTime);
    }

    private void SelectUniqueModeAndLog()
    {
        _currentMode = _uniqueRandom.Next();
        CameraBase model = _modes.Find(x => x.Index == _currentMode);
        Debug.Log($"_currentMode: {model.Name}-{model.Index}");
    }
}
