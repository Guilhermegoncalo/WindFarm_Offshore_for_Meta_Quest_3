using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class WaterDisplacementController : MonoBehaviour
{
    public Renderer waterRenderer;
    public Slider displacementSlider;
    public Toggle toggle;
    public TMP_Text waveHeightText;
    public GameObject[] fans; 

    private Material waterMaterialInstance;
    private const string displacementProperty = "Vector1_7273530c27a34c9f8ee5723b84f96baa";

    private string apiUrl = "https://marine-api.open-meteo.com/v1/marine?latitude=41.3834&longitude=-8.7636&hourly=wave_height";
    private float updateInterval = 60f;
    private Coroutine waveHeightCoroutine;
    private bool isToggled;

    private float waveHeight;

    void Start()
    {
        if (waterRenderer != null)
        {
            waterMaterialInstance = new Material(waterRenderer.sharedMaterial);
            waterRenderer.material = waterMaterialInstance;
        }

        if (waterMaterialInstance != null && waterMaterialInstance.HasProperty(displacementProperty))
        {
            displacementSlider.minValue = 0;
            displacementSlider.maxValue = 3;

            float initialValue = waterMaterialInstance.GetFloat(displacementProperty);
            displacementSlider.value = initialValue;
            waveHeight = initialValue;

            displacementSlider.onValueChanged.AddListener(OnSliderChanged);
        }
        else
        {
            Debug.LogError($"O Shader do not contain the propriety {displacementProperty}");
        }

        isToggled = toggle.isOn;
        toggle.onValueChanged.AddListener(UpdateToggleState);

        if (isToggled)
        {
            waveHeightCoroutine = StartCoroutine(UpdateWaveHeight());
        }

     
        UpdateWaveHeightUI(waveHeight);
    }

    void Update()
    {
        if (!isToggled)
        {
            waveHeight = displacementSlider.value;
            ApplyWaveHeight(waveHeight);
            UpdateWaveHeightUI(waveHeight);
        }

        
        foreach (GameObject fan in fans)
        {
            if (fan != null)
            {
                Vector3 pos = fan.transform.position;
                fan.transform.position = new Vector3(pos.x, waveHeight, pos.z);
            }
        }
    }

    void OnSliderChanged(float value)
    {
        if (!isToggled)
        {
            ApplyWaveHeight(value);
        }
    }

    void ApplyWaveHeight(float value)
    {
        if (waterMaterialInstance != null)
        {
            waterMaterialInstance.SetFloat(displacementProperty, value);
        }
    }

    void UpdateWaveHeightUI(float value)
    {
        if (waveHeightText != null)
        {
            if (isToggled)
            {
                
                waveHeightText.text = "Wave height: 0,0 m";
            }
            else
            {
                
                waveHeightText.text = "Wave height: " + value.ToString("F1") + " m";
            }
        }
    }

    void UpdateToggleState(bool value)
    {
        isToggled = value;

        if (isToggled)
        {
           
            if (waveHeightCoroutine == null)
                waveHeightCoroutine = StartCoroutine(UpdateWaveHeight());
        }
        else
        {
           
            if (waveHeightCoroutine != null)
            {
                StopCoroutine(waveHeightCoroutine);
                waveHeightCoroutine = null;
            }
            waveHeight = 0;
            UpdateWaveHeightUI(waveHeight);
        }
    }

    IEnumerator UpdateWaveHeight()
    {
        while (true)
        {
            yield return StartCoroutine(GetWaveHeightFromAPI());
            yield return new WaitForSeconds(updateInterval);
        }
    }

    IEnumerator GetWaveHeightFromAPI()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                WaveData waveData = JsonUtility.FromJson<WaveData>(jsonResponse);

                if (waveData != null && waveData.hourly.wave_height.Length > 0)
                {
                    string currentTime = System.DateTime.UtcNow.ToString("yyyy-MM-ddTHH:00");
                    int index = FindClosestTimeIndex(waveData.hourly.time, currentTime);
                    waveHeight = waveData.hourly.wave_height[index];

                    ApplyWaveHeight(waveHeight);
                    UpdateWaveHeightUI(waveHeight);
                }
            }
            else
            {
                Debug.LogWarning("Error finding Water height: " + request.error);
            }
        }
    }

    int FindClosestTimeIndex(string[] times, string currentTime)
    {
        for (int i = 0; i < times.Length; i++)
        {
            if (times[i] == currentTime)
                return i;
        }
        return 0;
    }

    [System.Serializable]
    public class WaveData
    {
        public Hourly hourly;
    }

    [System.Serializable]
    public class Hourly
    {
        public string[] time;
        public float[] wave_height;
    }
}
