using System;
using System.Collections.Generic;
using System.Linq;
using Research.CharacterDesign.Scripts.Environment;
using Research.CharacterDesign.Scripts.SpawnPoints;
using UnityEngine;

namespace Research.LevelDesign.Scripts
{
    public class GetSpawnProcedural : IGetSpawnPoints
    {
        public override MLCheckbox[] Points => GetComponentsInChildren<MLCheckbox>();

        public Dictionary<int, MLCheckbox> PointDict { get; set; }

        public void Start()
        {
            PointDict = new Dictionary<int, MLCheckbox>();
            var index = 0;
            foreach (var point in Points)
            {
                AddPoint(index, point);
                index++;
            }
        }

        public void AddPoint(int playerId, MLCheckbox checkpoint)
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
