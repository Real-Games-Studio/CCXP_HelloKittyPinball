using System.Collections;
using UnityEngine;

namespace _1._Project.Scripts
{
	public class StartCollider : MonoBehaviour
	{
		public GameObject ballCollider;
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Player"))
			{
				other.GetComponent<BallController>().MaxSpeedReset();
			}
		}

		private void OnCollisionEnter2D(Collision2D other)
		{
			if (other.gameObject.CompareTag("Player"))
			{
				other.gameObject.GetComponent<BallController>().MaxSpeedReset();
			}
		}

		
	}
}