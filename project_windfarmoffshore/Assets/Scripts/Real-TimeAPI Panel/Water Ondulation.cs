using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class WaterOndulation : MonoBehaviour
{
    public TMP_Text text;
    private string apiUrl = "https://marine-api.open-meteo.com/v1/marine?latitude=41.3834&longitude=-8.7636&hourly=wave_height";
    private float updateInterval = 60f;

    public float waveHeight { get; private set; }

    void Start()
    {
        StartCoroutine(UpdateWaveHeight());
    }

    IEnumerator UpdateWaveHeight()
    {
        while (true)
        {
            yield return StartCoroutine(GetWaveHeight());
            yield return new WaitForSeconds(updateInterval);
        }
    }

    IEnumerator GetWaveHeight()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                MarineData marineData = JsonUtility.FromJson<MarineData>(jsonResponse);

                if (marineData != null && marineData.hourly.wave_height.Length > 0)
                {
                    string currentTime = System.DateTime.UtcNow.ToString("yyyy-MM-ddTHH:00");
                    int index = FindClosestTimeIndex(marineData.hourly.time, currentTime);
                    waveHeight = marineData.hourly.wave_height[index];

                    text.text = "Wave height: " + waveHeight.ToString("F1") + " m";
                }
                else
                {
                    text.text = "Error finding wave height";
                }
            }
            else
            {
                text.text = "Request error " + request.error;
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

    [System.Serializable]
    public class MarineHourly
    {
        public string[] time;
        public float[] wave_height;
    }

    [System.Serializable]
    public class MarineData
    {
        public MarineHourly hourly;
    }
}
