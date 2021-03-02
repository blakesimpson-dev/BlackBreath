### Example of Border Only call:

IMapGenerationProcess<Map> mapGenerationProcess = new MGPBorderOnly<Map>(_mapWidth, _mapHeight, mapPalettePath);

currentMap = Map.Generate(mapGenerationProcess);
fov = new FOV(currentMap);

### Example of Random Rooms call:

IMapGenerationProcess<Map> mapGenerationProcess = new MGPRandomRooms<Map>(
    _mapWidth,
    _mapHeight,
    mapPalettePath,
    20,
    10,
    8
);

### Example of Cellular Automata call:

IMapGenerationProcess<Map> mapGenerationProcess = new MGPCellularAutomata<Map>(
    _mapWidth,
    _mapHeight,
    mapPalettePath,
    45,
    4,
    2
);

currentMap = Map.Generate(mapGenerationProcess);
fov = new FOV(currentMap);

### Example of String Deserialization call:

IMapGenerationProcess<Map> mapGenerationProcess = new MGPStringDeserialization<Map>(mapString, mapPalettePath);

currentMap = Map.Generate(mapGenerationProcess);
fov = new FOV(currentMap);

###