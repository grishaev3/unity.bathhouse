using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Vector3Extender
    {
        public static Vector3 Random(Bounds bounds)
        {
            return new Vector3(
                UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
                UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
            );
        }
    }

    public class UniqueRandom
    {
        private readonly int _maxTryCount = 10;
        private readonly string _id;
        private int _mimValue, _maxValue;
        private Stack<int> _stack;

        public UniqueRandom(int minValue, int maxValue, string id)
        {
            _id = id;

            _mimValue = minValue;
            _maxValue = maxValue;
            _stack = new Stack<int>(maxValue - minValue);
        }

        public static bool NextBool()
        {
            return UnityEngine.Random.Range(0, 1024) % 2 == 0;
        }

        public void SetFreq(int number, float freq)
        {

        }

        public int Next()
        {
            if (_id == "CameraModelManager")
            {
                int a = default;
            }

            _stack.TryPeek(out int lastValue);

            int tryNumber = 0;
            int value;
            do
            {
                value = UnityEngine.Random.Range(_mimValue, _maxValue);
                if ((tryNumber += 1) > _maxTryCount)
                {
                    break;
                }

            } while (value == lastValue);

            _stack.Push(value);
            
            return value;
        }
    }
}
