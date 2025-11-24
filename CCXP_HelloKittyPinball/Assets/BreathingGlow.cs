using UnityEngine;

/// <summary>
/// Controls an object's emission intensity with a breathing (pulsing) effect.
/// Uses a MaterialPropertyBlock so this works on shared materials without creating material instances.
/// The breathing is only active while Time.time is within [breathStartTime, breathEndTime].
/// </summary>
public class BreathingGlow : MonoBehaviour
{
    [Tooltip("Maximum emission intensity (multiplier).")]
    public float glowIntensityMax = 1.0f;

    [Tooltip("Minimum emission intensity (multiplier).")]
    public float glowIntensityMin = 0.5f;

    [Tooltip("Speed of the breathing cycle (cycles per second).")]
    public float glowSpeed = 1.0f;

    [Tooltip("Start time (in seconds since startup) when breathing becomes active.")]
    public float breathStartTime = 0.0f;

    [Tooltip("End time (in seconds since startup) when breathing stops.")]
    public float breathEndTime = float.PositiveInfinity;

    [Tooltip("Base emission color. The intensity is multiplied into this color's RGB values.")]
    public Color emissionColor = Color.white;

    // Cached renderer and property block
    Renderer cachedRenderer;
    MaterialPropertyBlock mpb;

    // Cache property id for speed
    static readonly int EmissionId = Shader.PropertyToID("_EmissionColor");

    void Awake()
    {
        cachedRenderer = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();

        // Safety clamps so inspector values stay sensible
        glowIntensityMax = Mathf.Max(0f, glowIntensityMax);
        glowIntensityMin = Mathf.Max(0f, glowIntensityMin);
        glowSpeed = Mathf.Max(0f, glowSpeed);
    }

    void OnValidate()
    {
        // Keep values sane when edited in inspector
        glowIntensityMax = Mathf.Max(0f, glowIntensityMax);
        glowIntensityMin = Mathf.Max(0f, glowIntensityMin);
        glowSpeed = Mathf.Max(0f, glowSpeed);

        // Ensure min is not greater than max
        if (glowIntensityMin > glowIntensityMax)
        {
            float tmp = glowIntensityMin;
            glowIntensityMin = glowIntensityMax;
            glowIntensityMax = tmp;
        }
    }

    void Update()
    {
        if (cachedRenderer == null)
            return;

        float t = Time.time;

        // Determine whether breathing should be active in this time window
        bool inWindow = t >= breathStartTime && t <= breathEndTime;

        float intensity = glowIntensityMin;

        if (inWindow)
        {
            // Use a sin wave to smoothly pulse between min and max.
            // sin ranges -1..1, map to 0..1 and then to min..max
            float cycle = Mathf.Sin(2f * Mathf.PI * glowSpeed * t) * 0.5f + 0.5f;
            // Optional smoothing
            cycle = Mathf.SmoothStep(0f, 1f, cycle);
            intensity = Mathf.Lerp(glowIntensityMin, glowIntensityMax, cycle);
        }

        // Apply to material via MaterialPropertyBlock to avoid instantiating materials
        cachedRenderer.GetPropertyBlock(mpb);
        Color c = emissionColor * intensity;
        mpb.SetColor(EmissionId, c);
        cachedRenderer.SetPropertyBlock(mpb);
    }
}
