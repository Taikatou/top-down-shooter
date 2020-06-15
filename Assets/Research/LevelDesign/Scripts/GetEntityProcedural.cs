using System;
using System.Collections.Generic;
using Research.CharacterDesign.Scripts.SpawnPoints;
using UnityEngine;

namespace Research.LevelDesign.Scripts
{
    public abstract class GetEntityProcedural<T> : IGetSpawnPoints<T>
    {
        public GameObject entityPrefab;

        private Dictionary<int, T> _pointDict;

        private bool set;
        
        public override T[] Points => GetComponentsInChildren<T>();

        private void Start()
        {
            SetPoints();
        }

        public Dictionary<int, T> PointDict
        {
            get
            {
                SetPoints();

                return _pointDict;
            }
        }

        private void SetPoints()
        {
            if (!set)
            {
                _pointDict = new Dictionary<int, T>();
                set = true;
                var index = 0;
                foreach (var point in Points)
                {
                    AddPoint(index, point);
                    index++;
                }
            }
        }

        public void AddPoint(int playerId, T checkpoint)
        {
            if (Application.isPlaying)
            {
                var contains = PointDict.ContainsKey(playerId);
                if (!contains)
                {
                    PointDict.Add(playerId, checkpoint);
                }
                else
                {
                    PointDict[playerId] = checkpoint;
                }   
            }
        }
    }
}
