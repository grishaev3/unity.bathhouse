using System;
using System.Linq;
using UnityEngine;

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

class UniqueRandom
{
    private readonly System.Random _random = new();

    private double[] _cumulativeProbs;
    private int[] _values;

    public UniqueRandom(string name, int minValue, int maxValue)
    {
        var values = Enumerable.Range(minValue, maxValue - minValue).ToArray();
        var probabilities = values.Select(x => 1.0d / values.Length).ToArray();

        CtorImpl(values, probabilities);
    }

    public UniqueRandom(string name, int minValue, int maxValue, double[] probabilities)
    {
        var values = Enumerable.Range(minValue, maxValue - minValue).ToArray();

        CtorImpl(values, probabilities);
    }

    public void CtorImpl(int[] values, double[] probabilities)
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

    public int Next(int previousValue = -1)
    {
        int result;
        do
        {
            double r = _random.NextDouble();
            int left = 0, right = _cumulativeProbs.Length - 1;
            while (left <= right)
            {
                int mid = (left + right) / 2;
                if (_cumulativeProbs[mid] < r)
                    left = mid + 1;
                else
                    right = mid - 1;
            }
            result = _values[left];
        }
        while (result == previousValue && _values.Length > 1);

        return result;
    }

    public static bool NextBool()
    {
        return UnityEngine.Random.Range(0, int.MaxValue) % 2 == 0;
    }
}

