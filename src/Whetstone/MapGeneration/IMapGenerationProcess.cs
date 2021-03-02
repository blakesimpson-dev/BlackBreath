namespace Whetstone.MapGeneration
{
    public interface IMapGenerationProcess<out TMap> : IMapGenerationProcess<TMap, Cell> where TMap : IMap<Cell> {}

    public interface IMapGenerationProcess<out TMap,TCell> where TMap : IMap<TCell> where TCell : ICell 
    {
        TMap GenerateMap();
    }
}