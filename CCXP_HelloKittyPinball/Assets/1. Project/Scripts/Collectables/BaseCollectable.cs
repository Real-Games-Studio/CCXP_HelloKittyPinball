using System;
using UnityEngine;
using UnityEngine.Events;

namespace _1._Project.Scripts.Collectables
{
	public class BaseCollectable : MonoBehaviour, ICollectable
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
			}
		}
	}
}