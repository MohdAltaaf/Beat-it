using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Vignette : MonoBehaviour
{
    public Volume globalVolume;
    public float maxIntensity = 0.4f; // whatever value you landed on while tweaking

    private UnityEngine.Rendering.Universal.Vignette vignette;

    void Start()
    {
        globalVolume.profile.TryGet(out vignette);
    }

    public void SetIntensity(float t)
    {
        if (vignette == null) return;
        vignette.intensity.value = Mathf.Clamp01(t) * maxIntensity;
    }
}