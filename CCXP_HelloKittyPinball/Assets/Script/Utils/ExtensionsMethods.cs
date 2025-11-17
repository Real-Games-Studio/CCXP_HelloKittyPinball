using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Utils
{
    public static class ExtensionsMethods
    {
        public static int ReturnFirstActiveFromList(this List<GameObject> gameObjects)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (gameObjects[i].activeSelf)
                {
                    return i;
                }
            }
            return 0;
        }

        public static T RandomFromList<T>(this List<T> list)
        {
            var randomIndex = Random.Range(0, list.Count);
            return list[randomIndex];
        }
        public static void DeactivateAllObjects(this List<GameObject> gameObjects)
        {
            foreach (var gameObject in gameObjects)
            {
                gameObject.SetActive(false);
            }
        }
        
        public static Color32 SetImageColor(this Image image)
        {
            return image.color;
        }

        public static KeyCode CharToKeyCode(this char c)
        {
            return (KeyCode) System.Enum.Parse(typeof(KeyCode), c.ToString());
        }

    }
}