using System;
using System.Linq;
using UnityEngine;

class BoundManager : IResetable<string>
{
    private int _currentBoundIndex;
    private readonly UniqueRandom _uniqueRandom;
    private readonly BoundParameters[] _bounds;

    public BoundManager()
    {
        Vector3[] defaultMoveset =
        {
            new(+1f, 0.5f, +float.MaxValue),
            new(+1f, 0.5f, -float.MaxValue),
            new(-1f, 0.5f, +float.MaxValue),
            new(-1f, 0.5f, -float.MaxValue),

            new(+1.5f, +1f, +float.MaxValue),
            new(-1.5f, +1f, -float.MaxValue),
            new(+1.5f, -1f, +float.MaxValue),
            new(-1.5f, -1f, -float.MaxValue),
        };

        _bounds = new BoundParameters[]
        {
            new(0.1f, "Внутри дома 1-ый эт.", FromZero(new Vector3(0f, 0.35f, -2f), new Vector3(5f, 2.40f, 3f)), defaultMoveset),
            new(0.1f, "Внутри дома 2-ой эт.", FromZero(new Vector3(0f, 3.35f, -2f), new Vector3(3.40f, 2.00f, 3f)), defaultMoveset),
            new(0.2f, "Глобальный объём", FromZero(new Vector3(0f, 0.5f, -2f), new Vector3(8f, 6f, 12f)), defaultMoveset),
            new(0.2f, "Забор левая сторона", FromMinMax(new Vector3(0f, 0.5f, 17f), new Vector3(11f, 4f, -10f)), defaultMoveset),
            new(0.4f, "Дом.2", FromZero(new Vector3(-8.2f, 0.5f, 19.4f), new Vector3(10f, 5f, -10f)), defaultMoveset),
        };

        double[] probabilities = _bounds.Select(x => (double)x.Freq).ToArray();
        _uniqueRandom = new UniqueRandom(0, _bounds.Count(), probabilities);
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
