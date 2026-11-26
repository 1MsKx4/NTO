using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BackgroundEffect : MonoBehaviour
{
    public int act;
    public FullScreenPassRendererFeature finalPass;
    public Texture2D[] filters;
    public float filterStrength = 1f;
    public bool saturationOnly;
    public Color finalColor = new Color(0.85f, 0.65f, 1f);
    public float strength = 1f, speed = 0.7f, wave = 0.009f;
    public float colorShift = 0.0015f, vignette = 0.3f;

    private void LateUpdate()
    {
        finalPass.SetActive(act > 0);
        Shader.SetGlobalFloat("_NTOFinalStrength", 0);
        if (act == 4)
        {
            Shader.SetGlobalFloat("_NTOFinalStrength", Mathf.Clamp01(strength));
        }
        if (act > 0)
        {
            Shader.SetGlobalTexture("_NTOFilter", filters[act - 1]);
        }
        Shader.SetGlobalFloat("_NTOFilterStrength", Mathf.Clamp01(filterStrength));
        Shader.SetGlobalInt("_NTOSaturationOnly", 0);
        if (saturationOnly)
        {
            Shader.SetGlobalInt("_NTOSaturationOnly", 1);
        }
        Shader.SetGlobalColor("_NTOFinalTint", finalColor);
        Shader.SetGlobalVector("_NTOFinalSettings", new Vector4(
            Mathf.Clamp(wave, 0, 0.03f), speed, colorShift, Mathf.Clamp01(vignette)));
    }

    private void OnDisable()
    {
        finalPass.SetActive(false);
        Shader.SetGlobalFloat("_NTOFinalStrength", 0);
    }
}
