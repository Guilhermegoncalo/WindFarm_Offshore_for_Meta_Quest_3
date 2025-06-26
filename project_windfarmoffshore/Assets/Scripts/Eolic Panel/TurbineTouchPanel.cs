using UnityEngine;
using System.Collections.Generic;

public class TurbineTouchPanelOVR : MonoBehaviour
{
    public GameObject panel;
    public Transform turbineBase;


    private static List<TurbineTouchPanelOVR> allTurbines = new List<TurbineTouchPanelOVR>();

    void Awake()
    {
        allTurbines.Add(this);
    }

    void OnDestroy()
    {
        allTurbines.Remove(this);
    }

    void Update()
    {

        TurbineTouchPanelOVR current = GetLastCreated();
        if (current == this && panel != null && turbineBase != null)
        {
            if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
            {
                bool isActive = panel.activeSelf;
                panel.SetActive(!isActive);

                if (!isActive)
                {
                    panel.transform.position = turbineBase.position + new Vector3(0, 15, 0);
                }

                Debug.Log("Botão A - painel alternado (última turbina ativa)");
            }
        }
    }

    public static TurbineTouchPanelOVR GetLastCreated()
    {

        if (allTurbines.Count > 0)
        {
            return allTurbines[allTurbines.Count - 1];
        }
        return null;
    }
}