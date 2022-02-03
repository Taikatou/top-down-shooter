using Research.Common.MapSensor.GridSpaceEntity;
using Research.LevelDesign.Scripts.MLAgents;

namespace Research.Common.MapSensor.Sensor.SensorData
{
    public class FullSensorData : BaseSensorData
    {
        public override void UpdateMap(GridSpace[,] observations)
        {
            if (Config.MapAccessor)
            {
                var map = Config.MapAccessor.GetMap();
                var startEnd = TileMapSensorConfigUtils.GetStartEndPosition(Config);
                for (var y = startEnd.StartPos.y; y < startEnd.EndPos.y; y++)
                {
                    for (var x = startEnd.StartPos.x; x < startEnd.EndPos.x; x++)
                    {
                        var value = map[x, y];
                        observations[x, y] = value;
                    }
                }
            }
        }
        
        public override void UpdateMapEntityPositions(GridSpace[,] observations, BaseMapPosition[] entityMapPositions)
        {
            foreach (var entityList in entityMapPositions)
            {
                foreach (var entity in entityList.GetGridSpaceType(Config.TeamId))
                {
                    if (Config.MapAccessor)
                    {
                        var cell = Config.MapAccessor.GetPosition(entity.Position);
                        var trackPos = TileMapSensorConfigUtils.GetStartEndPosition(Config);

                        var xValid = cell.x >= trackPos.StartPos.x && cell.x < trackPos.EndPos.x;
                        var yValid = cell.y >= trackPos.StartPos.y && cell.y < trackPos.EndPos.y;

                        if (xValid && yValid)
                        {
                            var gridType = entity.GridSpace;
                            var contains = Config.GridSpaceValues.ContainsKey(gridType);
                            if (contains)
                            {
                                observations[cell.x, cell.y] = gridType;
                            }
                        }   
                    }
                }
            }
        }

        public FullSensorData(ref TileMapSensorConfig config) : base(config)
        {
        }
    }
}
