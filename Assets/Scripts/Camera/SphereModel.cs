using UnityEngine;

class SphereModel : CameraBase
{
    public float Height { get; set; } = 0.5f;

    public float Radius { get; set; } = 6f;

    public override void Reset(BoundParameters bounds) { }
}
