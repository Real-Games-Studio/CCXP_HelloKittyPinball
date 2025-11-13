using System;
using UnityEngine;

namespace _1._Project.Scripts.Colliders
{
	public class ChocoCollider : MonoBehaviour
	{
		public GameObject ChocoOutPosition;
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Player"))
			{
				other.transform.position = ChocoOutPosition.transform.position;
			}
		}
	}
}