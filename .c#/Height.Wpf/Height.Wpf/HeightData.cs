namespace Height.Wpf
{
    public struct HeightData(float unityZ, float height)
    {
        public float UnityZ = unityZ;

        public float Height = height;

        public byte C = 0;
    }
}
