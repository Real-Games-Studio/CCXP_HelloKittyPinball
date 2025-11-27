using UnityEngine;
using System.IO;

namespace RealGames
{
    public static class JsonLoader
    {
        public static AppConfig appConfig = null;
        public static AppConfig LoadGameSettings(string jsonFilePath)
        {
            if (File.Exists(jsonFilePath))
            {
                if (appConfig == null)
                {
                    string json = File.ReadAllText(jsonFilePath);
                    AppConfig settings = JsonUtility.FromJson<AppConfig>(json);
                    appConfig = settings;
                    return settings;
                }
                else
                {
                    return appConfig;
                }
            }
            else
            {
                Debug.LogError("JSON file not found at: " + jsonFilePath);
                return null;
            }
        }
    }

}
