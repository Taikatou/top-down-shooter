using MLAgents.Policies;
using MLAgents.Sensors;
using Research.CharacterDesign.Scripts;
using Research.LevelDesign.NuclearThrone.Scripts;
using UnityEngine;

namespace Research.LevelDesign.Scripts
{
    public class TileMapSensor : ISensor
    {
        public int maxSize = 25;

        private int _sizeX;
        private int _sizeY;

        private float[] _map;

        public AiAccessor Accessor => _gameObject.GetComponentInParent<AiAccessor>();

        public void Update()
        {
            
        }

        public void Reset() { }
        
        public byte[] GetCompressedObservation() { return null; }

        private GameObject _gameObject;

        public TileMapSensor(GameObject gameObject)
        {
            _gameObject = gameObject;
        }

        private GridSpace[,] GridSpaces
        {
            get
            {
                if (Accessor != null)
                {
                    if (Accessor.Map != null)
                    {
                        return Accessor.Map;   
                    }
                }
                return null;
            }
        }

        public int[] GetObservationShape()
        {
            var obsSize = _sizeX * _sizeY;
            return new [] { obsSize };
        }

        private GridSpace GetAgentType(TopDownAgent agent)
        {
            if (agent.gameObject == _gameObject)
            {
                return GridSpace.Self;
            }
            var behaviour = _gameObject.GetComponent<BehaviorParameters>();
            var otherBehaviour = agent.GetComponent<BehaviorParameters>();
            var isTeam = behaviour.TeamId == otherBehaviour.TeamId;
            
            return isTeam ? GridSpace.OurTeam : GridSpace.OtherTeam;
        }

        public int Write(ObservationWriter writer)
        {
            UpdateBeforeWrite();
            var mapClone = (float[])_map.Clone();
            foreach (var pairs in Accessor.AgentPosition)
            {
                var agentType = GetAgentType(pairs.Item1);
                var pos = pairs.Item2;
                mapClone[pos.x + (pos.y * _sizeX)] = (float) agentType;
            }
            Debug.Log(mapClone);
            writer.AddRange(mapClone);
            return mapClone.Length;
        }

        public void UpdateBeforeWrite()
        {
            var roomWidth = GridSpaces.GetUpperBound(0);
            _sizeX = roomWidth; // Mathf.Min(roomWidth, maxSize);
            var roomHeight = GridSpaces.GetUpperBound(1);
            _sizeY = roomHeight; // Mathf.Min(roomHeight, maxSize);
            
            _map = new float[_sizeX * _sizeY];
            for (var x = 0; x < _sizeX; x++)
            {
                for (var y = 0; y < _sizeY; y++)
                {
                    _map[x + (y * _sizeX)] = (int)GridSpaces[x, y];
                }
            }
        }

        public SensorCompressionType GetCompressionType()
        {
            return SensorCompressionType.None;
        }

        public string GetName()
        {
            return "TopDown Sensor";
        }
    }
}
