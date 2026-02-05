using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;

class BoundParameters : IResetable<int>
{
    private UniqueRandom _uniqueRandom;
    private int _currentModelIndex;

    public string Description { get; private set; }
    public float Freq { get; private set; }
    public Bounds Bound { get; private set; }
    public Vector3[] CameraMovesets { get; private set; }

    public BoundParameters(string description, float freq, Bounds bound, Vector3[] cameraMovesets)
    {
        Description = description;
        Freq = freq;
        Bound = bound;
        CameraMovesets = cameraMovesets;

        _uniqueRandom = new UniqueRandom(0, cameraMovesets.Length, nameof(CameraModelManager));
        _currentModelIndex = _uniqueRandom.Next();
    }

    public Vector3 CurrentMoveset => CameraMovesets[_currentModelIndex];

    public void Reset(int o)
    {
        _currentModelIndex = _uniqueRandom.Next();
    }
}

class BoundManager : IResetable<string>
{
    private int _currentBoundIndex;
    private readonly UniqueRandom _uniqueRandom;
    private readonly BoundParameters[] _bounds;

    public BoundManager()
    {
        List<Vector3> houseMoveset = new()
        {
            new(+1f, 0.5f, +float.MaxValue),
            new(+1f, 0.5f, -float.MaxValue),
            new(-1f, 0.5f, +float.MaxValue),
            new(-1f, 0.5f, -float.MaxValue),

            new(+1.5f, 1f, +float.MaxValue),
            new(-1.5f, 1f, -float.MaxValue),
            new(+1.5f, -1f, +float.MaxValue),
            new(-1.5f, -1f, -float.MaxValue),
        };

        _bounds = new BoundParameters[]
        {
            //new("Внутри дома 1-ый эт.", 0.2f, FromZero(new Vector3(0f, 0.35f, -2f), new Vector3(5f, 2.40f, 3f)), houseMoveset.ToArray()),
            //new("Внутри дома 2-ой эт.", 0.2f, FromZero(new Vector3(0f, 3.35f, -2f), new Vector3(3.40f, 2.00f, 3f)), houseMoveset),
            new("Глобальный объём", 0.7f, FromZero(new Vector3(0f, 0.5f, -2f), new Vector3(8f, 6f, 12f)), houseMoveset.ToArray()),
            //new("Забор левая сторона", 0.3f, FromMinMax(new Vector3(0f, 0.5f, 17f), new Vector3(11f, 4f, -10f)), new List<Vector3>()),
        };

        _uniqueRandom = new UniqueRandom(0, _bounds.Count(), nameof(BoundManager));
    }

    public BoundParameters ActiveBound => _bounds[_currentBoundIndex];

    private static Bounds FromMinMax(Vector3 min, Vector3 max)
    {
        Bounds bounds = new()
        {
            min = min,
            max = max
        };

        return bounds;
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

    public void Reset(string description)
    {
        int index = Array.FindIndex(_bounds, x => x.Description == description);
        if (index != -1)
        {
            _currentBoundIndex = index;
        }
        else
        {
            _currentBoundIndex = _uniqueRandom.Next();
        }
    }
}
