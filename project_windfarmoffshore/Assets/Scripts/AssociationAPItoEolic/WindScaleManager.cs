using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WindScaleManager : MonoBehaviour
{
    public Slider Slider1;
    public Slider rpmSlider;
    public Slider energySlider;
    public Slider temperatureSlider;

    public List<GameObject> fans = new List<GameObject>();
    private string apiUrl = "https://api.open-meteo.com/v1/forecast?latitude=41.3834&longitude=-8.7636&hourly=wind_speed_80m";
    private float updateInterval = 60f;
    private float TSR = 7f; 
    private float raiopa = 107f;
    public float rpm;
    public Toggle toggle;
    private bool isToggled;
    private Coroutine windSpeedCoroutine;
    public float windspeed { get; private set; }
    public float NrgProduced { get; private set; }

    public TMP_Text windSpeedText;
    public TMP_Text rpmText;
    public TMP_Text energyProducedText;
    public TMP_Text rotorTemperatureText;

    private const float airDensity = 1.225f;
    private const float Cp = 0.45f;

    private const float temperaturaBase = 25f;
    private const float temperaturaMaximaExtra = 55f;
    private const float rpmMaximo = 30f;

    void Start()
    {
        isToggled = toggle.isOn;
        toggle.onValueChanged.AddListener(UpdateToggleState);

        if (isToggled)
        {
            windSpeedCoroutine = StartCoroutine(UpdateWindSpeed());
        }

        UpdateWindSpeedText(Slider1.value);
    }

    void Update()
    {
        if (!isToggled)
        {
            float speedInKmH = Mathf.Lerp(0f, 150f, Slider1.value);
            windspeed = speedInKmH;
            UpdateWindSpeedText(windspeed);

            float velocityMetric = windspeed / 3.6f;
            float Omega = (TSR * velocityMetric) / raiopa;
            rpm = (Omega * 60) / (2 * Mathf.PI);

            UpdateRPMText(rpm);
            UpdateEnergyProducedText(windspeed);
            UpdateRotorTemperature(rpm);
        }
        else
        {
            UpdateWindSpeedText(0f);
        }

        foreach (GameObject fan in fans)
        {
            if (fan != null)
            {
                float rotationSpeed = (rpm / 60) * 360 * Time.deltaTime;
                fan.transform.Rotate(0, 0, -rotationSpeed);
            }
        }
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

                    float velocityMetric = windspeed / 3.6f;
                    float Omega = (TSR * velocityMetric) / raiopa;
                    rpm = (Omega * 60) / (2 * Mathf.PI);

                    UpdateRPMText(rpm);
                    UpdateEnergyProducedText(windspeed);
                    UpdateRotorTemperature(rpm);
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

    public void UpdateToggleState(bool value)
    {
        isToggled = value;

        if (isToggled)
        {
            if (windSpeedCoroutine == null)
                windSpeedCoroutine = StartCoroutine(UpdateWindSpeed());
        }
        else
        {
            if (windSpeedCoroutine != null)
            {
                StopCoroutine(windSpeedCoroutine);
                windSpeedCoroutine = null;
            }
        }
    }

    private void UpdateWindSpeedText(float value)
    {
        if (windSpeedText != null)
        {
            windSpeedText.text = "Wind speed: " + value.ToString("F1") + " Km/h";
        }
    }

    private void UpdateRPMText(float value)
    {
        if (rpmText != null)
        {
            rpmText.text = value.ToString("F1") + " m/s";
        }

        if (rpmSlider != null)
        {
            float normalizedRPM = Mathf.InverseLerp(0, 26, value);
            rpmSlider.value = normalizedRPM;
        }
    }

    private void UpdateEnergyProducedText(float windspeed)
    {
        float windSpeedMps = windspeed / 3.6f;
        float A = Mathf.PI * Mathf.Pow(raiopa, 2);
        float powerWatts = 0.5f * airDensity * A * Cp * Mathf.Pow(windSpeedMps, 3);
        float powerKW = powerWatts / 1000f;
        NrgProduced = powerKW;

        if (energyProducedText != null)
        {
            energyProducedText.text = powerKW.ToString("F0") + " kW";
        }

        if (energySlider != null)
        {
            float normalizedEnergy = Mathf.InverseLerp(0, 710000, powerKW); 
            energySlider.value = normalizedEnergy;
        }
    }

    private void UpdateRotorTemperature(float rpm)
    {
        float temperatura;

        if (rpm <= 0f)
        {
            temperatura = 0f;
        }
        else
        {
            // Se tiver rpm > 0, começa logo nos 25 ºC
            float temperaturaMinimaOperacional = 25f;
            float extra = (Mathf.Clamp(rpm, 0f, rpmMaximo) / rpmMaximo) * (temperaturaMaximaExtra - (temperaturaMinimaOperacional - temperaturaBase));
            temperatura = temperaturaMinimaOperacional + extra;
        }

        if (rotorTemperatureText != null)
        {
            rotorTemperatureText.text = temperatura.ToString("F1") + " ºC";
        }

        if (temperatureSlider != null)
        {
            float normalizedTemp = Mathf.InverseLerp(temperaturaBase, 72, temperatura);
            temperatureSlider.value = normalizedTemp;
        }
    }

}
