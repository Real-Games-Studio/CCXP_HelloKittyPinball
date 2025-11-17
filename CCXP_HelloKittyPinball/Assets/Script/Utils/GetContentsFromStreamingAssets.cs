using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Windows;
using System.IO;
using File = System.IO.File;

namespace Script.Utils
{
    public static class GetContentsFromStreamingAssets
    {
        private static byte[] bytes;
        public static void SetVideoPlayerForMp4(this VideoPlayer videoPlayer, string folder, string name)
        {
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = StreamingAssetsHelper.FilePathMp4(folder, name);
        }
        public static void SetVideoPlayerForWebm(this VideoPlayer videoPlayer, string folder, string name)
        {
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = StreamingAssetsHelper.FilePathWebm(folder, name);
        }

        public static Texture2D GetTexture(string imageFolder, string name)
        {

            bytes = File.ReadAllBytes(StreamingAssetsHelper.FilePath(imageFolder, name));

            var texture2D = new Texture2D(2, 2);
            texture2D.LoadImage(bytes);
            return texture2D;
        }
    }
}