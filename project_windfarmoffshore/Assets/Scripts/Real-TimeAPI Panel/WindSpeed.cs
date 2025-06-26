using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class WindSpeed : MonoBehaviour
{
    public TMP_Text text;
    private string apiUrl = "https://api.open-meteo.com/v1/forecast?latitude=41.3834&longitude=-8.7636&hourly=wind_speed_80m";
    private float updateInterval = 60f;

    public float windspeed { get; private set; }

    void Start()
    {
        StartCoroutine(UpdateWindSpeed());
    }

    IEnumerator UpdateWindSpeed()
    {
        while (true)
        {
            yield return StartCoroutine(GetWindSpeed());
            yield return new WaitForSeconds(updateInterval);
        }
    }

    IEnumerator GetWindSpeed()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                WeatherData weatherData = JsonUtility.FromJson<WeatherData>(jsonResponse);

                if (weatherData != null && weatherData.hourly.wind_speed_80m.Length > 0)
                {
                    string currentTime = System.DateTime.UtcNow.ToString("yyyy-MM-ddTHH:00");
                    int index = FindClosestTimeIndex(weatherData.hourly.time, currentTime);
                    windspeed = weatherData.hourly.wind_speed_80m[index];
                    text.text = "Wind speed: " + windspeed + " km/h";
                }
                else
                {
                    text.text = "Error finding windspeed";
                }
            }
            else
            {
                text.text = "Request error: " + request.error;
            }
        }
    }
    int FindClosestTimeIndex(string[] times, string currentTime)
    {
        for (int i = 0; i < times.Length; i++)
        {
            if (times[i] == currentTime)
            {
                return i;
            }
        }

        return 0;
    }
}


[System.Serializable]
public class HourlyData
{
    public string[] time;
    public float[] wind_speed_80m;
    public float[] wind_direction_80m;
    public float[] cloud_cover;
    public float[] cloud_cover_low;
    public float[] cloud_cover_mid;
    public float[] cloud_cover_high;
    public float[] precipitation;
    public float[] showers;
    public float[] wind_gusts_10m;
}

[System.Serializable]
public class WeatherData
{
    public HourlyData hourly;
}