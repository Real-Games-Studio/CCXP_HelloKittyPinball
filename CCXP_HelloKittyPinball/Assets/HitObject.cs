using System.Collections;
using System.Collections.Generic;
using SgLib;
using UnityEngine;
using UnityEngine.Events;

public class HitObject : MonoBehaviour
{
    private const string PlayerTag = "Player";

    [SerializeField] private int scoreValue = 10;
    [SerializeField] private float timeIfHitMultiplieScore = 1.5f; // valor decresivo, caso o objeto seja acertado novamente em um curto espaço de tempo a pontuação será maior (x2)
    [SerializeField] private GameObject hitEffectPrefab; // esse prefab deve ser uma particula, e ela pode conter o ParticleTextController
    [SerializeField] private bool spawnEffectOnHitPosition = true;

    [SerializeField] private Transform hitObjectTransform;
    [SerializeField] private float maxSizeEffect = 1.5f; // tamanho máximo do efeito de hit
    [SerializeField] private float minSizeEffect = 0.5f; // tamanho mínimo do efeito de hit
    [SerializeField] private float scaleResetDuration = 0.2f;

    [SerializeField] private UnityEvent onHit = new UnityEvent(); // evento para quando o objeto for acertado
    [SerializeField] private UnityEvent onEnter = new UnityEvent(); // evento para quando a bola entrar em contato com o objeto
    [SerializeField] private UnityEvent onExit = new UnityEvent(); // evento para quando a bola sair do contato com o objeto

    private readonly HashSet<int> activePlayerColliders = new HashSet<int>();
    private Rigidbody2D cachedRigidbody2D;
    private float lastHitTime = float.NegativeInfinity;
    private bool hasBeenHit;
    private bool scoreManagerMissingLogged;
    private Coroutine scaleRoutine;
    private Vector3 baseLocalScale;
    private bool hasBaseLocalScale;

    private void Awake()
    {
        if (!hitObjectTransform)
        {
            hitObjectTransform = transform;
        }

        if (hitObjectTransform)
        {
            baseLocalScale = hitObjectTransform.localScale;
            hasBaseLocalScale = true;
        }

        TryGetComponent(out cachedRigidbody2D);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsPlayer(collision.collider.gameObject))
        {
            return;
        }

        Vector2 contactPoint = collision.contactCount > 0 ? collision.GetContact(0).point : (Vector2)hitObjectTransform.position;
        Vector3 hitPoint = new Vector3(contactPoint.x, contactPoint.y, hitObjectTransform.position.z);
        float relativeSpeed = collision.relativeVelocity.magnitude;

        TryHandleHit(collision.collider.GetInstanceID(), collision.collider.gameObject, hitPoint, relativeSpeed);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!IsPlayer(collision.collider.gameObject))
        {
            return;
        }

        UnregisterPlayerContact(collision.collider.GetInstanceID());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayer(other.gameObject))
        {
            return;
        }

        Vector2 origin = hitObjectTransform ? (Vector2)hitObjectTransform.position : (Vector2)transform.position;
        Vector2 contactPoint = other.ClosestPoint(origin);
        Vector3 hitPoint = new Vector3(contactPoint.x, contactPoint.y, hitObjectTransform.position.z);
        float relativeSpeed = EstimateRelativeSpeed(other);

        TryHandleHit(other.GetInstanceID(), other.gameObject, hitPoint, relativeSpeed);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayer(other.gameObject))
        {
            return;
        }

        UnregisterPlayerContact(other.GetInstanceID());
    }

    private void OnDisable()
    {
        ResetScaleEffect();

        if (activePlayerColliders.Count == 0)
        {
            return;
        }

        activePlayerColliders.Clear();
        onExit.Invoke();
    }

    private void TryHandleHit(int colliderId, GameObject other, Vector3 hitPoint, float relativeSpeed)
    {
        if (!RegisterPlayerContact(colliderId))
        {
            return;
        }

        int awardedScore = CalculateScore();

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(awardedScore);
        }
        else if (!scoreManagerMissingLogged)
        {
            Debug.LogWarning($"{nameof(HitObject)}: ScoreManager.Instance not found, score will not be recorded.");
            scoreManagerMissingLogged = true;
        }

        float scaleMultiplier = CalculateImpactScale(relativeSpeed);
        ApplyScaleEffect(scaleMultiplier);

        SpawnHitEffect(hitPoint, awardedScore);

        onHit.Invoke();
    }

    private bool RegisterPlayerContact(int colliderId)
    {
        bool isNewContact = activePlayerColliders.Add(colliderId);
        if (!isNewContact)
        {
            return false;
        }

        if (activePlayerColliders.Count == 1)
        {
            onEnter.Invoke();
        }

        return true;
    }

    private void UnregisterPlayerContact(int colliderId)
    {
        if (!activePlayerColliders.Remove(colliderId))
        {
            return;
        }

        if (activePlayerColliders.Count == 0)
        {
            onExit.Invoke();
        }
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

    private void SpawnHitEffect(Vector3 hitPoint, int awardedScore)
    {
        if (!hitEffectPrefab)
        {
            return;
        }

        Vector3 spawnPosition;
        if (spawnEffectOnHitPosition)
        {
            spawnPosition = new Vector3(hitPoint.x, hitPoint.y, -1f);
        }
        else
        {
            Vector3 center = hitObjectTransform ? hitObjectTransform.position : transform.position;
            spawnPosition = new Vector3(center.x, center.y, -1f);
        }

        Quaternion rotation = hitObjectTransform ? hitObjectTransform.rotation : Quaternion.identity;
        GameObject effectInstance = Instantiate(hitEffectPrefab, spawnPosition, rotation);

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

    private bool IsPlayer(GameObject other)
    {
        return other.CompareTag(PlayerTag);
    }

    private float EstimateRelativeSpeed(Collider2D other)
    {
        if (other.attachedRigidbody == null)
        {
            return 0f;
        }

        Vector2 otherVelocity = other.attachedRigidbody.linearVelocity;
        Vector2 thisVelocity = cachedRigidbody2D != null ? cachedRigidbody2D.linearVelocity : Vector2.zero;
        return (otherVelocity - thisVelocity).magnitude;
    }

    private float CalculateImpactScale(float relativeSpeed)
    {
        float lower = Mathf.Min(minSizeEffect, maxSizeEffect);
        float upper = Mathf.Max(minSizeEffect, maxSizeEffect);

        if (Mathf.Approximately(upper, lower))
        {
            return Mathf.Max(0.01f, lower);
        }

        float speedFactor = Mathf.Clamp01(relativeSpeed / 15f);
        float multiplier = Mathf.Lerp(lower, upper, speedFactor);
        return Mathf.Max(0.01f, multiplier);
    }

    private void ApplyScaleEffect(float multiplier)
    {
        if (!hasBaseLocalScale || !hitObjectTransform)
        {
            return;
        }

        multiplier = Mathf.Max(0.01f, multiplier);

        if (scaleRoutine != null)
        {
            StopCoroutine(scaleRoutine);
        }

        if (gameObject.activeInHierarchy)
        {
            scaleRoutine = StartCoroutine(ScaleEffectRoutine(multiplier));
        }
    }

    private IEnumerator ScaleEffectRoutine(float targetMultiplier)
    {
        Vector3 targetScale = baseLocalScale * targetMultiplier;
        float duration = Mathf.Max(0.01f, scaleResetDuration);

        hitObjectTransform.localScale = targetScale;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            hitObjectTransform.localScale = Vector3.Lerp(targetScale, baseLocalScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        hitObjectTransform.localScale = baseLocalScale;
        scaleRoutine = null;
    }

    private void ResetScaleEffect()
    {
        if (scaleRoutine != null)
        {
            StopCoroutine(scaleRoutine);
            scaleRoutine = null;
        }

        if (hasBaseLocalScale && hitObjectTransform)
        {
            hitObjectTransform.localScale = baseLocalScale;
        }
    }
}
