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
    internal enum Type
    {
        Simple,
        Freq
    }

    private readonly System.Random _random = new();
    private readonly Type _type;

    private double[] _cumulativeProbs;
    private int[] _values;


    public UniqueRandom(int minValue, int maxValue, Type type = Type.Freq)
    {
        _type = type;
        var values = Enumerable.Range(minValue, maxValue - minValue).ToArray();
        var probabilities = values.Select(x => 1.0d / values.Length).ToArray();

        CtorImpl(values, probabilities);
    }

    public UniqueRandom(int minValue, int maxValue, double[] probabilities, Type type = Type.Freq)
    {
        _type = type;
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

    /// <summary>
    /// prev - спец.режим для камеры
    /// </summary>
    /// <param name="prev"></param>
    /// <returns></returns>
    public int Next(int previousValue = 0)
    {
        if (_type == Type.Simple)
        {
            int newValue = _random.Next(_values.First(), _values.Last());
            return newValue >= previousValue ? newValue + 1 : newValue;
        }

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

    public static bool NextBool()
    {
        return UnityEngine.Random.Range(0, int.MaxValue) % 2 == 0;
    }

}

