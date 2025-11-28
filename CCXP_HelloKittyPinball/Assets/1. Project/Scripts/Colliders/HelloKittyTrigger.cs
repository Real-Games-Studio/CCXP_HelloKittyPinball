using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
		public Volume Profile;
		public AnimationCurve BloomAnimationCurve;
		
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Player"))
			{
				StartCoroutine(WaitToRelease(other.gameObject));
			}
			
			Bloom b;
			Profile.profile.TryGet<Bloom>(out b);

			if (b != null)
			{
				StartCoroutine(AlterBloom(b));
			}
		}
		
		private IEnumerator WaitToRelease(GameObject ball)
		{
			PlaySoundWithClip(EnterClip);
			ball.SetActive(false);
			GameManager.CreateBall();
			yield return new WaitForSeconds(TimeWaiting);
			if (ball != null)
			{
				ball.transform.position = HelloKittyOutPosition.transform.position;
				ball.transform.position = outputPosition.position;
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

		private IEnumerator AlterBloom(Bloom b)
		{
			float timecounter = 0.0f;
			while (timecounter < 2.0f)
			{
				b.intensity.value = BloomAnimationCurve.Evaluate(timecounter);
				timecounter += Time.deltaTime;
				yield return new WaitForEndOfFrame();
				
			}
        }

	}
}