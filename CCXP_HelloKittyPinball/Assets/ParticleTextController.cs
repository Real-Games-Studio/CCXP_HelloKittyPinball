using TMPro;
using UnityEngine;

public class ParticleTextController : MonoBehaviour
{
    [SerializeField] private TMP_Text pointText;
    [SerializeField] private Transform animatedTransform;
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private float riseDistance = 1f;
    [SerializeField] private float horizontalAmplitude = 0.5f;
    [SerializeField] private AnimationCurve verticalCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] private AnimationCurve alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    private Coroutine animateRoutine;
    private Color baseColor = Color.white;
    private Vector3 baseLocalPosition;
    private bool initialized;

    public void SetPointText(int points)
    {
        EnsureInitialized();

        if (pointText == null)
        {
            return;
        }

        pointText.text = "+" + points.ToString();

        if (animateRoutine != null)
        {
            StopCoroutine(animateRoutine);
        }

        animateRoutine = StartCoroutine(AnimateText());
    }
    
    private void Awake()
    {
        EnsureInitialized();
    }

    private void OnDisable()
    {
        if (animateRoutine != null)
        {
            StopCoroutine(animateRoutine);
            animateRoutine = null;
        }

        if (!initialized)
        {
            return;
        }

        animatedTransform.localPosition = baseLocalPosition;
        pointText.color = baseColor;
    }

    private System.Collections.IEnumerator AnimateText()
    {
        Vector3 initialLocalPosition = baseLocalPosition;
        float elapsed = 0f;

        animatedTransform.localPosition = initialLocalPosition;
        pointText.color = baseColor;

        while (elapsed < animationDuration)
        {
            float t = animationDuration > 0f ? elapsed / animationDuration : 1f;

            float horizontalOffset = Mathf.Sin(t * Mathf.PI * 2f) * horizontalAmplitude;
            float verticalOffset = verticalCurve.Evaluate(t) * riseDistance;

            animatedTransform.localPosition = initialLocalPosition + new Vector3(horizontalOffset, verticalOffset, 0f);

            Color color = baseColor;
            color.a *= alphaCurve.Evaluate(t);
            pointText.color = color;

            elapsed += Time.deltaTime;
            yield return null;
        }

        animatedTransform.localPosition = initialLocalPosition;
        Color finalColor = baseColor;
        finalColor.a = 0f;
        pointText.color = finalColor;

        animateRoutine = null;
    }

    private void EnsureInitialized()
    {
        if (initialized)
        {
            return;
        }

        if (pointText == null)
        {
            pointText = GetComponentInChildren<TMP_Text>();
        }

        if (pointText == null)
        {
            return;
        }

        if (animatedTransform == null)
        {
            animatedTransform = pointText.transform;
        }

        baseColor = pointText.color;
        baseLocalPosition = animatedTransform != null ? animatedTransform.localPosition : Vector3.zero;
        initialized = true;
    }
}
