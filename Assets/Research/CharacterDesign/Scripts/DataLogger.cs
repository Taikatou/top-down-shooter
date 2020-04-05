﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Research.CharacterDesign.Scripts.Environment;
using UnityEngine;

namespace Research.CharacterDesign.Scripts
{
    public class WinResults
    {
        public int WinNumber;

        public int LossNumber;

        public int DrawNumber;

        public WinResults(int win, int loss, int draw)
        {
            WinNumber = win;
            LossNumber = loss;
            DrawNumber = draw;
        }
    }

    public class DataLogger : MonoBehaviour
    {
        private Dictionary<string, WinResults> winResultsTeams;

        private Dictionary<string, WinResults> winResultsPlayers;

        public void Start()
        {
            winResultsTeams = new Dictionary<string, WinResults>();
            winResultsPlayers = new Dictionary<string, WinResults>();
        }

        public string GetFileName(string fileName)
        {
            var nowStr = DateTime.Now.ToString("_dd_MM_yyyy_HH_mm");
            return fileName + nowStr + ".csv";
        }

        public void OutputCsv(List<string[]> rowData, string filename)
        {
            string[][] output = new string[rowData.Count][];

            for (int i = 0; i < output.Length; i++)
            {
                output[i] = rowData[i];
            }

            int length = output.GetLength(0);
            string delimiter = ",";

            StringBuilder sb = new StringBuilder();

            for (int index = 0; index < length; index++)
                sb.AppendLine(string.Join(delimiter, output[index]));


            string filePath = GetPath(filename);

            StreamWriter outStream = System.IO.File.CreateText(filePath);
            outStream.WriteLine(sb);
            outStream.Close();
        }

        // Following method is used to retrive the relative path as device platform
        private string GetPath(string fileName)
        {
#if UNITY_EDITOR
            return Application.dataPath + "/CSV/" + GetFileName(fileName);
#elif UNITY_ANDROID
            return Application.persistentDataPath+"Saved_data.csv";
#elif UNITY_IPHONE
            return Application.persistentDataPath+"/"+"Saved_data.csv";
#else
        return Application.dataPath + "/" + "Saved_data.csv";
#endif
        }

        void OnApplicationQuit()
        {
            PrintFile(winResultsTeams, "teams");
            PrintFile(winResultsPlayers, "players");
        }

        private void PrintFile(Dictionary<string, WinResults> dict, string filename)
        {
            var rowData = new List<string[]> {new[] {"Name", "Win Rate", "Loss Rate", "Draw Rate"}};
            foreach (KeyValuePair<string, WinResults> item in winResultsTeams)
            {
                var row = new[]
                {
                    item.Key,
                    item.Value.WinNumber.ToString(),
                    item.Value.LossNumber.ToString(),
                    item.Value.DrawNumber.ToString()
                };
                rowData.Add(row);
            }

            OutputCsv(rowData, filename);
        }

        private void AddResult(Dictionary<string, WinResults> dict, string resultName, int gameEnding)
        {
            var win = gameEnding == 1 ? 1 : 0;
            var loss = gameEnding == -1 ? 1 : 0;
            var draw = gameEnding == 0 ? 1 : 0;
            if (dict.ContainsKey(resultName))
            {
                dict[resultName].WinNumber += win;
                dict[resultName].LossNumber += loss;
                dict[resultName].DrawNumber += draw;
            }
            else
            {
                var output = new WinResults(win, loss, draw);
                dict.Add(resultName, output);
            }

            Debug.Log(resultName + "\t" + win + "\t" + loss);
        }

        public void AddResultTeam(string teamName, int condition)
        {
            AddResult(winResultsTeams, teamName, condition);
        }

        public void AddResultAgent(string agentName, int condition)
        {
            AddResult(winResultsPlayers, agentName, condition);
        }
    }
}