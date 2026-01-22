using System;
using System.Linq;
using Assets.Scripts;
using UnityEngine;

class BoundManager : IResetable
{
    private int _currentBoundIndex = default;

    private readonly (string description, float freq, Bounds bound)[] _bounds = new (string description, float freq, Bounds bound)[]
    {
        //("Внутри дома 1-ый эт.", 0.2f, FromZero(new Vector3(0f, 0.35f, -2f), new Vector3(5f, 2.40f, 3f))),

        //("Внутри дома 2-ой эт.", 0.2f, FromZero(new Vector3(0f, 3.35f, -2f), new Vector3(3.40f, 2.00f, 3f))),

        ("Глобальный обём", 0.8f, FromZero(new Vector3(0f, 0.5f, -2f), new Vector3(8f, 6f, 12f))),

        ("Забор левая сторона", 0.8f, FromMinMax(new Vector3(0f, 0.5f, 17f), new Vector3(11f, 4f, -10f))),
    };

    public (string description, Bounds bound) ActiveBound
    {
        get
        {
            return (_bounds[_currentBoundIndex].description, _bounds[_currentBoundIndex].bound);
        }
    }

    public void Reset(object o = null)
    {
        if (o != null)
        {
            _currentBoundIndex = Array.FindIndex(_bounds, x => x.description == (string)o);
        }
        else
        {
            _currentBoundIndex = UniqueRandom.Next(0, _bounds.Count(), _currentBoundIndex);
        }
    }

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
}
