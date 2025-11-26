using System;
using System.Collections;
using UnityEngine;

namespace _1._Project.Scripts.Colliders
{
	public class HelloKittyTrigger : TriggerBaseClass
	{
		public GameManager GameManager;
		public GameObject HelloKittyOutPosition;
		public float TimeWaiting;
		private GameObject _currentBall;
		public Transform outputPosition;
		public Vector3 MinForce;
		public Vector3 MaxForce;
		
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Player"))
			{
				StartCoroutine(WaitToRelease(other.gameObject));
			}
		}
		
		private IEnumerator WaitToRelease(GameObject ball)
		{
			PlaySoundWithClip(EnterClip);
			ball.SetActive(false);
			GameManager.CreateBall();
			yield return new WaitForSeconds(TimeWaiting);
			ball.transform.position = HelloKittyOutPosition.transform.position;
			ball.transform.position = outputPosition.position;
			ball.SetActive(true);
			PlaySoundWithClip(ExitClip);
			var ballForce = new Vector3(
				x:UnityEngine.Random.Range(MinForce.x, MaxForce.x),
				y:UnityEngine.Random.Range(MinForce.y, MaxForce.y),
				z:UnityEngine.Random.Range(MinForce.z, MaxForce.z));
			ball.GetComponent<Rigidbody2D>().AddRelativeForce(ballForce);
		}
	}
}