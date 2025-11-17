using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Script.Utils
{
    public static class LoadAudioFromStreamingAssets
    {
        public static async void LoadWavClip(string path, AudioClip clip) => await LoadClip(path,clip, AudioType.WAV);
        public static async Task LoadMp3Clip(string path, AudioClip clip) => await LoadClip(path,clip, AudioType.MPEG);
        private static async Task<AudioClip> LoadClip(string path, AudioClip clip, AudioType audioType)
        {
            using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, audioType))
            {
                uwr.SendWebRequest();

                // wrap tasks in try/catch, otherwise it'll fail silently
                try
                {
                    while (!uwr.isDone) await Task.Delay(5);

                    if (uwr.isNetworkError || uwr.isHttpError) Debug.Log($"{uwr.error}");
                    else
                    {
                        clip = DownloadHandlerAudioClip.GetContent(uwr);
                        clip.name = path.Split("/").Last();
                    }
                }
                catch (Exception err)
                {
                    Debug.Log($"{err.Message}, {err.StackTrace}");
                }
            }

            return clip;
        }
    }
}