using System;
using System.Collections.Generic;

namespace Whetstone.Random
{

    public class KnownSeriesRandom : IRandom
    {
        private long _numberGenerated;
        private Queue<int> _series;

        public KnownSeriesRandom(params int[] series)
        {
            if (series == null)
            {
                throw new ArgumentNullException(nameof(series), "Series cannot be null");
            }

            _series = new Queue<int>();

            foreach (int number in series)
            {
                _series.Enqueue(number);
            }
        }

        public int Next(int maxValue)
        {
            return Next(0, maxValue);
        }

        public int Next(int minValue, int maxValue)
        {
            int value = _series.Dequeue();
            if (value < minValue)
            {
                throw new ArgumentOutOfRangeException(nameof(minValue), "Next value in series is smaller than the minValue parameter");
            }

            if (value > maxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(maxValue), "Next value in series is larger than the maxValue parameter");
            }

            _series.Enqueue(value);
            _numberGenerated++;

            return value;
        }

        public RandomState Save()
        {
            return new RandomState
            {
                NumberGenerated = _numberGenerated,
                Seed = _series.ToArray()
            };
        }

        public void Restore(RandomState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state), "RandomState cannot be null");
            }

            _series = new Queue<int>();

            foreach (int i in state.Seed)
            {
                _series.Enqueue(i);
            }
            
            _numberGenerated = state.NumberGenerated;
        }
    }
}