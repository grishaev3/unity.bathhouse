using System;
using System.Collections.Generic;
using System.Linq;

interface INumberProvider
{
    int Next(int previousValue = -1);
}

static class NumberProviderFactory
{
    private static Dictionary<string, List<int>> _seq;
    private static bool _benchmark = false;

    public static void Record(string name, int value)
    {
        _seq ??= new Dictionary<string, List<int>>();

        //string json = JsonSerializer.Serialize(_seq, new JsonSerializerOptions { WriteIndented = true });

        if (_seq.TryGetValue(name, out List<int> list))
        {
            list.Add(value);
        }
        else
        {
            list = new List<int>
            {
                value
            };
            _seq[name] = list;
        }
    }

    public static INumberProvider New(string name, int minValue, int maxValue, double[] probabilities)
    {
        INumberProvider item;
        if (!_benchmark)
        {
            item = new UniqueRandom(name, minValue, maxValue, probabilities);
        }
        else
        {
            item = new UniqueDefinite(name);
        }

        return item;
    }
}

class UniqueDefinite : INumberProvider
{
    class DataRecord
    {
        public string Name;
        public int Index;
        public List<int> Values;
    }

    private string _name;

    public UniqueDefinite(string name)
    {
        _name = name;
    }

    private static readonly List<DataRecord> _data = new()
    {
        new DataRecord { Name = "cameraMovesets", Index = 0, Values = new() { 1, 4 } },
        new DataRecord { Name = "_cameraModels",  Index = 0, Values = new() { 0 } },
        new DataRecord { Name = "_bounds",        Index = 0, Values = new() { 0, 2 } },
    };

    public int Next(int previousValue = -1)
    {
        var record = _data.FirstOrDefault(m => m.Name == _name);

        int result = record.Values[record.Index];

        record.Index = (record.Index + 1) % record.Values.Count;

        return result;
    }
}

class UniqueRandom : INumberProvider
{
    private readonly System.Random _random = new();

    private double[] _cumulativeProbs;
    private int[] _values;
    private string _name;

    public UniqueRandom(string name, int minValue, int maxValue, double[] probabilities)
    {
        var values = Enumerable.Range(minValue, maxValue - minValue).ToArray();
        if (values.Length != probabilities.Length)
        {
            throw new ArgumentException("Массивы должны быть одинаковой длины");
        }

        _name = name;
        _values = values;

        // Нормализация и кумулятивная сумма
        double sum = 0;
        double totalProb = probabilities.Sum();
        _cumulativeProbs = new double[probabilities.Length];

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

        NumberProviderFactory.Record(_name, result);

        return result;
    }

    public static bool NextBool()
    {
        return UnityEngine.Random.Range(0, int.MaxValue) % 2 == 0;
    }
}

