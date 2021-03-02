using System;

namespace Whetstone.Random
{
    public class GaussianRandom : IRandom
    {
        private int _seed;
        private System.Random _random;
        private long _numberGenerated;
        private double _nextGaussian;
        private bool _useLast = true;

        public GaussianRandom() : this(Environment.TickCount) { }

        public GaussianRandom(int seed)
        {
            _seed = seed;
            _random = new System.Random(_seed);
        }

        public int Next(int maxValue)
        {
            return Next(0, maxValue);
        }

        public int Next(int minValue, int maxValue)
        {
            _numberGenerated++;
            double deviations = 3.5;
            var r = (int)BoxMuller(minValue + ((maxValue - minValue) / 2.0), (maxValue - minValue) / 2.0 / deviations);

            if (r > maxValue)
            {
                r = maxValue;
            }
            else if (r < minValue)
            {
                r = minValue;
            }

            return r;
        }

        public RandomState Save()
        {
            return new RandomState
            {
                NumberGenerated = _numberGenerated,
                Seed = new[] { _seed }
            };
        }

        public void Restore(RandomState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state), "RandomState cannot be null");
            }

            _seed = state.Seed[0];
            _random = new System.Random(_seed);
            _numberGenerated = default(long);
            _nextGaussian = default(double);
            _useLast = true;

            for (long i = 0; i < state.NumberGenerated; i++)
            {
                Next(1);
            }
        }

        private double BoxMuller()
        {
            if (_useLast)
            {
                _useLast = false;
                return _nextGaussian;
            }
            else
            {
                double v1, v2, s;
                
                do
                {
                    v1 = (2.0 * _random.NextDouble()) - 1.0;
                    v2 = (2.0 * _random.NextDouble()) - 1.0;
                    s = (v1 * v1) + (v2 * v2);
                }
                while (s >= 1.0 || s == 0);

                s = Math.Sqrt(-2.0 * Math.Log(s) / s);

                _nextGaussian = v2 * s;
                _useLast = true;
                return v1 * s;
            }
        }

        private double BoxMuller(double mean, double standardDeviation)
        {
            return mean + (BoxMuller() * standardDeviation);
        }
    }
}