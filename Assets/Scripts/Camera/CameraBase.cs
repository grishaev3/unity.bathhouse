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

    public string Name { get; protected set; }

    public int Index { get; set; }

    public TimeSpan Duration { get; set; }

    private Vector3 _from;
    private Vector3 _at;

    protected Func<float, CameraBase, Vector3> _funcLookFrom;
    protected Func<float, CameraBase, Vector3> _funcLookAt;

    public (Vector3 from, Vector3 to) Invoke(float normalizedTime, CameraBase camera)
    {
        _from = _funcLookFrom(normalizedTime, camera);

        if (_funcLookAt == null)
        {
            _at = new Vector3(_from.x, _from.y, -2f);
        }
        else
        {
            _at = _funcLookAt(normalizedTime, camera);
        }

        return (_from, _at);
    }

    public abstract void Reset(Bounds bounds);
}