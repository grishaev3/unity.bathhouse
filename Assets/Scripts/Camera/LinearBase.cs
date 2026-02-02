using System;
using UnityEngine;



class LinearBase : CameraBase
{
    internal class Range<T>
    {
        public T Min { get; set; }
        public T Max { get; set; }

        public Range(T v)
        {
            Min = Max = v;
        }

        public Range(T min, T max)
        {
            Min = min;
            Max = max;
        }
    }

    internal class Properties
    {
        public Range<float> X { get; set; }
        public Range<float> Y { get; set; }
        public Range<float> Z { get; set; }

        public DirectionType DirectionType
        {
            get
            {
                if (X.Min != X.Max) return DirectionType.X;
                if (Y.Min != Y.Max) return DirectionType.Y;
                if (Z.Min != Z.Max) return DirectionType.Z;

                return DirectionType.Zero;
            }
        }
    }

    public Vector3 A { get; set; }
    public Vector3 B { get; set; }

    private Properties _properties;

    public LinearBase(
        TimeSpan duration, 
        Vector3 direction, 
        Func<float, CameraBase, Vector3> func, 
        string name, 
        Bounds bounds,
        CameraDirectionType directionType)
    {
        _direction = direction;
        _directionType = directionType;

        _funcLookFrom = func;

        Duration = duration;

        Name = name;

        Reset(bounds);
    }

    /// direction.x = -1 +1 направление
    /// direction.y < 1.0f коеффициент 
    /// direction.z minValue maxValue по объёму
    public override void Reset(Bounds bound)
    {
        float start, end, z;
        z = CalculateDepth(bound);

        switch (GetDirectionType(_direction))
        {
            case DirectionType.Zero:
                break;

            case DirectionType.X:
                if (_direction.x > 0)
                {
                    start = bound.min.x;
                    end = bound.max.x;
                }
                else
                {
                    start = bound.max.x;
                    end = bound.min.x;
                }

                var height = bound.min.y + bound.size.y * _direction.y;
                A = new Vector3(start, height, z);
                B = new Vector3(end, height, z);

                break;
            case DirectionType.Y:
                if (_direction.y > 0)
                {
                    start = bound.min.y;
                    end = bound.max.y;
                }
                else
                {
                    start = bound.max.y;
                    end = bound.min.y;
                }

                A = new Vector3(_direction.x, start, z);
                B = new Vector3(_direction.x, end, z);

                break;
            case DirectionType.Z:
                throw new NotImplementedException();
        }

        // min.Z || max.Z || 0
        float CalculateDepth(Bounds bound)
        {
            float z;
            if (IsBound(_direction.z))
            {
                if (_direction.z > 0)
                {
                    z = bound.max.z;
                }
                else
                {
                    z = bound.min.z;
                }
            }
            else
            {
                z = (float)0f;
            }

            return z;
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
