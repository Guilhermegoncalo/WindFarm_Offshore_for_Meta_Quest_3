using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System;

public class WindDirection : MonoBehaviour
{
    public TMP_Text text;
    private string apiUrl = "https://api.open-meteo.com/v1/forecast?latitude=41.3834&longitude=-8.7636&hourly=wind_direction_80m";
    private float updateInterval = 60f;
    public float windDirection { get; private set; }

    void Start()
    {
        StartCoroutine(UpdateWindData());
    }

    IEnumerator UpdateWindData()
    {
        while (true)
        {
            yield return StartCoroutine(GetWindData());
            yield return new WaitForSeconds(updateInterval);
        }
    }

    IEnumerator GetWindData()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("API Response: " + jsonResponse);

                WeatherData weatherData = JsonUtility.FromJson<WeatherData>(jsonResponse);

                if (weatherData != null && weatherData.hourly.wind_direction_80m.Length > 0)
                {
                    string currentTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:00");
                    int index = FindClosestTimeIndex(weatherData.hourly.time, currentTime);


                    windDirection = weatherData.hourly.wind_direction_80m[index];
                    text.text = "Wind direction angle: " + windDirection + "º";
                    text.ForceMeshUpdate();

                }
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