using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
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

class SphereModel : CameraBase
{
    public float Height { get; set; } = 0.5f;

    public float Radius { get; set; } = 6f;

    public override void Reset(Bounds bounds) { }
}

class LinearBase : CameraBase
{
    enum DirectionType
    {
        Zero,
        X,
        Y,
        Z
    }

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
}

class LinearRandom : LinearBase
{
    public LinearRandom(TimeSpan duration, Func<float, CameraBase, Vector3> func, string name, Bounds bounds) :
        base(duration, Vector3.zero, func, name, bounds)
    {
        Duration = duration;
        Func = func;
        Name = name;

        Reset(bounds);
    }

    public override void Reset(Bounds bounds)
    {
        A = Vector3Extender.Random(bounds);

        B = Vector3Extender.Random(bounds);
    }
}

public class CameraBehaviour : MonoBehaviour
{
    private readonly Bounds[] _localBounds = new Bounds[] {
        // весь доступный
        FromZero(new Vector3(0f, 0.5f, -2f), new Vector3(6f, 6f, 8f)),

        //// внутри дома 1-ый эт
        //FromZero(new Vector3(0f, 0.35f, -2f), new Vector3(5f, 2.40f, 3f)),

        //// внутри дома 2-ой эт.
        //FromZero(new Vector3(0f, 3.10f, -2f), new Vector3(3.60f, 2.10f, 3f))
    };

    private double _msCurrentTime = 0d;

    private List<CameraBase> _modes = null;

    private Dictionary<string, UniqueRandom> _uniqueRandom = new();

    private int _currentMode;
    private int _currentBoundIndex;

    void Awake()
    {
        _uniqueRandom["bound_index"] = new UniqueRandom(0, _localBounds.Length);
        _currentBoundIndex = _uniqueRandom["bound_index"].Next();
        Bounds bound = _localBounds.ElementAtOrDefault(_currentBoundIndex);

        _modes = new()
        {
            //new SphereModel { Duration = TimeSpan.FromSeconds(20), Func = Sphere, Name = "Sphere"},
            //new LinearRandom(TimeSpan.FromSeconds(10), Linear, "Random", bound),

            new LinearBase(TimeSpan.FromSeconds(12), new Vector3(+1f, 0.5f, +float.MaxValue), Linear, "LeftToRight", bound), //front
            new LinearBase(TimeSpan.FromSeconds(12), new Vector3(+1f, 0.5f, -float.MaxValue), Linear, "LeftToRight", bound), //back

            new LinearBase(TimeSpan.FromSeconds(12), new Vector3(-1f, 0.5f, +float.MaxValue), Linear, "RightToLeft", bound), //front
            new LinearBase(TimeSpan.FromSeconds(12), new Vector3(-1f, 0.5f, -float.MaxValue), Linear, "RightToLeft", bound), //back

            new LinearBase(TimeSpan.FromSeconds(12), new Vector3(+1.5f, 1f, +float.MaxValue), Linear, "DownToUp", bound), //front
            new LinearBase(TimeSpan.FromSeconds(12), new Vector3(-1.5f, 1f, -float.MaxValue), Linear, "DownToUp", bound), //back

            new LinearBase(TimeSpan.FromSeconds(12), new Vector3(+1.5f, -1f, +float.MaxValue), Linear, "DownToUp", bound), //front
            new LinearBase(TimeSpan.FromSeconds(12), new Vector3(-1.5f, -1f, -float.MaxValue), Linear, "DownToUp", bound), //back

        };
        for (int i = 0; i < _modes.Count; i++)
        {
            _modes[i].Index = i;
        }

        _uniqueRandom["camera_mode_index"] = new UniqueRandom(0, _modes.Count);
        

        SelectUniqueModeAndLog();
    }

    void LateUpdate()
    {
        if (_modes == null)
        {
            return;
        }

        CameraBase model = _modes.Find(x => x.Index == _currentMode);
        float normalizedTime = GetNormalizedTime(model);
        Vector3 position = model.Func(normalizedTime, model);

        transform.position = position;
        transform.LookAt(new Vector3(position.x, position.y, -2f));

        CheckAndReset(normalizedTime);
    }

    private static Bounds FromZero(Vector3 center, Vector3 size)
    {
        Bounds bounds = new()
        {
            min = new Vector3(-size.x / 2f, center.y, center.z + (-size.z / 2f)),
            max = new Vector3(+size.x / 2f, center.y + size.y, center.z + (+size.z / 2f))
        };

        return bounds;
    }

    private void CheckAndReset(float normalizedTime)
    {
        if (normalizedTime < 0.999f)
        {
            return;
        }

        _msCurrentTime = 0d;

        SelectUniqueModeAndLog();

        _modes.ForEach(x =>
        {
            _currentBoundIndex = _uniqueRandom["bound_index"].Next();
            Bounds bound = _localBounds.ElementAtOrDefault(_currentBoundIndex);

            x.Reset(bound);
        });
    }

    private float GetNormalizedTime(CameraBase model)
    {
        _msCurrentTime += Time.deltaTime * TimeSpan.FromSeconds(1).TotalMilliseconds;

        // учитываем что замедлили время 
        double normalizedTime = _msCurrentTime / (model.Duration.TotalMilliseconds / (1d / Time.timeScale));

        return (float)normalizedTime;
    }

    private Vector3 Sphere(float normalizedTime, CameraBase model)
    {
        float radius = (model as SphereModel).Radius;
        float height = (model as SphereModel).Height;

        // Угол в радианах для кругового движения
        float angle = normalizedTime * Mathf.PI;

        // Позиция на полусфере (Y >= 0): сферические координаты
        float x = radius * Mathf.Sin(angle);
        float y = height + (radius * Mathf.Abs(Mathf.Sin(angle))); // Полусфера (верхняя)
        float z = radius * Mathf.Cos(angle);

        return new Vector3(x, y, z);
    }

    private Vector3 Linear(float normalizedTime, CameraBase model)
    {
        LinearBase linearBase = (model as LinearBase);

        return Vector3.Lerp(linearBase.A, linearBase.B, normalizedTime);
    }

    private void SelectUniqueModeAndLog()
    {
        _currentMode = _uniqueRandom["camera_mode_index"].Next();
        CameraBase model = _modes.Find(x => x.Index == _currentMode);
        Debug.Log($"_currentMode: {model.Name}-{model.Index}");
    }
}
