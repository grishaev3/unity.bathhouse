using System;
using System.Collections.Generic;
using System.Linq;
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

    public class WeightedRandomGenerator
    {
        private readonly System.Random _random = new System.Random();
        private readonly int[] _values;
        private readonly double[] _cumulativeProbs;

        public WeightedRandomGenerator(int[] values, double[] probabilities)
        {
            if (values.Length != probabilities.Length)
                throw new ArgumentException("Массивы должны быть одинаковой длины");

            _values = values;

            // Нормализация и кумулятивная сумма
            double totalProb = probabilities.Sum();
            _cumulativeProbs = new double[probabilities.Length];
            double sum = 0;
            for (int i = 0; i < probabilities.Length; i++)
            {
                sum += probabilities[i] / totalProb;
                _cumulativeProbs[i] = sum;
            }
        }

        public int Next()
        {
            double r = _random.NextDouble();
            // Бинарный поиск для эффективности (O(log n))
            int left = 0, right = _cumulativeProbs.Length - 1;
            while (left <= right)
            {
                int mid = (left + right) / 2;
                if (_cumulativeProbs[mid] < r)
                    left = mid + 1;
                else
                    right = mid - 1;
            }
            return _values[left];
        }
    }
}
