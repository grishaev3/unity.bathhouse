namespace Height.Wpf
{
    public class SimpleNoise
    {
        private readonly byte[] _noiseMap;
        private readonly int _width, _height;
        private readonly float _scaleX, _scaleZ;

        public SimpleNoise(float terrainSizeX, float terrainSizeZ, int resolution, int seed)
        {
            _width = resolution;
            _height = resolution;
            _scaleX = _width / terrainSizeX;   // ~0.041 для 1025/50
            _scaleZ = _height / terrainSizeZ;  // ~0.041 для 1025/50

            Random rand = new(seed);
            _noiseMap = new byte[_width * _height];
            for (int i = 0; i < _noiseMap.Length; i++)
            {
                _noiseMap[i] = (byte)rand.Next(256);
            }
        }

        public float GetNoise(float unityX, float unityZ)
        {
            float tx = unityX * _scaleX;
            float tz = unityZ * _scaleZ;

            // **ИСПРАВЛЕНО**: Правильный wraparound для отрицательных
            int xi = (int)tx;
            int yi = (int)tz;

            xi = (xi % _width + _width) % _width;  // [-∞..∞] → [0..width-1]
            yi = (yi % _height + _height) % _height;

            int x2i = (xi + 1) % _width;
            int y2i = (yi + 1) % _height;

            float xf = tx - (int)tx;
            float yf = tz - (int)tz;

            float i1 = Interpolate(_noiseMap[xi + yi * _width] / 255f, _noiseMap[x2i + yi * _width] / 255f, xf);
            float i2 = Interpolate(_noiseMap[xi + y2i * _width] / 255f, _noiseMap[x2i + y2i * _width] / 255f, xf);

            return Interpolate(i1, i2, yf);
        }

        private static float Interpolate(float a, float b, float t)
        {
            t = t * t * (3f - 2f * t); // Smoothstep
            return a * (1f - t) + b * t;
        }

        // **Кубическая smoothstep** (3x smoother)
        private static float CubicInterpolate(float a, float b, float t)
        {
            t = t * t * (3f - 2f * t); // 6t^5 - 15t^4 + 10t^3
            return a * (1f - t) + b * t;
        }

        // **Квинтическая (5 степени)** - супер гладкая
        private static float QuinticInterpolate(float a, float b, float t)
        {
            float t2 = t * t;
            t = 6f * t2 * t2 * t - 15f * t2 * t2 + 10f * t2 * t; // 6t^5 - 15t^4 + 10t^3
            return a * (1f - t) + b * t;
        }
    }
}