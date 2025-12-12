using System;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

class CameraModelManager : IResetable
{
    private int _currentModelIndex = default;
    private readonly List<CameraBase> _modes = null;

    public CameraModelManager(Bounds bound)
    {
        _modes = new()
        {
            //new SphereModel { Duration = TimeSpan.FromSeconds(20), Func = Sphere, Name = "Sphere"},
            //new LinearRandom(TimeSpan.FromSeconds(10), Linear, "Random", bound),

            new LinearBase(TimeSpan.FromSeconds(12), new Vector3(+1f, 0.5f, +float.MaxValue), Linear, "LeftToRightFront", bound),
            new LinearBase(TimeSpan.FromSeconds(12), new Vector3(+1f, 0.5f, -float.MaxValue), Linear, "LeftToRightBack", bound),

            new LinearBase(TimeSpan.FromSeconds(12), new Vector3(-1f, 0.5f, +float.MaxValue), Linear, "RightToLeftFront", bound),
            new LinearBase(TimeSpan.FromSeconds(12), new Vector3(-1f, 0.5f, -float.MaxValue), Linear, "RightToLeftBack", bound),

            new LinearBase(TimeSpan.FromSeconds(12), new Vector3(+1.5f, 1f, +float.MaxValue), Linear, "DownToUpFront", bound),
            new LinearBase(TimeSpan.FromSeconds(12), new Vector3(-1.5f, 1f, -float.MaxValue), Linear, "DownToUpBack", bound),

            new LinearBase(TimeSpan.FromSeconds(12), new Vector3(+1.5f, -1f, +float.MaxValue), Linear, "DownToUpFront", bound),
            new LinearBase(TimeSpan.FromSeconds(12), new Vector3(-1.5f, -1f, -float.MaxValue), Linear, "DownToUpBack", bound),
        };

        for (int i = 0; i < _modes.Count; i++)
        {
            _modes[i].Index = i;
        }
    }

    public CameraBase ActiveModel => _modes.Find(x => x.Index == _currentModelIndex);

    public void Reset(object @object)
    {
        var bound = (Bounds) @object;

        _currentModelIndex = UniqueRandom.Next(0, _modes.Count, _currentModelIndex);

        _modes.ForEach(x =>
        {
            x.Reset(bound);
        });
    }

    public int Count => _modes.Count;

    private Vector3 Sphere(float normalizedTime, CameraBase model)
    {
        float radius = (model as SphereModel).Radius;
        float height = (model as SphereModel).Height;

        // Угол в радианах для кругового движения
        float angle = normalizedTime * Mathf.PI;

        // Позиция на полусфере (Y >= 0): сферические координаты
        float x = radius * Mathf.Sin(angle);
        float y = height + (radius * Mathf.Abs(Mathf.Sin(angle))); // Полусфера (верхняя)
        float z = radius * Mathf.Cos(angle);

        return new Vector3(x, y, z);
    }

    private Vector3 Linear(float normalizedTime, CameraBase model)
    {
        LinearBase linearBase = (model as LinearBase);

        return Vector3.Lerp(linearBase.A, linearBase.B, normalizedTime);
    }
}

