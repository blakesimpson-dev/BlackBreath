namespace Whetstone.Random
{
    public interface IWeightedPool<T>
    {
        void Add(T item, int weight);
        T Choose();
        T Draw();
        void Clear();
    }
}