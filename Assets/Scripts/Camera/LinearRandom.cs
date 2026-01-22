using Assets.Scripts;
using System;
using UnityEngine;

class LinearRandom : LinearBase
{
    public LinearRandom(TimeSpan duration, Func<float, CameraBase, Vector3> func, string name, Bounds bounds, CameraDirectionType directionType) :
        base(duration, Vector3.zero, func, name, bounds, directionType)
    {
        _funcLookFrom = func;

        Duration = duration;
        
        Name = name;

        Reset(bounds);
    }

    public override void Reset(Bounds bounds)
    {
        A = Vector3Extender.Random(bounds);

        B = Vector3Extender.Random(bounds);
    }
}
