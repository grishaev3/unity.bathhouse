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
        private readonly float _minHeight, _maxHeight;

        public Calculator(int textureLength, Vector2 terrainTopLeft, Vector2 terrainBottomRight, HeightData[] heightData)
        {
            _textureLength = textureLength;

            _terrainTopLeft = terrainTopLeft;
            _terrainBottomRight = terrainBottomRight;

            _heightData = heightData;

            _unitysizeX = Math.Abs(_terrainTopLeft.X - _terrainBottomRight.X);  // 50
            _unitysizeZ = Math.Abs(_terrainTopLeft.Y - _terrainBottomRight.Y);  // 50

            _minHeight = heightData.Min(x => x.Height);
            _maxHeight = heightData.Max(x => x.Height);
        }

        public byte GetColor(int x, int y)
        {
            y = _textureLength - y; // Переворачиваем по Y

            var (_, unityZ) = Calc(x, y);

            var normalizeHeight = GetNormalizeHeight(unityZ);

            var result = (byte)Math.Floor(normalizeHeight * 255f);

            return result;
        }

        public float GatAbsoluteHeight(float unityZ)
        {
            var (_heightDataA, _heightDataB) = FindByZ(unityZ);

            var currentHeight = GetHeightByZ(unityZ, _heightDataA, _heightDataB);
            return currentHeight;
        }

        public float GetNormalizeHeight(float unityZ)
        {
            float currentHeight = GatAbsoluteHeight(unityZ);

            var normalizeHeight = Interpolate(_minHeight, _maxHeight, currentHeight);

            return normalizeHeight;
        }

        private (float unityX, float unityZ) Calc(int x, int y)
        {
            return
            (
                _terrainTopLeft.X + ((float)x / _textureLength) * _unitysizeX,
                _terrainTopLeft.Y - ((float)y / _textureLength) * _unitysizeZ
            );
        }

        private (HeightData a, HeightData b) FindByZ(float unityZ)
        {
            for (int i = 0; i < _heightData.Length - 1; i++)
            {
                if (_heightData[i].UnityZ <= unityZ && unityZ <= _heightData[i + 1].UnityZ)
                {
                    return (_heightData[i], _heightData[i + 1]);
                }
            }

            throw new Exception(nameof(FindByZ));
        }

        private static float GetHeightByZ(float unityZ, HeightData a, HeightData b)
        {
            // Нормализуем X к [0,1]
            float t = (unityZ - a.UnityZ) / (b.UnityZ - a.UnityZ);

            // Ограничиваем t в [0,1] (clamp)
            t = MathF.Max(0f, MathF.Min(1f, t));

            // Линейная интерполяция Y: a.Y + t * (b.Y - a.Y)
            return a.Height + t * (b.Height - a.Height);
        }

        private static float Interpolate(float a, float b, float c)
        {
            if (a == b) return 0f; // Избегаем деления на ноль
            return (c - a) / (b - a);
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
