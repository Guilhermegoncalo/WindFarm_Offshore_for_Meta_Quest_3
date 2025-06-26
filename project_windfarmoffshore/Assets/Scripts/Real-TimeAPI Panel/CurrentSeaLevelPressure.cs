using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro; 

public class CurrentPressure : MonoBehaviour
{
   
    private string apiUrl = "https://api.open-meteo.com/v1/forecast?latitude=41.3834&longitude=-8.7636&current=pressure_msl";
    private float updateInterval = 60f; 

    
    public TMP_Text pressureText;

    void Start()
    {
        
        StartCoroutine(GetCurrentPressure());
    }

    IEnumerator GetCurrentPressure()
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
                        float currentPressure = weatherResponse.current.pressure_msl;
                        Debug.Log("Sea level pressure: " + currentPressure + " hPa");

                       
                        if (pressureText != null)
                        {
                            pressureText.text = "Sea level pressure: " + currentPressure.ToString("F1") + " hPa";
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Error finding sea level pressure: " + request.error);
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
        public float pressure_msl;
    }
}
