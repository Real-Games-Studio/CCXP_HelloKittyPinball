using System.IO;
using UnityEngine;

namespace Script
{
    public static class LoadJsonExemple
    {
        private static bool hasBeenLoaded;
        public static int speedIncrement;
        public static int minimalSpawnTime;
        public static int maximumSpawnTime;
        public static float recyclablesSpeed;
        public static int chanceOfSpawnPlastic;
        public static float animationTime;
        

        public static void LoadAllVariables()
        {
            if (hasBeenLoaded)
            {
                return;
            }

            var filePath = Path.Combine(Application.streamingAssetsPath + "/", "Variaveis.json");
            if (File.Exists(filePath))
            {
                var dataJson = File.ReadAllText(filePath);
                //var loadedData = JsonUtility.FromJson<JsonData>(dataJson);

            }
        }
    }
}