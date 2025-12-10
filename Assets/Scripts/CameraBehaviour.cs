using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;


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

    public override void Reset(Bounds bound)
    {
        float start, end, x, y, z;
        switch (GetDirectionType(_direction))
        {
            case DirectionType.Zero:
                break;

            case DirectionType.X:
                start = _direction.x > 0 ? bound.min.x : bound.max.x;
                end = _direction.x > 0 ? bound.max.x : bound.min.x;
                z = IsBound(_direction.z) ? (_direction.z > 0 ? bound.max.z : bound.min.z) : 0f;

                A = new Vector3(start, _direction.y, z);
                B = new Vector3(end, _direction.y, z);

                break;
            case DirectionType.Y:
                break;
            case DirectionType.Z:
                break;
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
            return Mathf.Abs(x) == float.MaxValue;
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
        new(new Vector3(0f, 0f, -2f), new Vector3(6f, 7f, 8f)),

        // внутри дома 1-ый эт
        //new(new Vector3(0f, 0.25f, -2f), new Vector3(5f, 2.4f, 3f)),

        // внутри дома 2-ой эт.
        //new(new Vector3(0f, 2.7f, 2f), new Vector3(6f, 2.7f, 4f))
    };

    private double _msCurrentTime = 0d;

    private List<CameraBase> _modes = null;

    private Dictionary<string, UniqueRandom> _uniqueRandom = new();

    private int _currentMode;
    private int _currentBoundIndex;

    void Awake()
    {

        Bounds bound = _localBounds[0];

        _modes = new()
        {
            //new SphereModel { Duration = TimeSpan.FromSeconds(20), Func = Sphere, Name = "Sphere"},
            //new LinearRandom(TimeSpan.FromSeconds(10), Linear, "Random", bound),

            new LinearBase(TimeSpan.FromSeconds(12), new Vector3(+1f, 2.0f, +float.MaxValue), Linear, "LeftToRight", bound), //front
            new LinearBase(TimeSpan.FromSeconds(12), new Vector3(+1f, 2.0f, -float.MaxValue), Linear, "LeftToRight", bound), //back

            new LinearBase(TimeSpan.FromSeconds(12), new Vector3(-1f, 2.0f, +float.MaxValue), Linear, "RightToLeft", bound), //front
            new LinearBase(TimeSpan.FromSeconds(12), new Vector3(-1f, 2.0f, -float.MaxValue), Linear, "RightToLeft", bound), //back


            //new LinearBase { Duration = TimeSpan.FromSeconds(14), A = new Vector3(-_range.x, 0.25f, _range.z), B = new Vector3(-_range.x, _range.y, _range.z), Func = Linear, Name = "DownToUp" },
            //new LinearBase { Duration = TimeSpan.FromSeconds(14), A = new Vector3(+_range.x, 0.25f, _range.z), B = new Vector3(+_range.x, _range.y, _range.z), Func = Linear, Name = "DownToUp" },

            //new LinearBase { Duration = TimeSpan.FromSeconds(14), A = new Vector3(-_range.x, _range.y, -_range.z), B = new Vector3(-_range.x, 0.25f, -_range.z), Func = Linear, Name = "UpToDown" },
            //new LinearBase { Duration = TimeSpan.FromSeconds(14), A = new Vector3(+_range.x, _range.y, -_range.z), B = new Vector3(+_range.x, 0.25f, -_range.z), Func = Linear, Name = "UpToDown" },

        };
        for (int i = 0; i < _modes.Count; i++)
        {
            _modes[i].Index = i;
        }

        _uniqueRandom["camera_mode_index"] = new UniqueRandom(0, _modes.Count);
        _uniqueRandom["bound_index"] = new UniqueRandom(0, _localBounds.Length);

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
        transform.LookAt(new Vector3(0f, 2f, -2f));

        CheckAndReset(normalizedTime);
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
            _currentBoundIndex = _uniqueRandom["bound_index"].SafeNext();
            Bounds bound = _localBounds.ElementAtOrDefault(_currentBoundIndex);

            if (bound == null)
            {
                Debug.Break();
            }

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
        _currentMode = _uniqueRandom["camera_mode_index"].SafeNext();
        CameraBase model = _modes.Find(x => x.Index == _currentMode);
        Debug.Log($"_currentMode: {model.Name}-{model.Index}");
    }
}
