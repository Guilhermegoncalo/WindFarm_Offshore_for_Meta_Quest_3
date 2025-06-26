using UnityEngine;
using TMPro;
using System;

public class CurrentTime : MonoBehaviour
{
    public TMP_Text timeText;

    void Start()
    {
        InvokeRepeating(nameof(UpdateTime), 0f, 1f); // Atualiza a cada 1 segundo
    }

    void UpdateTime()
    {
        if (timeText != null)
        {
            timeText.text = "Time: " + DateTime.Now.ToString("HH:mm:ss");
        }
    }
}
