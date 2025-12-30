using System.Numerics;

namespace Height.Wpf
{
    public class Calculator
    {
        private Vector2 _terrainTopLeft;
        private Vector2 _terrainBottomRight;
        private int _textureLength;

        public Calculator(Vector2 terrainTopLeft, Vector2 terrainBottomRight, int textureLength)
        {
            _terrainTopLeft = terrainTopLeft;
            _terrainBottomRight = terrainBottomRight;
            _textureLength = textureLength;
        }

        public (double u, double v) Calc(float unityX, float unityZ)
        {
            float startX = _terrainTopLeft.X; 
            float startZ = _terrainTopLeft.Y;

            float endX = _terrainBottomRight.X; 
            float endZ = _terrainBottomRight.Y;

            float sizeZ = Math.Abs(startZ - endZ);      // 50
            float sizeX = Math.Abs(startX - endX);      // 35

            return 
            (
                Math.Abs((unityX - startX) / sizeX * _textureLength),
                Math.Abs((unityZ - startZ) / sizeZ * _textureLength)
            );
        }

    }
}
