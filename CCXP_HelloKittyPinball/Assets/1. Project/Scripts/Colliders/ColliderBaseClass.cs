using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _1._Project.Scripts.Colliders
{
	[RequireComponent(typeof(AudioSource))]
	[RequireComponent(typeof(Collider2D))]
	public class ColliderBaseClass : MonoBehaviour, IPlaySoundOnCollide
	{
		public AudioClip AudioClip;
		private AudioSource _audioSource;
		
		private void Awake()
		{
			_audioSource = GetComponent<AudioSource>();
			_audioSource.playOnAwake = false;
			_audioSource.loop = false;
		}

		private void OnCollisionEnter2D(Collision2D other)
		{
			PlaySound();
		}

		public void PlaySound()
		{
			if (_audioSource.isPlaying)
			{
				_audioSource.Stop();
			}
			_audioSource.clip = AudioClip;
			_audioSource.Play();
		}
	}
}