using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace _1._Project.Scripts.Colliders
{
	public class ChocoCollider : MonoBehaviour
	{
		public GameObject ball;
		public GameObject ChocoOutPosition;

		public Transform OutPutTransform;
		public Vector3 pulseScale = new Vector3(1.2f, 1.2f, 1f);
		public float pulseDuration = 0.4f;

		public UnityEvent OnChocoTriggerEnter;

		private Coroutine pulseRoutine;
		private Vector3 baseScale;

		private void Awake()
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
				ball = other.transform.gameObject;
				ball.SetActive(false);
					StartPulseEffect();
				StartCoroutine(WaitToRelease());
			}
		}

		private IEnumerator WaitToRelease()
		{
			yield return new WaitForSeconds(1f);
			ball.transform.position = ChocoOutPosition.transform.position;
			ball.SetActive(true);
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