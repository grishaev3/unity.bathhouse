using System;
using UnityEngine;

class LinearBase : CameraBase
{
    public LinearBase(TimeSpan duration, Vector3 direction, Func<float, CameraBase, Vector3> func, string name, Bounds bounds)
    {
        _direction = direction;

        Duration = duration;
        Func = func;
        Name = name;

        Reset(bounds);
    }

    public Vector3 A { get; set; }

    public Vector3 B { get; set; }

    /// direction.x = -1 +1 направление
    /// direction.y < 1.0f коеффициент 
    /// direction.z minValue maxValue по объёму
    public override void Reset(Bounds bound)
    {
        float start, end, z;
        switch (GetDirectionType(_direction))
        {
            case DirectionType.Zero:
                break;

            case DirectionType.X:
                start = _direction.x > 0 ? bound.min.x : bound.max.x;
                end = _direction.x > 0 ? bound.max.x : bound.min.x;
                z = IsBound(_direction.z) ? (_direction.z > 0 ? bound.max.z : bound.min.z) : 0f;
                var height = bound.min.y + bound.size.y * _direction.y;

                A = new Vector3(start, height, z);
                B = new Vector3(end, height, z);

                break;
            case DirectionType.Y:

                start = _direction.y > 0 ? bound.min.y : bound.max.y;
                end = _direction.y > 0 ? bound.max.y : bound.min.y;
                z = IsBound(_direction.z) ? (_direction.z > 0 ? bound.max.z : bound.min.z) : 0f;

                A = new Vector3(_direction.x, start, z);
                B = new Vector3(_direction.x, end, z);

                break;
            case DirectionType.Z:
                throw new NotImplementedException();
        }

        static DirectionType GetDirectionType(Vector3 v)
        {
            if (IsDirection(v.x)) return DirectionType.X;
            if (IsDirection(v.y)) return DirectionType.Y;
            if (IsDirection(v.z)) return DirectionType.Z;

            return DirectionType.Zero;
        }

        static bool IsDirection(float x)
        {
            return Mathf.Abs(x) == 1f;
        }

        static bool IsBound(float x)
        {
            return Mathf.Abs(x) == float.MaxValue || Mathf.Abs(x) == float.MinValue;
        }
    }
    internal enum DirectionType
    {
        Zero,
        X,
        Y,
        Z
    }
}
