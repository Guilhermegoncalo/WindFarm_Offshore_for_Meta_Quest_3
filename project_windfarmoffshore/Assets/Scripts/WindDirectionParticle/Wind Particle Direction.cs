using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WindParticleController : MonoBehaviour
{
    public Slider directionSlider;
    public Toggle toggle;
    public GameObject windPrefab;

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
    }

    void Update()
    {
        if (!isToggled)
        {
            windDirection = Mathf.Lerp(290f, -70f, directionSlider.value);


        }

        if (windPrefab != null)
        {
            windPrefab.transform.rotation = Quaternion.Euler(0f, windDirection, 0f);
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
                }
            }
            else
            {
                Debug.LogWarning("Error findinf windspeeddirection: " + request.error);
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
