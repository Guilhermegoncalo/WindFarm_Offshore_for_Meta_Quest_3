using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro; 

public class CurrentSnowfall : MonoBehaviour
{
  
    private string apiUrl = "https://api.open-meteo.com/v1/forecast?latitude=52.52&longitude=13.41&current=snowfall";
    private float updateInterval = 60f; 


    public TMP_Text snowfallText;

    void Start()
    {
        
        StartCoroutine(GetCurrentSnowfall());
    }

    void Update()
    {
    }

    IEnumerator GetCurrentSnowfall()
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
                        float currentSnowfall = weatherResponse.current.snowfall;
                        Debug.Log("Snowfall: " + currentSnowfall + " cm");

                      
                        if (snowfallText != null)
                        {
                            snowfallText.text = "Snowfall: " + currentSnowfall.ToString("F1") + " cm";
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Error finding snowfall: " + request.error);
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
        public float snowfall;
    }
}
