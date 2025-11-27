using System;
using UnityEngine;

namespace _1._Project.Scripts.Colliders
{
	public class CollectablesTrigger : TriggerBaseClass
	{
		private void OnTriggerEnter2D(Collider2D other)
		{

			if (other.CompareTag("Player"))
			{
				Debug.Log(other.name);
				PlaySoundWithClip(EnterClip);
			}
		}
	}
}