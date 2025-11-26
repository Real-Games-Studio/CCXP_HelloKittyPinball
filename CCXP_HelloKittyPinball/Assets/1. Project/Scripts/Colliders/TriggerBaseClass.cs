using System;
using UnityEngine;

namespace _1._Project.Scripts.Colliders
{
	[RequireComponent(typeof(AudioSource))]
	[RequireComponent(typeof(Collider2D))]
	public class TriggerBaseClass : MonoBehaviour, IPlaySoundOnCollide
	{
		public AudioClip EnterClip;
		public AudioClip ExitClip;
		private AudioClip _audioClip;
		private AudioSource _audioSource;
		
		
		public void Awake()
		{
			_audioSource = GetComponent<AudioSource>();
			_audioSource.playOnAwake = false;
			_audioSource.loop = false;
		}


		public void PlaySound()
		{
			if (_audioSource.isPlaying)
			{
				_audioSource.Stop();
			}
			_audioSource.clip = _audioClip;
			_audioSource.Play();
		}

		public void PlaySoundWithClip(AudioClip clip)
		{
			if (clip != null)
			{
				_audioClip = clip;
				PlaySound();
				
			}
		}
	}
}