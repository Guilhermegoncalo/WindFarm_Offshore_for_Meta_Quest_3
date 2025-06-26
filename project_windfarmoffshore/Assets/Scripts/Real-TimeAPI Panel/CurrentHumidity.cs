using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro; 

public class CurrentHumidity : MonoBehaviour
{
    private string apiUrl = "https://api.open-meteo.com/v1/forecast?latitude=41.3834&longitude=-8.7636&current=relative_humidity_2m";
    private float updateInterval = 60f; 

    
    public TMP_Text humidityText;

    void Start()
    {
       
        StartCoroutine(GetCurrentHumidity());
    }



    IEnumerator GetCurrentHumidity()
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
                        float currentHumidity = weatherResponse.current.relative_humidity_2m;
                        Debug.Log("Humidity: " + currentHumidity + "%");

                       
                        if (humidityText != null)
                        {
                            humidityText.text = "Humidity: " + currentHumidity.ToString("F1") + "%";
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Error finding humidity: " + request.error);
                }
            }

           
            yield return new WaitForSeconds(updateInterval);
        }
    }

    
    [System.Serializable]
    public class WeatherResponse
    {
        public CurrentWeather current;
    }

    [System.Serializable]
    public class CurrentWeather
    {
        public float relative_humidity_2m;
    }
}
