using System;

namespace Whetstone.Algorithms
{
    public class UnionFind
    {
        private readonly int[] _id;
        private readonly int[] _size;

        public UnionFind(int count)
        {
            if (count < 0)
            {
                throw new ArgumentException("count must be positive", nameof(count));
            }

            Count = count;
            _id = new int[count];
            _size = new int[count];

            for (int i = 0; i < count; i++)
            {
                _id[i] = i;
                _size[i] = 1;
            }
        }

        public int Count { get; private set; }

        public int Find(int p)
        {
            if (p < 0 || p >= _id.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(p), "Index out of bounds");
            }
            while (p != _id[p])
            {
                p = _id[p];
            }
            return p;
        }

        public bool Connected(int p, int q)
        {
            return Find(p) == Find(q);
        }

        public void Union(int p, int q)
        {
            int i = Find(p);
            int j = Find(q);
            if (i == j)
            {
                return;
            }
            if (_size[i] < _size[j])
            {
                _id[i] = j;
                _size[j] += _size[i];
            }
            else
            {
                _id[j] = i;
                _size[i] += _size[j];
            }
            Count--;
        }
    }
}