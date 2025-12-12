using System.Linq;
using Assets.Scripts;
using UnityEngine;

class BoundManager : IResetable
{
    private int _currentBoundIndex = default;

    private readonly (string name, float freq, Bounds bound)[] _bounds = new (string name, float freq, Bounds bound)[]
    {
        ("¬нутри дома 1-ый эт.", 0.2f, FromZero(new Vector3(0f, 0.35f, -2f), new Vector3(5f, 2.40f, 3f)) ),

        ("¬нутри дома 2-ой эт.", 0.2f, FromZero(new Vector3(0f, 3.35f, -2f), new Vector3(3.40f, 2.00f, 3f)) ),

        ("√лобальный обЄм", 0.8f, FromZero(new Vector3(0f, 0.5f, -2f), new Vector3(6f, 6f, 8f)) ),
    };

    public (string name, Bounds bound) ActiveBound
    {
        get
        {
            return (_bounds[_currentBoundIndex].name, _bounds[_currentBoundIndex].bound);
        }
    }

    public void Reset(object o = null)
    {
        _currentBoundIndex = UniqueRandom.Next(0, _bounds.Count(), _currentBoundIndex);
    }

    public int Count => _bounds.Length;

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
