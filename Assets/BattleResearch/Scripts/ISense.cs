﻿using System.Collections.Generic;
using UnityEngine;

namespace BattleResearch.Scripts
{
    public interface ISense
    {
        Dictionary<string, float> GetObservations();
        
        string SenseName { get; }
    }

    public class SenseMethods
    {
        public static void PrintDebug(string name, float [] senses)
        {
            var debug = name + "\t";
            foreach (var ob in senses)
            {
                debug += ob + "\t";
            }
            Debug.Log(debug);
        }
    }
}