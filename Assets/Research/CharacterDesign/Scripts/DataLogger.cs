using System;
using System.Collections.Generic;
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
        public bool outputCsv;
        
        private Dictionary<int, WinResults> _winResultsTeams;

        private Dictionary<int, WinResults> _winResultsPlayers;

        public void Start()
        {
            _winResultsTeams = new Dictionary<int, WinResults>();
            _winResultsPlayers = new Dictionary<int, WinResults>();
        }

        private static string GetFileName(string fileName)
        {
            var nowStr = DateTime.Now.ToString("_dd_MM_yyyy_HH_mm");
            return fileName + nowStr + ".csv";
        }

        public void OutputMap(List<string[]> rowData, int mapId)
        {
            OutputCsv(rowData, "mapdata/map_" + mapId);
        }

        private void OutputCsv(List<string[]> rowData, string filename)
        {
            var output = new string[rowData.Count][];

            for (var i = 0; i < output.Length; i++)
            {
                output[i] = rowData[i];
            }

            var length = output.GetLength(0);
            var delimiter = ",";

            var sb = new StringBuilder();

            for (var index = 0; index < length; index++)
            {
                sb.AppendLine(string.Join(delimiter, output[index]));   
            }

            var filePath = GetPath(filename);

            var outStream = System.IO.File.CreateText(filePath);
            outStream.WriteLine(sb);
            outStream.Close();
        }

        // Following method is used to retrive the relative path as device platform
        private static string GetPath(string fileName)
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

        private void OnApplicationQuit()
        {
            PrintFile(_winResultsTeams, "teams/teams");
            PrintFile(_winResultsPlayers, "players/players");
        }

        private void PrintFile(Dictionary<int, WinResults> dict, string filename)
        {
            var rowData = new List<string[]> {new[] {"MapId", "Win Rate", "Loss Rate", "Draw Rate"}};
            foreach (var item in dict)
            {
                var row = new[]
                {
                    item.Key.ToString(),
                    item.Value.WinNumber.ToString(),
                    item.Value.LossNumber.ToString(),
                    item.Value.DrawNumber.ToString(),
                };
                rowData.Add(row);
            }

            OutputCsv(rowData, filename);
        }

        private void AddResult(Dictionary<int, WinResults> dict, WinLossCondition gameEnding, int mapId)
        {
            var win = gameEnding == WinLossCondition.Win ? 1 : 0;
            var loss = gameEnding == WinLossCondition.Loss ? 1 : 0;
            var draw = gameEnding == WinLossCondition.Draw ? 1 : 0;
            if (dict.ContainsKey(mapId))
            {
                dict[mapId].WinNumber += win;
                dict[mapId].LossNumber += loss;
                dict[mapId].DrawNumber += draw;
            }
            else
            {
                var output = new WinResults(win, loss, draw);
                dict.Add(mapId, output);
            }

            Debug.Log(mapId + "\t" + win + "\t" + loss);
        }

        public void AddResultTeam(WinLossCondition condition, int mapId)
        {
            AddResult(_winResultsTeams, condition, mapId);
        }

        public void AddResultAgent(WinLossCondition condition, int mapId)
        {
            AddResult(_winResultsPlayers, condition, mapId);
        }
    }
}
