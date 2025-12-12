using UnityEngine;

namespace Assets.Scripts
{
    public struct CircularQueue<T>
    {
        private T[] buffer;
        private int head, tail, count, capacity;

        public CircularQueue(int capacity)
        {
            this.capacity = capacity;
            buffer = new T[capacity];
            head = tail = count = 0;
        }

        public bool Enqueue(T item)
        {
            if (count == capacity) return false; // Полная
            buffer[tail] = item;
            tail = (tail + 1) % capacity;
            count++;
            return true;
        }

        public bool Dequeue(out T item)
        {
            if (count == 0) { item = default; return false; } // Пустая
            item = buffer[head];
            head = (head + 1) % capacity;
            count--;
            return true;
        }

        public bool IsEmpty => count == 0;
        public int Count => count;
    }

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

    public static class UniqueRandom
    {

        public static bool NextBool()
        {
            return UnityEngine.Random.Range(1, 101) % 2 == 0;
        }

        public static int Next(int minValue, int maxValue, int lastValue)
        {
            var tryNumber = 0;

            int value;
            do
            {
                value = UnityEngine.Random.Range(minValue, maxValue);
                if ((tryNumber += 1) > 10)
                    break;

            } while (value == lastValue);

            lastValue = value;
            return value;
        }
    }
}
