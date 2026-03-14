using UnityEngine;

class BoundParameters : IResetable<int>
{
    private UniqueRandom _uniqueRandom;
    private int _currentModelIndex;

    public string Description { get; private set; }
    public float Freq { get; private set; }
    public Bounds Bound { get; private set; }
    public Vector3[] CameraMovesets { get; private set; }

    public BoundParameters(float freq, string description, Bounds bound, Vector3[] cameraMovesets)
    {
        Description = description;
        Freq = freq;
        Bound = bound;
        CameraMovesets = cameraMovesets;

        _uniqueRandom = new UniqueRandom(0, cameraMovesets.Length);
        _currentModelIndex = _uniqueRandom.Next();
    }

    public Vector3 CurrentMoveset => CameraMovesets[_currentModelIndex];

    public void Reset(int o)
    {
        _currentModelIndex = _uniqueRandom.Next();
    }
}
