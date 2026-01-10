using System.Numerics;

namespace Height.Wpf
{
    public class Calculator
    {
        private readonly int _textureLength;

        private readonly Vector2 _terrainTopLeft;
        private readonly Vector2 _terrainBottomRight;
        private readonly HeightData[] _heightData;

        private readonly float _unitysizeX, _unitysizeZ;
        private readonly float _minHeight, _maxHeight;

        private readonly SimpleNoise _noise;

        public Calculator(int textureLength, Vector2 terrainTopLeft,
            Vector2 terrainBottomRight, HeightData[] heightData, int perlinSeed)
        {
            _textureLength = textureLength;

            _terrainTopLeft = terrainTopLeft;
            _terrainBottomRight = terrainBottomRight;

            _heightData = heightData;

            _unitysizeX = Math.Abs(_terrainTopLeft.X - _terrainBottomRight.X);  // 50
            _unitysizeZ = Math.Abs(_terrainTopLeft.Y - _terrainBottomRight.Y);  // 50

            _minHeight = heightData.Min(x => x.Height);
            _maxHeight = heightData.Max(x => x.Height);

            _noise = new SimpleNoise(_unitysizeX, _unitysizeZ, _textureLength, perlinSeed);
        }

        public byte GetColor(int tx, int ty)
        {
            ty = _textureLength - ty; // Переворачиваем по Y

            var (unityX, unityZ) = Calc(tx, ty);

            var normalizeHeight = GetNormalizeHeight(unityX, unityZ);

            var result = (byte)Math.Floor(normalizeHeight * byte.MaxValue);

            return result;
        }

        private (float UnityX, float UnityZ) Calc(int x, int y)
        {
            return
            (
                UnityX: _terrainTopLeft.X + ((float)x / _textureLength) * _unitysizeX,
                UnityZ: _terrainTopLeft.Y - ((float)y / _textureLength) * _unitysizeZ
            );
        }

        private float GetNormalizeHeight(float unityX, float unityZ)
        {
            float currentHeight = GetAbsoluteHeight(unityX, unityZ);

            var normalizeHeight = Interpolate(_minHeight, _maxHeight, currentHeight);

            return normalizeHeight;
        }

        private float GetAbsoluteHeight(float unityX, float unityZ)
        {
            float baseHeight = 0f;

            (HeightData a, HeightData b) = FindByZ(unityZ);

            baseHeight = GetHeightByZ(unityZ, a, b);

            float noiseValue = _noise.GetNoise(unityX, unityZ);
            float noiseOffset = 0f;
            //float noiseOffset = (noiseValue - 0.5f) * 0.08f; // ±0.04

            return baseHeight + noiseOffset;
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
            if (Math.Abs(b - a) < float.Epsilon) return 0f;
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
