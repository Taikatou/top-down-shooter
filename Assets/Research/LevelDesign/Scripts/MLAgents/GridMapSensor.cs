using System;
using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.LevelDesign.Scripts.MLAgents
{
    public enum GridSpace
    {
        Empty=0,
        Wall=1,
        Floor=2,
        Team1=3,
        Team2=4,
        Coin=5,
        Projectile1=6,
        Projectile2=7,
        Spawn1=8,
        Spawn2=9,
        Health=10,
        Dead=11
    }
    public class GridMapSensor : GridSensorBase
    {
        private GridSpace[] _detectableGridSpaces;
        private GridSpace[] DetectableGridSpaces
        {
            get
            {
                if (_detectableGridSpaces == null)
                {
                    var gridTags = new List<GridSpace> { GridSpace.Wall, GridSpace.Health };
                    if (!Prior)
                    {
                        gridTags.AddRange(new [] {GridSpace.Team1, GridSpace.Team2, GridSpace.Projectile1, GridSpace.Projectile2});
                    }
                    else
                    {
                        gridTags.AddRange(new [] {GridSpace.Team2, GridSpace.Team1, GridSpace.Projectile2, GridSpace.Projectile1});
                    }

                    _detectableGridSpaces = gridTags.ToArray();
                }

                return _detectableGridSpaces;
            }
        }
        protected override int GetCellObservationSize()
        {
            return DetectableGridSpaces.Length;
        }
        
        protected override bool IsDataNormalized()
        {
            return true;
        }
        
        private void GetObjectData(int tagIndex, float[] dataBuffer)
        {
            dataBuffer[tagIndex] = 1;
        }
        
        public void ProcessDetectedGrid(GridSpace tag, int cellIndex)
        {
            for (var i = 0; i < DetectableGridSpaces.Length; i++)
            {
                if (tag == DetectableGridSpaces[i])
                {
                    Array.Clear(m_CellDataBuffer, 0, m_CellDataBuffer.Length);

                    GetObjectData(i, m_CellDataBuffer);
                    ValidateValues(m_CellDataBuffer, null);
                   
                    Array.Copy(m_CellDataBuffer, 0, m_PerceptionBuffer, cellIndex * m_CellObservationSize, m_CellObservationSize);
                    break;
                }
            }
        }

        public GridMapSensor(string name, Vector3Int gridSize, bool prior) : base(name, new Vector3(1, 1, 1), gridSize, new string[] {}, SensorCompressionType.None, prior)
        {
            
        }
    }
}
