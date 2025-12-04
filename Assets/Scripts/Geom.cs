using UnityEngine;

record Geom(string Name, Vector3 Size)
{
    public string Name { get; private set; } = Name;
    public Vector3 Size { get; private set; } = Size;
}
