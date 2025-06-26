using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;

public class WeatherManager : MonoBehaviour
{
    [Header("UI Sliders")]
    [SerializeField] private Slider RainSlider;
    [SerializeField] private Slider SnowSlider;
    [SerializeField] private Slider HailSlider;

    [Header("VFX References")]
    [SerializeField] private VisualEffect RainVFX;
    [SerializeField] private VisualEffect SnowVFX;
    [SerializeField] private VisualEffect HailVFX;

    private void Start()
    {
        
        RainSlider.minValue = 0f;
        RainSlider.maxValue = 1f;
        SnowSlider.minValue = 0f;
        SnowSlider.maxValue = 1f;
        HailSlider.minValue = 0f;
        HailSlider.maxValue = 1f;

      
        RainSlider.value = 0f;
        SnowSlider.value = 0f;
        HailSlider.value = 0f;

       
        RainVFX.SetFloat("Intensity", RainSlider.value);
        SnowVFX.SetFloat("Intensity", SnowSlider.value);
        HailVFX.SetFloat("Intensity", HailSlider.value);

    
        RainSlider.onValueChanged.AddListener(UpdateRain);
        SnowSlider.onValueChanged.AddListener(UpdateSnow);
        HailSlider.onValueChanged.AddListener(UpdateHail);
    }

    private void UpdateRain(float value)
    {
        RainVFX.SetFloat("Intensity", value);
    }

    private void UpdateSnow(float value)
    {
        SnowVFX.SetFloat("Intensity", value);
    }

    private void UpdateHail(float value)
    {
        HailVFX.SetFloat("Intensity", value);
    }
}
