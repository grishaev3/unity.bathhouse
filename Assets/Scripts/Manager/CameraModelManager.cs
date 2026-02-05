using Assets.Scripts;
using Assets.Scripts.Types;
using System;
using System.Collections.Generic;
using UnityEngine;

class CameraModelManager : IResetable<BoundParameters>
{
    private readonly Settings _settings;

    private int _currentModelIndex;
    private readonly UniqueRandom _uniqueRandom;
    private readonly List<CameraBase> _modes;

    public CameraModelManager(BoundParameters boundParameters, Settings settings)
    {
        _settings = settings;
        TimeSpan duration = _settings.Timer.CameraModelDuration;
        CameraDirectionType center = CameraDirectionType.Center;
        CameraDirectionType direct = CameraDirectionType.Center;

        _modes = new List<CameraBase>();

        if (_settings.Camera.Mode.HasFlag(CameraMode.Dynamic))
        {
            _modes.AddRange(new List<CameraBase>()
            {
                // смотрим сверху
                new LinearBase(duration, Linear, "Linear", boundParameters, center),
                new LinearRandom(duration, Linear, "Random", boundParameters, direct),
            });
        }

        if (_settings.Camera.Mode.HasFlag(CameraMode.Static))
        {
            _modes.AddRange(new List<CameraBase>() {
                new StaticCamera(duration, (_, _) => new Vector3(-0.63f, 1.97f, +6.00f), Linear, new Vector3(-5f, 1.97f, 0f), new Vector3(+5f, 1.97f, 0f), "Static0"),
                new StaticCamera(duration, (_, _) => new Vector3(-0.63f, 1.97f, +6.00f), Linear, new Vector3(+5f, 1.97f, 0f), new Vector3(-5f, 1.97f, 0f), "Static1")
            });
        }

        for (int i = 0; i < _modes.Count; i++)
        {
            _modes[i].Index = i;
        }

        _uniqueRandom = new UniqueRandom(0, _modes.Count, nameof(CameraModelManager));

        _currentModelIndex = _uniqueRandom.Next();
        boundParameters.Reset(default);
    }

    public CameraBase ActiveModel => _modes.Find(x => x.Index == _currentModelIndex);

    public void Reset(BoundParameters boundParameters)
    {
        _currentModelIndex = _uniqueRandom.Next();
        boundParameters.Reset(default);

        _modes.ForEach(x =>
        {
            x.Reset(boundParameters);
        });
    }

    public int Count => _modes.Count;

    private Vector3 Linear(float normalizedTime, CameraBase model) => model switch
    {
        LinearBase m => Vector3.Lerp(m.A, m.B, normalizedTime),
        StaticCamera m => Vector3.Lerp(m.A, m.B, normalizedTime),
        _ => throw new ArgumentException(nameof(model))
    };

    private Vector3 Sphere(float normalizedTime, CameraBase model)
    {
        float radius = 6f;
        float height = 0.5f;

        // Угол в радианах для кругового движения
        float angle = normalizedTime * Mathf.PI;

        // Позиция на полусфере (Y >= 0): сферические координаты
        float x = radius * Mathf.Sin(angle);
        float y = height + (radius * Mathf.Abs(Mathf.Sin(angle))); // Полусфера (верхняя)
        float z = radius * Mathf.Cos(angle);

        Debug.Log($"{x} {y} {z}");
        return new Vector3(x, y, z);
    }
}

