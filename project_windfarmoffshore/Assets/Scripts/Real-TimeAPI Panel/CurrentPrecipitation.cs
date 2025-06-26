using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro; 

public class CurrentPrecipitation : MonoBehaviour
{
    
    private string apiUrl = "https://api.open-meteo.com/v1/forecast?latitude=52.52&longitude=13.41&current=precipitation";
    private float updateInterval = 60f; 

   
    public TMP_Text precipitationText;

    void Start()
    {
       
        StartCoroutine(GetCurrentPrecipitation());
    }

    void Update()
    {
      
    }

    IEnumerator GetCurrentPrecipitation()
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
                        float currentPrecipitation = weatherResponse.current.precipitation;
                        Debug.Log("Precipitation: " + currentPrecipitation + " mm");

                        
                        if (precipitationText != null)
                        {
                            precipitationText.text = "Precipitation (Rain/Hail): " + currentPrecipitation.ToString("F1") + " mm";
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Erro ao buscar precipitação: " + request.error);
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
        public float precipitation;
    }
}
