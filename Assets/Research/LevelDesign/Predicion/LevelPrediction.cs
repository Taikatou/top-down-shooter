using System.Collections.Generic;
using Research.LevelDesign.Scripts;
using Unity.Barracuda;
using UnityEditor;
using UnityEngine;

namespace Research.LevelDesign.Predicion
{
    public class LevelPrediction : MonoBehaviour
    {
        public NNModel predictionModel;
        public MapAccessor mapAccessor;
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(LevelPrediction))]
    public class LevelPredictionEditor : Editor
    {
        private IWorker _engine;
        private IWorker Engine
        {
            get
            {
                if (_engine == null)
                {
                    var levelGen = (LevelPrediction)target;
                    var model =  ModelLoader.Load(levelGen.predictionModel);
                    _engine = WorkerFactory.CreateWorker(model, WorkerFactory.Device.CPU);
                }

                return _engine;
            }
        }
        
        public Dictionary<int, float> inputSwap = new Dictionary<int, float>
        {
            {0, 0f},
            {1, 1f},
            {2, 2f},
            {8, 3f},
            {9, 4f},
            {10, 5f}
        };

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var fairness = GetFairness();
            EditorGUILayout.LabelField("Winrate Player 1: " + fairness);
        }

        private float GetFairness()
        {
            var levelGen = (LevelPrediction)target;
            if (levelGen.predictionModel)
            {
                var input = new float[50*49];
                var counter = 0;
                var map = levelGen.mapAccessor.GetMap(true);
                for (var i = 0; i < map.GetUpperBound(0); i++)
                {
                    for (var j = 0; j < map.GetUpperBound(1); j++)
                    {
                        var data = (int) map[i, j];
                        input[counter] = inputSwap[data] / 5f;
                        counter++;
                    }
                }

                var debug = string.Join(",", input);
                Debug.Log(debug);
                var inputTensor = new Tensor(1, 50, 49, 1, input);
                Engine.Execute(inputTensor);

                var fairness = Engine.PeekOutput()[0];
                inputTensor.Dispose();
                return fairness;
            }
            return 0f;
        }
    }
#endif
}
