using System;
using _1._Project.Scripts.Colliders;
using UnityEngine;
using UnityEngine.Events;

namespace _1._Project.Scripts.Collectables
{
	public class BaseCollectable : TriggerBaseClass, ICollectable
	{
		public UnityEvent<int> onCollectedEvent;
		public int RowId;
		public void OnCollected()
		{
			gameObject.SetActive(false);
			StopAllCoroutines();
			onCollectedEvent.Invoke(RowId);
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Player"))
			{
				OnCollected();
				PlaySoundWithClip(EnterClip);
			}
		}
	}
}