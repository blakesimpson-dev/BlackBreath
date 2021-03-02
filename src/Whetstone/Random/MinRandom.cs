namespace Whetstone.Random
{
    public class MinRandom : IRandom
    {
        public int Next(int maxValue)
        {
            return 0;
        }

        public int Next(int minValue, int maxValue)
        {
            return minValue;
        }

        public RandomState Save()
        {
            return new RandomState();
        }

        public void Restore(RandomState state) { }
    }
}
