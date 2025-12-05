using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

abstract class CameraModel
{
    public int Index { get; set; }

    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Default
    /// </summary>
    public Bounds Bounds { get; set; } = new Bounds()
    {
        min = new Vector3 { x = 8f, y = 0.25f, z = 6f },
        max = new Vector3 { x = 8f, y = 8.0f, z = 6f }
    };

    public Func<float, CameraModel, Vector3> Func { get; set; }

    public abstract void Reset();
}

class SphereModel : CameraModel
{
    public float Radius { get; set; } = 6f;

    public override void Reset() { }
}

class LinearModel : CameraModel
{
    /// <summary>
    /// Высота камеры над центром
    /// </summary>
    public float Height { get; set; } = 2f;

    /// <summary>
    /// Расстояние от центра по Z
    /// </summary>
    public float Distance { get; set; } = 7f;

    public override void Reset() { }
}

class LinearRandom : CameraModel
{
    public Vector3 A { get; set; }

    public Vector3 B { get; set; }

    public LinearRandom()
    {
        Reset();
    }

    public override void Reset()
    {
        // только спереди
        Bounds = new Bounds()
        {
            min = new Vector3 { x = 6f, y = 0.25f, z = 0f },
            max = new Vector3 { x = 6f, y = 8.0f, z = 6f }
        };

        A = Vector3Extender.Random(Bounds);

        B = Vector3Extender.Random(Bounds);
    }
}

public class SphereCameraBehaviour : MonoBehaviour
{
    private double _msCurrentTime = 0d;

    private readonly Vector3 _center = new(0, 1.97f, 0);

    private List<CameraModel> _modes = null;

    private int _currentMode;

    void Awake()
    {
        _modes = new()
        {
            new SphereModel { Index = 0, Duration = TimeSpan.FromSeconds(20), Func = Sphere},

            new LinearModel { Index = 1, Duration = TimeSpan.FromSeconds(12), Func = LinearLeftToRight },
            new LinearModel { Index = 2, Duration = TimeSpan.FromSeconds(12), Func = LinearRightToLeft },

            new LinearModel { Index = 3, Duration = TimeSpan.FromSeconds(14), Func = LinearUpToDown },
            new LinearModel { Index = 4, Duration = TimeSpan.FromSeconds(14), Func = LinearDownToUp },

            new LinearRandom { Index = 5, Duration = TimeSpan.FromSeconds(10), Func = LinearFront }
        };

        _currentMode = 5;
    }

    void LateUpdate()
    {
        CameraModel model = _modes.FirstOrDefault(x => x.Index == _currentMode);
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

        _msCurrentTime = default;

        _currentMode = new UniqueRandom(0, _modes.Count).Next();

        Debug.Log("_currentMode: " + _currentMode);

        _modes.ForEach(x =>
        {
            x.Reset();
        });
    }

    private float GetNormalizedTime(CameraModel model)
    {
        _msCurrentTime += Time.deltaTime * TimeSpan.FromSeconds(1).TotalMilliseconds;

        // учитываем что замедлили время 
        double normalizedTime = _msCurrentTime / (model.Duration.TotalMilliseconds / (1d / Time.timeScale));

        return (float)normalizedTime;
    }

    private Vector3 Sphere(float normalizedTime, CameraModel model)
    {
        var radius = (model as SphereModel).Radius;

        // Угол в радианах для кругового движения
        float angle = normalizedTime * Mathf.PI;

        // Позиция на полусфере (Y >= 0): сферические координаты
        float x = radius * Mathf.Sin(angle);
        float y = model.Bounds.min.y + (radius * Mathf.Abs(Mathf.Sin(angle))); // Полусфера (верхняя)
        float z = radius * Mathf.Cos(angle);

        return new Vector3(x, y, z);
    }

    private Vector3 LinearLeftToRight(float normalizedTime, CameraModel model)
    {
        LinearModel linearModel = (model as LinearModel);
        float distance = linearModel.Distance;
        float height = linearModel.Height;

        float x = Mathf.Lerp(-distance, distance, normalizedTime);
        float y = height;
        float z = -distance;

        return new Vector3(x, y, z);
    }

    private Vector3 LinearRightToLeft(float normalizedTime, CameraModel model)
    {
        LinearModel linearModel = (model as LinearModel);
        float distance = linearModel.Distance;
        float height = linearModel.Height;

        float x = Mathf.Lerp(distance, -distance, normalizedTime);
        float y = height;
        float z = -distance;

        return new Vector3(x, y, z);
    }

    private Vector3 LinearUpToDown(float normalizedTime, CameraModel model)
    {
        float distance = (model as LinearModel).Distance;
        float height = (model as LinearModel).Height;

        float x = height;
        float y = Mathf.Lerp(distance, model.Bounds.max.y, normalizedTime);
        float z = -distance;

        return new Vector3(x, y, z);
    }

    private Vector3 LinearDownToUp(float normalizedTime, CameraModel model)
    {
        float distance = (model as LinearModel).Distance;
        float height = (model as LinearModel).Height;

        float x = height;
        float y = Mathf.Lerp(model.Bounds.min.y, distance, normalizedTime);
        float z = -distance;

        return new Vector3(x, y, z);
    }

    private Vector3 LinearFront(float normalizedTime, CameraModel model)
    {
        var a = (model as LinearRandom).A;
        var b = (model as LinearRandom).B;

        return Vector3.Lerp(a, b, normalizedTime);
    }
}
