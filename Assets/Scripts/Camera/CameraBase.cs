using System;
using UnityEngine;

abstract class CameraBase : IPeriod
{
    protected CameraDirectionType _directionType; 

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

        if (_directionType == CameraDirectionType.Direct)
        {
            _at = new Vector3(_from.x, _from.y, _from.z - 1f);
        }
        else if (_funcLookAt != null)
        {
            _at = _funcLookAt(normalizedTime, camera);
        }
        else if (_funcLookAt == null || _directionType == CameraDirectionType.Center)
        {
            _at = new Vector3(_from.x, _from.y, -2f);
        }

        return (_from, _at);
    }

    public abstract void Reset(BoundParameters bounds);
}