using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace _1._Project.Scripts.Colliders
{
	public class ChocoTrigger : TriggerBaseClass
	{
		public GameObject ChocoOutPosition1;
		public GameObject ChocoOutPosition2;
		public Vector3 Force;

		public Transform OutPutTransform;
		public Vector3 pulseScale = new Vector3(1.2f, 1.2f, 1f);
		public float pulseDuration = 0.4f;

		public UnityEvent OnChocoTriggerEnter;

		private Coroutine pulseRoutine;
		private Vector3 baseScale;

		public void Start()
		{
			if (OutPutTransform != null)
			{
				baseScale = OutPutTransform.localScale;
			}
		}

		private void OnDisable()
		{
			StopPulseEffect();
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Player"))
			{
				OnChocoTriggerEnter?.Invoke();
				StartCoroutine(WaitToRelease(other.transform.gameObject));
			}
		}

		private IEnumerator WaitToRelease(GameObject ball)
		{
			PlaySoundWithClip(EnterClip);
			ball.SetActive(false);
			StartPulseEffect();
			yield return new WaitForSeconds(1f);
			var r = Random.Range(0, 2);
			var ChocoOutPosition = r == 0 ? ChocoOutPosition1 : ChocoOutPosition2;
			ball.transform.position = ChocoOutPosition.transform.position;
			ball.transform.localEulerAngles = Vector3.zero;
			ball.SetActive(true);
			PlaySoundWithClip(ExitClip);
			ball.GetComponent<Rigidbody2D>().AddRelativeForce(Force);
			StopPulseEffect();
		}

		private void StartPulseEffect()
		{
			if (OutPutTransform == null)
			{
				return;
			}

			if (pulseRoutine != null)
			{
				StopCoroutine(pulseRoutine);
			}

			pulseRoutine = StartCoroutine(PulseRoutine());
		}

		private void StopPulseEffect()
		{
			if (pulseRoutine != null)
			{
				StopCoroutine(pulseRoutine);
				pulseRoutine = null;
			}

			if (OutPutTransform != null)
			{
				OutPutTransform.localScale = baseScale;
			}
		}

		private IEnumerator PulseRoutine()
		{
			if (OutPutTransform == null)
			{
				yield break;
			}

			if (baseScale == Vector3.zero)
			{
				baseScale = OutPutTransform.localScale;
			}

			float elapsed = 0f;
			while (true)
			{
				while (elapsed < pulseDuration)
				{
					float t = Mathf.PingPong(elapsed, pulseDuration) / pulseDuration;
					OutPutTransform.localScale = Vector3.Lerp(baseScale, pulseScale, t);
					elapsed += Time.deltaTime;
					yield return null;
				}

				elapsed = 0f;
			}
		}
	}
}