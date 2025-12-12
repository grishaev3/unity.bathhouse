using System;
using UnityEngine;

abstract class CameraBase
{
    protected Vector3 _direction;

    public int Index { get; set; }

    public string Name { get; set; }

    public TimeSpan Duration { get; set; }

    public bool isDirection(float x) => Mathf.Abs(x) == 1f;

    //public Bounds Bounds { get; set; } = new Bounds()
    //{
    //    min = new Vector3 { x = 8f, y = 0.25f, z = 6f },
    //    max = new Vector3 { x = 8f, y = 8.0f, z = 6f }
    //};

    public Func<float, CameraBase, Vector3> Func { get; set; }

    public abstract void Reset(Bounds bounds);
}
