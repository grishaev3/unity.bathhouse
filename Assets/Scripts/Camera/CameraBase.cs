using System;
using UnityEngine;

public interface IPeriod
{
    public TimeSpan Duration { get; set; }
}

public class Period : IPeriod
{
    public TimeSpan Duration { get; set; }
}

abstract class CameraBase : IPeriod
{
    protected Vector3 _direction;

    public int Index { get; set; }

    public string Name { get; set; }

    public TimeSpan Duration { get; set; }

    public Func<float, CameraBase, Vector3> Func { get; set; }

    public abstract void Reset(Bounds bounds);
}