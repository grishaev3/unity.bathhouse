using System;
using UnityEngine;

class StaticCamera : CameraBase
{
    public Vector3 A { get; set; }

    public Vector3 B { get; set; }

    public StaticCamera(
        TimeSpan duration,
        Func<float, CameraBase, Vector3> funcLookFrom,
        Func<float, CameraBase, Vector3> funcLookTo,
        Vector3 a, Vector3 b,
        string name)
    {
        A = a;
        B = b;

        _funcLookFrom = funcLookFrom;
        _funcLookAt = funcLookTo;

        Duration = duration;

        Name = name;

        Reset(null);
    }

    public override void Reset(BoundParameters bounds)
    {
    }
}
