using System.Numerics;
using System.Windows.Media.Media3D;

namespace Height.Wpf
{
    public class Calculator
    {
        private int _textureLength;

        private Vector2 _terrainTopLeft;
        private Vector2 _terrainBottomRight;
        
        private float _unitysizeZ, _unitysizeX;

        private float _heightMin, _heightMax;

        private readonly HeightData[] _heightData;

        public Calculator(int textureLength, Vector2 terrainTopLeft, Vector2 terrainBottomRight, HeightData[] heightData)
        {
            // C = ;
            _textureLength = textureLength;

            _terrainTopLeft = terrainTopLeft;
            _terrainBottomRight = terrainBottomRight;

            _heightData = heightData;

            _unitysizeX = Math.Abs(_terrainTopLeft.X - _terrainBottomRight.X);  // 50
            _unitysizeZ = Math.Abs(_terrainTopLeft.Y - _terrainBottomRight.Y);  // 50

            _heightMin = heightData.Min(x => x.Height);
            _heightMax = heightData.Max(x => x.Height);
        }


        public byte GetColor(int x, int y)
        {
            var (unityX, unityZ) = Calc(x, y);

            HeightData? _heightData = FindByZ(unityZ);

            return _heightData != null ? 
                (byte)Math.Floor(_heightData.Value.Height / Math.Abs(_heightMin - _heightMax) / 255f) :
                (byte)byte.MaxValue;
        }

        private HeightData? FindByZ(float unityZ)
        {
            float[] distances = [.. _heightData.Select(heightData => unityZ - heightData.UnityZ)];

            float minDistance = distances.Min();

            int index = Array.FindIndex(distances, x => x == minDistance);

            return index != -1 ? _heightData[index] : null;
        }

        private (float unityX, float unityZ) Calc(int x, int y)
        {
            return
            (
                (_terrainTopLeft.X) + x / _textureLength * _unitysizeX,
                (_terrainTopLeft.Y) - y / _textureLength * _unitysizeZ
            );
        }

        /*
        public (float u, float v) Calc(float unityX, float unityZ)
        {
            return
            (
                Math.Abs((unityX - _terrainTopLeft.X) / _unitysizeX * _textureLength),
                Math.Abs((unityZ - _terrainTopLeft.Y) / _unitysizeZ * _textureLength)
            );
        }
        */
    }
}
