using Assets.Scripts;
using Assets.Scripts.Types;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

class CameraModelManager : IResetable
{
    private int _currentModelIndex;
    private readonly List<CameraBase> _modes;
    private readonly Settings _settings;

    public CameraModelManager(Bounds bound, Settings settings)
    {
        _settings = settings;
        TimeSpan duration = _settings.Timer.CameraModelDuration;

        _modes = new List<CameraBase>();

        switch (_settings.Camera.Mode)
        {
            case CameraMode.Static:
                _modes.Add(new StaticCamera(
                    TimeSpan.FromSeconds(30),
                    (_, _) => new Vector3(-0.63f, 1.97f, +6.00f),
                    Sphere,
                    "Static"
                 ));
                _currentModelIndex = default;
                break;

            case CameraMode.Dynamic:
            default:
                _modes.AddRange(new List<CameraBase>()
                {
                    new LinearBase(duration, new Vector3(+1f, 0.5f, +float.MaxValue), Linear, "LeftToRightFront", bound),
                    new LinearBase(duration, new Vector3(+1f, 0.5f, -float.MaxValue), Linear, "LeftToRightBack", bound),

                    new LinearBase(duration, new Vector3(-1f, 0.5f, +float.MaxValue), Linear, "RightToLeftFront", bound),
                    new LinearBase(duration, new Vector3(-1f, 0.5f, -float.MaxValue), Linear, "RightToLeftBack", bound),

                    new LinearBase(duration, new Vector3(+1.5f, 1f, +float.MaxValue), Linear, "DownToUpFront", bound),
                    new LinearBase(duration, new Vector3(-1.5f, 1f, -float.MaxValue), Linear, "DownToUpBack", bound),

                    new LinearBase(duration, new Vector3(+1.5f, -1f, +float.MaxValue), Linear, "DownToUpFront", bound),
                    new LinearBase(duration, new Vector3(-1.5f, -1f, -float.MaxValue), Linear, "DownToUpBack", bound),

                    new LinearRandom(duration, Linear, "Random", bound),
                    //new SphereModel { Duration = TimeSpan.FromSeconds(20), Func = Sphere, Name = "Sphere"},
                });

                _currentModelIndex = UniqueRandom.Next(0, _modes.Count, _currentModelIndex);
                break;
        }

        for (int i = 0; i < _modes.Count; i++)
        {
            _modes[i].Index = i;
        }
    }

    public CameraBase ActiveModel => _modes.Find(x => x.Index == _currentModelIndex);

    public void Reset(object @object)
    {
        var bound = (Bounds)@object;

        if (_settings.Camera.Mode == CameraMode.Dynamic)
        {
            _currentModelIndex = UniqueRandom.Next(0, _modes.Count, _currentModelIndex);
        }

        _modes.ForEach(x =>
        {
            x.Reset(bound);
        });
    }

    public int Count => _modes.Count;

    private Vector3 Linear(float normalizedTime, CameraBase model)
    {
        LinearBase linearBase = (model as LinearBase);

        return Vector3.Lerp(linearBase.A, linearBase.B, normalizedTime);
    }

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

