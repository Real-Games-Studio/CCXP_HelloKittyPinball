using System;
using Script.Utils;
using UnityEngine;
using UnityEngine.Video;

namespace Script
{
    [RequireComponent(typeof(VideoPlayer))]
    public class VideoController : MonoBehaviour
    {
        private VideoPlayer videoPlayer;

        [Header("Video Player Opitions")] 
        public bool HasToLoop;

        public VideoType VideoTypeToPlay;
        private void Awake()
        {
            videoPlayer = GetComponent<VideoPlayer>();
            SetVideoPlayerOptions();
            videoPlayer.loopPointReached += VideoPlayerOnloopPointReached;
        }

        protected private virtual void VideoPlayerOnloopPointReached(VideoPlayer source)
        {
            
        }

        protected private void StopVideo()
        {
            videoPlayer.Stop();
        }

        protected private void PlayVideo()
        {
            videoPlayer.Play();
        }

        protected private void SetVideoClip(VideoClip videoClip)
        {
            videoPlayer.source = VideoSource.VideoClip;
            videoPlayer.clip = videoClip;
        }

        protected private void SetUrlMp4(string folder, string name)
        {
            videoPlayer.SetVideoPlayerForMp4(folder, name);
        }
        protected private void SetUrlWebm(string folder, string name)
        {
            videoPlayer.SetVideoPlayerForWebm(folder, name);
        }

        protected private void SetVideoPlayerOptions()
        {
            videoPlayer.isLooping = HasToLoop;
        }
        protected private virtual void OnDestroy()
        {
            videoPlayer.loopPointReached -= VideoPlayerOnloopPointReached;
        }
        
        public enum VideoType
        {
            Clip,
            Webm,
            Mp4
        }
    }
}