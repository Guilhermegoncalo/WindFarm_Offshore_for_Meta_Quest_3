using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class WeatherDetails : MonoBehaviour
{
    public TMP_Text text;
    private string apiUrl = "https://api.open-meteo.com/v1/forecast?latitude=41.3834&longitude=-8.7636&hourly=cloud_cover,cloud_cover_low,cloud_cover_mid,cloud_cover_high,precipitation,showers,wind_gusts_10m";
    private float updateInterval = 60f;

    private WeatherData weatherData;

    void Start()
    {
        StartCoroutine(UpdateWeather());
    }

    IEnumerator UpdateWeather()
    {
        while (true)
        {
            yield return StartCoroutine(GetWeatherData());
            yield return new WaitForSeconds(updateInterval);
        }
    }

    IEnumerator GetWeatherData()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                weatherData = JsonUtility.FromJson<WeatherData>(json);

                if (weatherData != null && weatherData.hourly.time.Length > 0)
                {
                    string currentTime = System.DateTime.UtcNow.ToString("yyyy-MM-ddTHH:00");
                    int index = FindClosestTimeIndex(weatherData.hourly.time, currentTime);

                    string output =
                        $"Cloud Cover Total: {weatherData.hourly.cloud_cover[index]} %\n" +
                        $"Cloud Cover Low: {weatherData.hourly.cloud_cover_low[index]} %\n" +
                        $"Cloud Cover Mid: {weatherData.hourly.cloud_cover_mid[index]} %\n" +
                        $"Cloud Cover High: {weatherData.hourly.cloud_cover_high[index]} %\n" +
                        $"Showers: {weatherData.hourly.showers[index]} mm\n" +
                        $"Wind Gusts: {weatherData.hourly.wind_gusts_10m[index]} km/h";

                    text.text = output;
                }
                else
                {
                    text.text = "Erro ao processar dados meteorológicos.";
                }
            }
            else
            {
                text.text = "Erro de requisição: " + request.error;
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

        return 0; // fallback
    }
}
