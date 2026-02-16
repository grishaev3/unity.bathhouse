using Assets.Scripts;
using System;
using UnityEngine;

class LinearRandom : LinearBase
{
    public LinearRandom(float freq, TimeSpan duration, Func<float, CameraBase, Vector3> func, string name, BoundParameters bounds, CameraDirectionType directionType) :
        base(freq, duration, func, name, bounds, directionType)
    {
        _funcLookFrom = func;

        Duration = duration;
        
        Name = name;

        Reset(bounds);
    }

    public override void Reset(BoundParameters boundParameters)
    {
        Bounds bound = boundParameters.Bound;

        A = Vector3Extender.Random(bound);

        B = Vector3Extender.Random(bound);
    }
}
