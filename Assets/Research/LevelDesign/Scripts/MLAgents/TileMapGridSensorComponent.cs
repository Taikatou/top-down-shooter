using System.Collections.Generic;
using System.Linq;
using Research.CharacterDesign.Scripts.Environment;
using Research.Common.MapSensor.GridSpaceEntity;
using Research.Common.MapSensor.Sensor;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.LevelDesign.Scripts.MLAgents
{

    /// <summary>
    /// Grid-based sensor.
    /// </summary>
    public class TileMapGridSensorComponent : SensorComponent
    {
        // dummy sensor only used for debug gizmo
        GridSensorBase m_DebugSensor;
        List<GridSensorBase> m_Sensors;
        internal IGridPerception m_GridPerception;

        /// <summary>
        /// Name of the generated <see cref="GridSensorBase"/> object.
        /// Note that changing this at runtime does not affect how the Agent sorts the sensors.
        /// </summary>
        public string sensorName;

        /// <summary>
        /// The number of grid on each side.
        /// Note that changing this after the sensor is created has no effect.
        /// </summary>
        public Vector3Int gridSize;

        public GameObject agentGameObject;
        
        [Range(1, 50)]
        [Tooltip("Number of frames of observations that will be stacked before being fed to the neural network.")]
        public int observationStacks = 1;

        public Color[] debugColors;
        
        public TileMapSensorConfig tileMapConfig;

        /// <inheritdoc/>
        public override ISensor[] CreateSensors()
        {
            var environmentInstance = FindObjectOfType<EnvironmentInstance>();
            m_GridPerception = new TileMapGridSensor(
                new Vector2Int(gridSize.x, gridSize.z),
                agentGameObject,
                tileMapConfig,
                environmentInstance
            );

            // debug data is positive int value and will trigger data validation exception if SensorCompressionType is not None.
            m_DebugSensor = new GridMapSensor("DebugGridSensor", gridSize, TeamID == 1);
            m_GridPerception.RegisterDebugSensor(m_DebugSensor);

            m_Sensors = GetGridSensors().ToList();
            if (m_Sensors == null || m_Sensors.Count < 1)
            {
                throw new UnityAgentsException("GridSensorComponent received no sensors. Specify at least one observation type (OneHot/Counting) to use grid sensors." +
                    "If you're overriding GridSensorComponent.GetGridSensors(), return at least one grid sensor.");
            }

            // Only one sensor needs to reference the boxOverlapChecker, so that it gets updated exactly once
            m_Sensors[0].m_GridPerception = m_GridPerception;
            foreach (var sensor in m_Sensors)
            {
                m_GridPerception.RegisterSensor(sensor);
            }

            if (observationStacks != 1)
            {
                var sensors = new ISensor[m_Sensors.Count];
                for (var i = 0; i < m_Sensors.Count; i++)
                {
                    sensors[i] = new StackingSensor(m_Sensors[i], observationStacks);
                }
                return sensors;
            }
            else
            {
                return m_Sensors.ToArray();
            }
        }

        /// <summary>
        /// Get an array of GridSensors to be added in this component.
        /// Override this method and return custom GridSensor implementations.
        /// </summary>
        /// <returns>Array of grid sensors to be added to the component.</returns>
        protected virtual GridSensorBase[] GetGridSensors()
        {
            List<GridSensorBase> sensorList = new List<GridSensorBase>();
            var sensor = new GridMapSensor(sensorName + "-OneHot", gridSize, TeamID == 1);
            sensorList.Add(sensor);
            return sensorList.ToArray();
        }
        
        private int TeamID => GetComponent<BehaviorParameters>().TeamId;

        void OnDrawGizmos()
        {
            if (true)
            {
                if (m_GridPerception == null || m_DebugSensor == null)
                {
                    return;
                }

                m_DebugSensor.ResetPerceptionBuffer();
                m_GridPerception.UpdateGizmo();
                var cellColors = m_DebugSensor.PerceptionBuffer;
                var rotation = m_GridPerception.GetGridRotation();

                var scale = new Vector3(1, 1, 1);
                var gizmoYOffset = new Vector3(0, 0, 0);
                var oldGizmoMatrix = Gizmos.matrix;
                Debug.Log("Tracked length\t" + m_DebugSensor.PerceptionBuffer.Length);
                for (var i = 0; i < m_DebugSensor.PerceptionBuffer.Length; i++)
                {
                    var cellPosition = m_GridPerception.GetCellGlobalPosition(i);
                    var cubeTransform = Matrix4x4.TRS(cellPosition + gizmoYOffset, rotation, scale);
                    Gizmos.matrix = oldGizmoMatrix * cubeTransform;
                    var colorIndex = cellColors[i] - 1;
                    var debugRayColor = Color.white;
                    if (colorIndex > -1 && debugColors.Length > colorIndex)
                    {
                        debugRayColor = debugColors[(int)colorIndex];
                    }
                    Gizmos.color = new Color(debugRayColor.r, debugRayColor.g, debugRayColor.b, .5f);
                    Gizmos.DrawCube(Vector3.zero, Vector3.one);
                }

                Gizmos.matrix = oldGizmoMatrix;
            }
        }
    }
}