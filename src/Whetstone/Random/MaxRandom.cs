namespace Whetstone.Random
{
    public class MaxRandom : IRandom
    {
        public int Next(int maxValue)
        {
            return maxValue;
        }

        public int Next(int minValue, int maxValue)
        {
            return maxValue;
        }

        public RandomState Save()
        {
            return new RandomState();
        }

        public void Restore(RandomState state) { }
    }
}
