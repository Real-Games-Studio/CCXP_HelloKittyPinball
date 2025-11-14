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
		private bool baseScaleCached;

		private void Awake()
		{
			CacheBaseScale();
		}

		private void OnEnable()
		{
			CacheBaseScale();
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

			if (OutPutTransform != null && baseScaleCached)
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

			CacheBaseScale();

			float elapsed = 0f;
			float duration = Mathf.Max(0.01f, pulseDuration);

			while (true)
			{
				elapsed += Time.deltaTime;
				float normalized = Mathf.PingPong(elapsed, duration) / duration;
				OutPutTransform.localScale = Vector3.Lerp(baseScale, pulseScale, normalized);
				yield return null;
			}
		}

		private void CacheBaseScale()
		{
			if (OutPutTransform == null)
			{
				return;
			}

			baseScale = OutPutTransform.localScale;
			baseScaleCached = true;
		}
	}
}