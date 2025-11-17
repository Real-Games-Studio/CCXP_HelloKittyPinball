using UnityEngine;
using UnityRawInput;

namespace Script.Utils
{
    public static class StreamingAssetsHelper
    {
        public static string FilePathWav(string folder, string fileName) => FilePath(folder, fileName, "wav");
        public static string FilePathMp3(string folder, string fileName) => FilePath(folder, fileName, "mp3");
        public static string FilePathJson(string folder, string fileName) => FilePath(folder, fileName, "json");
        public static string FilePathWebm(string folder, string fileName) => FilePath(folder, fileName, "webm");
        public static string FilePathMp4(string folder, string fileName) => FilePath(folder, fileName, "mp4");
        public static string FilePathPng(string folder, string fileName) => FilePath(folder, fileName, "png");

        public static string FilePath(string folder, string fileName)
        {
            return $"{Application.streamingAssetsPath}/{folder}/{fileName}";
        }
        private static string FilePath(string folder, string fileName, string fileExtension)
        {
            return $"{Application.streamingAssetsPath}/{folder}/{fileName}.{fileExtension}";
        }
        public static KeyCode GetKeyCodeFromChar(this char name)
        {
            return (KeyCode) System.Enum.Parse(typeof(KeyCode), name.ToString());
        }
        public static RawKey GetRawKeyFromChar(this char name)
        {
            return (RawKey) System.Enum.Parse(typeof(RawKey), name.ToString());
        }

    }
}