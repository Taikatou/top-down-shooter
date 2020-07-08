using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Research.CharacterDesign.Scripts.Environment;
using Research.LevelDesign.NuclearThrone.Scripts;
using UnityEngine;
using Application = UnityEngine.Application;

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

        public int maxCount = 100;
        
        private int _counter;

        private DateTime _date;
        
        private Dictionary<int, WinResults> _winResultsTeams;

        private Dictionary<int, WinResults> _winResultsPlayers;
        

        public void Start()
        {
            _counter = 0;
            _date = DateTime.Now;
            _winResultsTeams = new Dictionary<int, WinResults>();
            _winResultsPlayers = new Dictionary<int, WinResults>();
        }

        public void OutputMap(GridSpace[,] map, int mapId)
        {
            var rowData = new List<string[]>();
            var roomHeight = map.GetUpperBound(0);
            var roomWidth = map.GetUpperBound(1);
            
            for (var y = roomHeight; y >= 0; y--)
            {
                var row = new string [roomWidth];
                for (var x = 0; x < roomWidth; x++)
                {
                    row[x] = ((int)map[x, y]).ToString();
                }
                rowData.Add(row);
            }
            OutputCsv(rowData, "map_" + mapId);
        }

        private void OutputCsv(List<string[]> rowData, string filename)
        {
            if (outputCsv)
            {
                var output = new string[rowData.Count][];

                for (var i = 0; i < output.Length; i++)
                {
                    output[i] = rowData[i];
                }

                var length = output.GetLength(0);

                var sb = new StringBuilder();

                for (var index = 0; index < length; index++)
                {
                    sb.AppendLine(string.Join(",", output[index]));
                }

                Directory.CreateDirectory(FolderName);

                var outStream = File.CreateText(FolderName + filename + ".csv");
                outStream.WriteLine(sb);
                outStream.Close();
            }
        }

        private static string FolderName
        {
            get
            {
                #if UNITY_ANDROID
                    return Application.persistentDataPath + "/CSV/";
                #elif UNITY_IPHONE
                    return Application.persistentDataPath + "/CSV/";
                #else
                    return Application.dataPath + "/CSV/";
                #endif
            }
        }

        private void OnApplicationQuit()
        {
            // PrintFile(_winResultsTeams, "teams/teams");
            PrintFile(_winResultsPlayers, "players");
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

            // Debug.Log(mapId + "\t" + win + "\t" + loss);
        }

        public void AddResultTeam(WinLossCondition condition, int mapId)
        {
            AddResult(_winResultsTeams, condition, mapId);
        }

        public void AddResultAgent(WinLossCondition condition, int mapId)
        {
            AddResult(_winResultsPlayers, condition, mapId);
        }

        private void Update()
        {
            _counter++;
            if (_counter >= maxCount)
            {
                _counter = 0;
                
                var playerName = $"{_date.Hour}_{_date.Minute}_{_date.Second}";
                
                PrintFile(_winResultsPlayers, "players_" + playerName);
            }
        }
    }
}
