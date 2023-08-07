using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace SillySkills
{
    public class Utils
    {
        private static int savedJsonCount;

        public static void SaveJson<T>(T obj, string filename = "")
        {
            string json = JsonUtility.ToJson(obj, true);
            string jsonPath = Application.persistentDataPath + $"/ALLTHETHINGS2/{filename}{obj}.json";
            savedJsonCount++;

            File.WriteAllText(jsonPath, json);
            Log.Warning("printedjson to " + jsonPath);
        }

        public static void PrintMyCodePlease()
        {
            var skillStats = Assets.LoadJsonFromFile<SkillStats>("AirChannelDashGoodSkillStats.json");

            string log = "printing " + skillStats.GetType();

            FieldInfo[] property_infos = skillStats.GetType().GetFields();

            int padLength = 0;
            for (int i = 0; i < property_infos.Length; i++)
            {
                FieldInfo info = property_infos[i];

                if (info.Name.Length > padLength)
                {
                    padLength = info.Name.Length;
                }
            }

            for (int i = 0; i < property_infos.Length; i++)
            {
                FieldInfo info = property_infos[i];

                //log += ($"\nskillStats.{info.Name} = new {GetCodeTypeString(info.GetValue(skillStats))}nig[attackInfos.Length];").Replace("[]nig", "");
                log += ($"\nskillStats.{info.Name}[i] = attackInfo.{info.Name};");
            }

            Log.Message(log);
        }

        private static object GetCodeTypeString(object infoValue)
        {
            string infoString = infoValue.GetType().ToString();
            infoString = infoString.Replace("System.String", "string");
            infoString = infoString.Replace("System.Boolean", "bool");
            infoString = infoString.Replace("System.Int32", "int");
            infoString = infoString.Replace("System.Single", "float");
            return infoString;
        }

        public static void printAllFields<T>(T obj, bool saveJson = false)
        {
            string log = "printing " + obj.GetType();

            FieldInfo[] property_infos = obj.GetType().GetFields();

            int padLength = 0;
            for (int i = 0; i < property_infos.Length; i++)
            {
                FieldInfo info = property_infos[i];

                if (info.Name.Length > padLength)
                {
                    padLength = info.Name.Length;
                }
            }


            for (int i = 0; i < property_infos.Length; i++)
            {
                FieldInfo info = property_infos[i];

                log += ($"\n{info.Name.PadLeft(padLength, '-')}| {info.GetValue(obj)}");
            }

            Log.Message(log);

            if (saveJson)
                SaveJson(obj);
        }

        public static void PrintStatData(StatData self)
        {
            Log.Warning("Printing all stat data");

            foreach (KeyValuePair<string, object> item in self.statDict)
            {
                Log.Warning($"{item.Key}:");

                if (item.Value is List<float>)
                {
                    PrintList<float>(item.Value as List<float>);
                }
                if (item.Value is List<int>)
                {
                    PrintList<int>(item.Value as List<int>);
                }
                if (item.Value is List<bool>)
                {
                    PrintList<bool>(item.Value as List<bool>);
                }
                if (item.Value is List<string>)
                {
                    PrintList<string>(item.Value as List<string>);
                }
            }
        }

        public static void PrintList<T>(List<T> list)
        {
            Log.Warning("Printing list. Count: " + list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                Log.Warning(list[i]);
            }
        }

        //credit to komradespectre
        public static List<Vector3> DistributePointsEvenlyAroundCircle(int points, float radius, Vector3 origin)
        {
            List<Vector3> pointsList = new List<Vector3>();
                double theta = (Math.PI * 2) / points;
            for (int i = 0; i < points; i++)
            {
                double angle = theta * i;
                Vector3 positionChosen = new Vector3((float)(radius * Math.Cos(angle) + origin.x), (float)(radius * Math.Sin(angle) + origin.y), origin.z);
                pointsList.Add(positionChosen);
            }

            return pointsList;
        }
    }
}
