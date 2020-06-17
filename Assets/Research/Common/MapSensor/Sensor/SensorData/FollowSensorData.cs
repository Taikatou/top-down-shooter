﻿using System;
using System.Linq;
using Research.Common.MapSensor.GridSpaceEntity;
using Research.LevelDesign.NuclearThrone.Scripts;
using UnityEngine;

namespace Research.Common.MapSensor.Sensor.SensorData
{
    public class FollowSensorData : BaseSensorData
    {
        private readonly Transform _parentTransform;
        private StartEndPosition _startEndPosition;

        private Vector3Int _agentCell;

        public FollowSensorData(TileMapSensorConfig config, Transform parentTransform) : base(config)
        {
            _parentTransform = parentTransform;
        }

        public override void UpdateMap(GridSpace[,] observations)
        {
            Array.Clear(observations, 0, Config.sizeX * Config.sizeY);
            // NuclearThroneMapGenerator.OutputDebugMap(observations);
            var map = Config.mapAccessor.GetMap();
            var cell = Config.mapAccessor.GetPosition(_parentTransform.position);

            var trackedPosition = TileMapSensorConfigUtils.GetTrackStartEndPosition(Config, cell);
            
            _agentCell = Config.mapAccessor.GetPosition(_parentTransform.transform.position);
            _startEndPosition = TileMapSensorConfigUtils.GetTrackStartEndPosition(Config, _agentCell);

            for (var y = trackedPosition.StartPos.y; y <= trackedPosition.EndPos.y; y++)
            {
                for (var x = trackedPosition.StartPos.x; x <= trackedPosition.EndPos.x; x++)
                {
                    var xyValid = x >= 0 && x <= map.GetUpperBound(0) &&
                                  y >= 0 && y <= map.GetUpperBound(1);
                    if (xyValid && XyValid(x, y))
                    {
                        InsertXy(observations, x, y, map[x, y]);   
                    }
                }
            }
        }

        private bool XyValid(int x, int y)
        {
            var xyValid = x >= _startEndPosition.StartPos.x && x <= _startEndPosition.EndPos.x &&
                          y >= _startEndPosition.StartPos.y && y <= _startEndPosition.EndPos.y;
            return xyValid;
        }

        private void InsertXy(GridSpace[,] observations, int x, int y, GridSpace gridSpace)
        {
            var contains = Config.GridSpaceValues.ContainsKey(gridSpace);
            if (contains)
            {
                var mappedCell = TileMapSensorConfigUtils.GetMappedPosition(Config, x, y, _agentCell);
                observations[mappedCell.x, mappedCell.y] = gridSpace;
                //Debug.Log(entity.GridSpace + "\t" + mappedCell.x + "\t" + mappedCell.y);
            }
        }

        public override void UpdateMapEntityPositions(GridSpace[,] observations, BaseMapPosition[] entityMapPositions)
        {
            var debugText = "";
            var counter = 0;
            foreach (var entityList in entityMapPositions)
            {
                foreach (var entity in entityList.GetGridSpaceType(Config.TeamId))
                {
                    var entityCell = Config.mapAccessor.GetPosition(entity.Position);
                    if (XyValid(entityCell.x, entityCell.y))
                    {
                        InsertXy(observations, entityCell.x, entityCell.y, entity.GridSpace);   
                    }
                }
            }
            Debug.Log(debugText + "\t" + counter);
        }
    }
}
