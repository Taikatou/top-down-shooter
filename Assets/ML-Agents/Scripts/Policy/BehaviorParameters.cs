using Barracuda;
using System;
using System.Collections.Generic;
using System.Linq;
using BattleResearch.Scripts;
using UnityEngine;

namespace MLAgents
{

    /// <summary>
    /// The Factory to generate policies.
    /// </summary>
    ///
    [AddComponentMenu("ML Agents/Behavior Parameters", (int) MenuGroup.Default)]
    public class BehaviorParameters : MonoBehaviour
    {

        [Serializable]
        private enum BehaviorType
        {
            Default,
            HeuristicOnly,
            InferenceOnly
        }

        [HideInInspector]
        [SerializeField]
        BrainParameters m_BrainParameters = new BrainParameters();
        [HideInInspector]
        [SerializeField]
        NNModel m_Model;
        [HideInInspector]
        [SerializeField]
        InferenceDevice m_InferenceDevice;
        [HideInInspector]
        [SerializeField]
        BehaviorType m_BehaviorType;
        [HideInInspector]
        [SerializeField]
        string m_BehaviorName = "My Behavior";
        [HideInInspector] [SerializeField]
        int m_TeamID = 0;
        [HideInInspector]
        [SerializeField]
        [Tooltip("Use all Sensor components attached to child GameObjects of this Agent.")]
        bool m_useChildSensors = true;

        public BrainParameters brainParameters
        {
            get { return m_BrainParameters; }
        }

        public bool useChildSensors
        {
            get { return m_useChildSensors; }
        }

        public string behaviorName
        {

            get { return m_BehaviorName + "?team=" + m_TeamID;}

        }

        public IPolicy GeneratePolicy(Func<float[]> heuristic)
        {
            switch (m_BehaviorType)
            {
                case BehaviorType.HeuristicOnly:
                    return new HeuristicPolicy(heuristic);
                case BehaviorType.InferenceOnly:
                    return new BarracudaPolicy(m_BrainParameters, m_Model, m_InferenceDevice);
                case BehaviorType.Default:
                    if (Academy.Instance.IsCommunicatorOn)
                    {
                        return new RemotePolicy(m_BrainParameters, behaviorName);
                    }
                    if (m_Model != null)
                    {
                        return new BarracudaPolicy(m_BrainParameters, m_Model, m_InferenceDevice);
                    }
                    else
                    {
                        return new HeuristicPolicy(heuristic);
                    }
                default:
                    return new HeuristicPolicy(heuristic);
            }
        }

        public void GiveModel(
            string behaviorName,
            NNModel model,
            InferenceDevice inferenceDevice = InferenceDevice.CPU)
        {
            m_Model = model;
            m_InferenceDevice = inferenceDevice;
            m_BehaviorName = behaviorName;
        }

        public float GetObservations()
        {
            var nameId = NameChecker.Index(m_BehaviorName);
            return nameId;
        }
    }

    public class NameChecker
    {
        private static NameChecker _instance;
        
        private readonly List<string> _currentStuff;

        private static NameChecker Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NameChecker();
                }

                return _instance;
            }
        }

        public static float Index(string current)
        {
            return Instance._Index(current);
        }
        
        public NameChecker()
        {
            _currentStuff = new List<string>();
        }

        public float _Index(string current)
        {
            if (!_currentStuff.Contains(current))
            {
                _currentStuff.Append(current);
            }
            return _currentStuff.IndexOf(current);
        }
    }
}
