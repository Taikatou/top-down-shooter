using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace BattleResearch.Scripts
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
        private Dictionary<string, WinResults> winResults;

        public void Start()
        {
            winResults = new Dictionary<string, WinResults>();
        }

        public string GetFileName(string fileName)
        {
            var nowStr = DateTime.Now.ToString("_dd_MM_yyyy_HH_mm");
            return fileName + nowStr + ".csv";
        }

        public void OutputCsv(List<string[]> rowData)
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


            string filePath = GetPath("analysis.csv");

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
            var rowData = new List<string[]> {new[] {"Name", "Win Rate", "Loss Rate", "Draw Rate"}};
            foreach (KeyValuePair<string, WinResults> item in winResults)
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

            OutputCsv(rowData);
        }

        public void AddResult(string name, int win, int loss, int draw)
        {
            if (winResults.ContainsKey(name))
            {
                winResults[name].WinNumber += win;
                winResults[name].LossNumber += loss;
                winResults[name].DrawNumber += draw;
            }
            else
            {
                var output = new WinResults(win, loss, draw);
                winResults.Add(name, output);
            }
            Debug.Log(name + "\t" + win + "\t" + loss);
        }
    }
}
