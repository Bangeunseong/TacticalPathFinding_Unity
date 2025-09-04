using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Tools.DataStructures
{
    public class DoubleKeyPriorityQueue<T>
    {
        class KeyNodeComparer : IComparer<(Key, T)> {
            public int Compare((Key, T) x, (Key, T) y) {
                return x.Item1 < y.Item1 ? -1 : x.Item1 > y.Item1 ? 1 : 0;
            }
        }

        readonly SortedSet<(Key, T)> set = new(new KeyNodeComparer());
        readonly Dictionary<T, Key> lookups = new();

        public int Count => set.Count;
        public (Key, T) Min => set.Min;

        public void Add((Key, T) item) {
            set.Add(item);
            lookups[item.Item2] = item.Item1;
        }

        public void Remove(T item) {
            set.Remove((lookups[item], item));
            lookups.Remove(item);
        }

        public bool ContainsKey(T item) => lookups.ContainsKey(item);

        public bool Contains((Key, T) item) => set.Contains(item);

        public void Clear() => set.Clear();
    }

    public readonly struct Key {
        public readonly float k1;
        readonly float k2;

        public Key(float k1, float k2) {
            this.k1 = k1;
            this.k2 = k2;
        }

        public static bool operator <(Key a, Key b) => a.k1 < b.k1 || Mathf.Approximately(a.k1, b.k1) && a.k2 < b.k2;
        public static bool operator >(Key a, Key b) => a.k1 > b.k1 || Mathf.Approximately(a.k1, b.k1) && a.k2 > b.k2;
        public static bool operator ==(Key a, Key b) => Mathf.Approximately(a.k1, b.k1) && Mathf.Approximately(a.k2, b.k2);
        public static bool operator !=(Key a, Key b) => !(a == b);

        public override bool Equals(object obj) => obj is Key key && this == key;
        public override int GetHashCode() => HashCode.Combine(k1, k2);
        public override string ToString() => $"({k1}, {k2})";
    }
}