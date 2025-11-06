using System;
using UnityEngine;

namespace _1._Project.Scripts.Collectables
{
	public class BaseCollectable : MonoBehaviour, ICollectable
	{
		public void OnCollected()
		{
			gameObject.SetActive(false);
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