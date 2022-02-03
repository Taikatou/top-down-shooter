using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Research.CharacterDesign.Scripts.Environment;
using Research.LevelDesign.NuclearThrone.Scripts;
using Research.LevelDesign.Scripts.MLAgents;
using UnityEngine;

namespace Research.CharacterDesign.Scripts
{
    public class WinResults
    {
        public int WinNumber;

        public int LossNumber;

        public int DrawNumber;

        public readonly TeamMember[] TeamMembers;

        public WinResults(int win, int loss, int draw, TeamMember[] teamMembers)
        {
            WinNumber = win;
            LossNumber = loss;
            DrawNumber = draw;
            TeamMembers = teamMembers;
        }
    }

    public class DataLogger : MonoBehaviour
    {
        public bool outputCsv;

        public int maxCount = 100;
        
        private int _counter;

        private DateTime _date;

        private Dictionary<int, WinResults> _winResultsPlayers;

        private static int _dataLoggerCounter;

        private int _logCount;
        

        public void Start()
        {
            _counter = 0;
            _date = DateTime.Now;
            _winResultsPlayers = new Dictionary<int, WinResults>();

            _logCount = _dataLoggerCounter;
            _dataLoggerCounter++;
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
            OutputCsv(rowData, _logCount + "_map_" + mapId);
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

                var outStream = File.CreateText(FolderName + "/" + filename + ".csv");
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
            var firstRow = new List<string> {"MapId", "Win Rate", "Loss Rate", "Draw Rate"};

            for(var i = 0; i < EnvironmentInstance.TeamCount*2; i++)
            {
                var teamId = i / 2;
                var characterId = i % 2;
                firstRow.AddRange(new [] {$"Team{teamId}Character{characterId}", $"Team{teamId}Elo{characterId}"});
            }
            var rowData = new List<string[]> {firstRow.ToArray()};
            foreach (var item in dict)
            {
                var row = new List<string>
                {
                    item.Key.ToString(),
                    item.Value.WinNumber.ToString(),
                    item.Value.LossNumber.ToString(),
                    item.Value.DrawNumber.ToString(),
                };
                foreach (var member in item.Value.TeamMembers)
                {
                    row.AddRange(new[]
                    {
                        member.Type.ToString(),
                        member.TeamID.ToString(),
                        member.SkillRating.ToString()
                    });
                }
                rowData.Add(row.ToArray());
            }

            OutputCsv(rowData, filename);
        }

        public void AddResult(WinLossCondition gameEnding, TeamMember[] teamMember, int mapId)
        {
            var win = gameEnding == WinLossCondition.Win ? 1 : 0;
            var loss = gameEnding == WinLossCondition.Loss ? 1 : 0;
            var draw = gameEnding == WinLossCondition.Draw ? 1 : 0;
            if (_winResultsPlayers.ContainsKey(mapId))
            {
                _winResultsPlayers[mapId].WinNumber += win;
                _winResultsPlayers[mapId].LossNumber += loss;
                _winResultsPlayers[mapId].DrawNumber += draw;
            }
            else
            {
                var output = new WinResults(win, loss, draw, teamMember);
                _winResultsPlayers.Add(mapId, output);
            }

            var text = "";
            foreach (var t in teamMember)
            {
                text += t.Type + "\t" + t.SkillRating;
            }
            Debug.Log(text);

            // Debug.Log(mapId + "\t" + win + "\t" + loss);
        }

        private void Update()
        {
            _counter++;
            if (_counter >= maxCount)
            {
                _counter = 0;
                
                var playerName = $"players_{_date.Hour}_{_date.Minute}_{_date.Second}_{_date.Millisecond}";
                
                PrintFile(_winResultsPlayers, playerName);
            }
        }
    }
}
