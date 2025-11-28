using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace _1._Project.Scripts.Colliders
{
	public class ChocoTrigger : TriggerBaseClass
	{
		
		[SerializeField] private int scoreValue = 10;
		[SerializeField] private float timeIfHitMultiplieScore = 1.5f; // valor decresivo, caso o objeto seja acertado novamente em um curto espaço de tempo a pontuação será maior (x2)
		[SerializeField] private GameObject hitEffectPrefab; // esse prefab deve ser uma particula, e ela pode conter o ParticleTextController

		public GameObject ChocoOutPosition1;
		public GameObject ChocoOutPosition2;
		public Vector3 Force;

		public Transform OutPutTransform;
		public Vector3 pulseScale = new Vector3(1.2f, 1.2f, 1f);
		public float pulseDuration = 0.4f;

		public UnityEvent OnChocoTriggerEnter;

		private Coroutine pulseRoutine;
		private Vector3 baseScale;
		private float lastHitTime = float.NegativeInfinity;
		private bool hasBeenHit;

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
				if (ball != null)
				{
					
				var r = Random.Range(0, 2);
				var ChocoOutPosition = r == 0 ? ChocoOutPosition1 : ChocoOutPosition2;
				ball.transform.position = ChocoOutPosition.transform.position;
				ball.transform.localEulerAngles = Vector3.zero;
				ball.SetActive(true);
				PlaySoundWithClip(ExitClip);
				ball.GetComponent<Rigidbody2D>().AddRelativeForce(Force);
				
				int awardedScore = CalculateScore();
			
				if (ScoreManager.Instance != null)
				{
					ScoreManager.Instance.AddScore(awardedScore);
				}
				SpawnHitEffect(awardedScore);
			}
			StopPulseEffect();
		}

		
		private int CalculateScore()
		{
			float now = Time.time;
			float multiplier = 1f;

			if (hasBeenHit)
			{
				float elapsed = now - lastHitTime;
				if (elapsed <= 0f)
				{
					multiplier = 2f;
				}
				else if (timeIfHitMultiplieScore > 0f && elapsed < timeIfHitMultiplieScore)
				{
					float normalized = Mathf.Clamp01(elapsed / timeIfHitMultiplieScore);
					multiplier = Mathf.Lerp(2f, 1f, normalized); // decaimento linear do multiplicador
				}
			}

			hasBeenHit = true;
			lastHitTime = now;

			return Mathf.RoundToInt(scoreValue * multiplier);
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
		
		private void SpawnHitEffect(int awardedScore)
		{
			if (!hitEffectPrefab)
			{
				return;
			}

			Vector3 spawnPosition;
				spawnPosition = new Vector3(OutPutTransform.transform.position.x, OutPutTransform.transform.position.y, -1f);

			GameObject effectInstance = Instantiate(hitEffectPrefab, spawnPosition, this.transform.rotation);

			ParticleTextController textController = effectInstance.GetComponentInChildren<ParticleTextController>();
			if (textController != null)
			{
				textController.SetPointText(awardedScore);
			}

			if (!TryScheduleDestroy(effectInstance))
			{
				Destroy(effectInstance, 2f);
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
		
		
		private bool TryScheduleDestroy(GameObject effectInstance)
		{
			ParticleSystem[] particleSystems = effectInstance.GetComponentsInChildren<ParticleSystem>();
			if (particleSystems.Length == 0)
			{
				return false;
			}

			float maxLifetime = 0f;
			foreach (ParticleSystem particleSystem in particleSystems)
			{
				ParticleSystem.MainModule main = particleSystem.main;
				float lifetime = main.duration + main.startLifetime.constantMax;
				if (lifetime > maxLifetime)
				{
					maxLifetime = lifetime;
				}
			}

			Destroy(effectInstance, maxLifetime);
			return true;
		}
	}
}