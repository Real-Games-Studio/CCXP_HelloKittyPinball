using System;
using System.Collections;
using UnityEngine;

namespace _1._Project.Scripts.Colliders
{
	public class HelloKittyCollider : MonoBehaviour
	{
		public GameObject HelloKittyOutPosition;
		public float TimeWaiting;
		private GameObject _currentBall;
		
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Player"))
			{
				_currentBall = other.gameObject;
				StartCoroutine(WaitToRelease());
			}
		}
		
		private IEnumerator WaitToRelease()
		{
			_currentBall.SetActive(false);
			yield return new WaitForSeconds(TimeWaiting);
			_currentBall.transform.position = HelloKittyOutPosition.transform.position;
			_currentBall.SetActive(true);
		}
	}
}