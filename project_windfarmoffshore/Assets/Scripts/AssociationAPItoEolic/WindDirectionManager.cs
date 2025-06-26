using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WindDirectionManager : MonoBehaviour
{
    public Slider directionSlider;
    public Toggle toggle;
    public TMP_Text windDirectionText;
    public List<GameObject> fans = new List<GameObject>();

    private string apiUrl = "https://api.open-meteo.com/v1/forecast?latitude=41.3834&longitude=-8.7636&hourly=wind_direction_80m";
    private float updateInterval = 60f;
    private Coroutine windDirectionCoroutine;
    private bool isToggled;

    public float windDirection { get; private set; } 

    void Start()
    {
        isToggled = toggle.isOn;
        toggle.onValueChanged.AddListener(UpdateToggleState);

        if (isToggled)
        {
            windDirectionCoroutine = StartCoroutine(UpdateWindDirection());
        }

        UpdateDirectionText(directionSlider.value);
    }

    void Update()
    {
        if (!isToggled)
        {
            
            windDirection = Mathf.Lerp(0f, 360f, directionSlider.value);
            UpdateDirectionText(windDirection);
        }

        
        foreach (GameObject fan in fans)
        {
            if (fan != null)
            {
                fan.transform.rotation = Quaternion.Euler(0f, -windDirection -90, 0f); 
            }
        }
    }

    IEnumerator UpdateWindDirection()
    {
        while (true)
        {
            yield return StartCoroutine(GetWindDirection());
            yield return new WaitForSeconds(updateInterval);
        }
    }

    IEnumerator GetWindDirection()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                WeatherData weatherData = JsonUtility.FromJson<WeatherData>(jsonResponse);

                if (weatherData != null && weatherData.hourly.wind_direction_80m.Length > 0)
                {
                    string currentTime = System.DateTime.UtcNow.ToString("yyyy-MM-ddTHH:00");
                    int index = FindClosestTimeIndex(weatherData.hourly.time, currentTime);
                    windDirection = weatherData.hourly.wind_direction_80m[index];
                    UpdateDirectionText(windDirection);
                }
            }
            else
            {
                Debug.LogWarning("Errr finding wind direction angle: " + request.error);
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

    public void UpdateToggleState(bool value)
    {
        isToggled = value;

        if (isToggled)
        {
            if (windDirectionCoroutine == null)
                windDirectionCoroutine = StartCoroutine(UpdateWindDirection());
        }
        else
        {
            if (windDirectionCoroutine != null)
            {
                StopCoroutine(windDirectionCoroutine);
                windDirectionCoroutine = null;
            }
        }
    }

    private void UpdateDirectionText(float value)
    {
        if (!isToggled && windDirectionText != null)
        {
            windDirectionText.text = "Wind direction angle: " + value.ToString("F0") + "°";
        }
        else if (isToggled && windDirectionText != null)
        {
            windDirectionText.text = "Wind direction angle: 0°";
        }
    }


    [System.Serializable]
    public class WeatherData
    {
        public Hourly hourly;
    }

    [System.Serializable]
    public class Hourly
    {
        public string[] time;
        public float[] wind_direction_80m;
    }
}
