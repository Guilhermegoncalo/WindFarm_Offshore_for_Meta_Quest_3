using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro; 

public class CurrentTemperature : MonoBehaviour
{
    private string apiUrl = "https://api.open-meteo.com/v1/forecast?latitude=41.3834&longitude=-8.7636&current_weather=true";
    private float updateInterval = 60f; 

   
    public TMP_Text temperatureText;

    void Start()
    {
        StartCoroutine(GetCurrentTemperature());
    }

    void Update()
    {
        
    }

    IEnumerator GetCurrentTemperature()
    {
        while (true) 
        {
            
            using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                  
                    string jsonResponse = request.downloadHandler.text;
                    WeatherResponse weatherResponse = JsonUtility.FromJson<WeatherResponse>(jsonResponse);

                    if (weatherResponse != null)
                    {
                        float currentTemperature = weatherResponse.current_weather.temperature;
                        Debug.Log("Temperature: " + currentTemperature + "°C");

                       
                        if (temperatureText != null)
                        {
                            temperatureText.text = "Temperature: " + currentTemperature.ToString("F1") + "°C";
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Error finding temperature: " + request.error);
                }
            }

           
            yield return new WaitForSeconds(updateInterval);
        }
    }

   
    [System.Serializable]
    public class WeatherResponse
    {
        public CurrentWeather current_weather;
    }

    [System.Serializable]
    public class CurrentWeather
    {
        public float temperature;
    }
}
