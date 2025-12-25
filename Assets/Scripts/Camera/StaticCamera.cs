using System;
using UnityEngine;

class StaticCamera : CameraBase
{
    public StaticCamera(
        TimeSpan duration,
        Func<float, CameraBase, Vector3> funcLookFrom,
        Func<float, CameraBase, Vector3> funcLookTo,
        string name)
    {
        _funcLookFrom = funcLookFrom;

        _funcLookAt = funcLookTo;

        Duration = duration;

        Name = name;

        Reset(new Bounds());
    }

    public override void Reset(Bounds bounds)
    {
    }
}
