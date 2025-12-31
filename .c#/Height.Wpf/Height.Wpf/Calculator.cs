using System.Numerics;

namespace Height.Wpf
{
    public class Calculator
    {
        private int _textureLength;

        private readonly Vector2 _terrainTopLeft;
        private readonly Vector2 _terrainBottomRight;
        private readonly HeightData[] _heightData;

        private readonly float _unitysizeZ, _unitysizeX;
        private readonly float _heightDiff;

        public Calculator(int textureLength, Vector2 terrainTopLeft, Vector2 terrainBottomRight, HeightData[] heightData)
        {
            _textureLength = textureLength;

            _terrainTopLeft = terrainTopLeft;
            _terrainBottomRight = terrainBottomRight;

            _heightData = heightData;

            _unitysizeX = Math.Abs(_terrainTopLeft.X - _terrainBottomRight.X);  // 50
            _unitysizeZ = Math.Abs(_terrainTopLeft.Y - _terrainBottomRight.Y);  // 50

            _heightDiff = Math.Abs(heightData.Max(x => x.Height) - heightData.Min(x => x.Height));
        }

        public byte GetColor(int x, int y)
        {
            y = _textureLength - y; // Переворачиваем по Y

            var (unityX, unityZ) = Calc(x, y);

            HeightData? _heightData = FindByZ(unityZ);

            if (_heightData != null)
            {
                return Color(_heightData.Value);
            }
            else
            {
                return (byte)byte.MaxValue;
            }

            byte Color(HeightData _heightData)
            {
                float coeff = _heightDiff * _heightData.Height;

                return (byte)Math.Floor(coeff * 255f);
            }
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
