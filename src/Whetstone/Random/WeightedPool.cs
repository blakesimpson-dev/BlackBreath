using System;
using System.Collections.Generic;

namespace Whetstone.Random
{
    public class WeightedPool<T> : IWeightedPool<T>
    {
        private int _totalWeight;
        private readonly IRandom _random;
        private List<WeightedItem<T>> _pool = new List<WeightedItem<T>>();
        private readonly Func<T, T> _cloneFunc;

        public int Count => _pool.Count;

        public WeightedPool() : this(RSingleton.DefaultRandom) { }

        public WeightedPool(IRandom random, Func<T, T> cloneFunc = null)
        {
            _random = random ?? throw new ArgumentNullException(nameof(random), "Implementation of IRandom must not be null");
            _cloneFunc = cloneFunc;
        }

        public void Add(T item, int weight)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Can not add null item to the pool");
            }

            if (weight <= 0)
            {
                throw new ArgumentException("Weight must be greater than 0", nameof(weight));
            }

            WeightedItem<T> weightedItem = new WeightedItem<T>(item, weight);
            _pool.Add(weightedItem);

            if (int.MaxValue - weight < _totalWeight)
            {
                throw new OverflowException("The weight of items in the pool would be over Int32.MaxValue");
            }

            _totalWeight += weight;
        }

        public T Choose()
        {
            if (_cloneFunc == null)
            {
                throw new InvalidOperationException("A clone function was not defined when this pool was constructed");
            }

            if (Count <= 0 || _totalWeight <= 0)
            {
                throw new InvalidOperationException("Add items to the pool before attempting to choose one");
            }

            WeightedItem<T> item = ChooseRandomWeightedItem();

            return _cloneFunc(item.Item);
        }

        public T Draw()
        {
            if (Count <= 0 || _totalWeight <= 0)
            {
                throw new InvalidOperationException("Add items to the pool before attempting to draw one");
            }

            WeightedItem<T> item = ChooseRandomWeightedItem();

            Remove(item);
            return item.Item;
        }

        private WeightedItem<T> ChooseRandomWeightedItem()
        {
            int lookupWeight = _random.Next(1, _totalWeight);
            int currentWeight = 0;
            WeightedItem<T> item = null;

            foreach (WeightedItem<T> weightedItem in _pool)
            {
                currentWeight += weightedItem.Weight;
                if (currentWeight >= lookupWeight)
                {
                    item = weightedItem;
                    break;
                }
            }

            if (item == null)
            {
                throw new InvalidOperationException("The random lookup was greater than the total weight");
            }

            return item;
        }

        private void Remove(WeightedItem<T> item)
        {
            if (item != null && _pool.Remove(item))
            {
                _totalWeight -= item.Weight;
            }
        }

        public void Clear()
        {
            _totalWeight = 0;
            _pool = new List<WeightedItem<T>>();
        }
    }

    internal class WeightedItem<T>
    {
        public T Item
        {
            get;
            private set;
        }

        public int Weight
        {
            get;
            private set;
        }

        public WeightedItem(T item, int weight)
        {
            Item = item;
            Weight = weight;
        }
    }
}
