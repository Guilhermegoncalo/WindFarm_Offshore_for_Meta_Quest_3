using UnityEngine;

public class RemovePanelWithFan : MonoBehaviour
{
    public GameObject panelToClose;
    private WindTurbineAlocation windTurbineAlocation;

    void Awake()
    {
        windTurbineAlocation = GetComponent<WindTurbineAlocation>();
        if (windTurbineAlocation == null)
            Debug.LogWarning("WindTurbineAlocation not finded");
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            RemoveFanAndClosePanel();
        }
    }

    public void RemoveFanAndClosePanel()
    {
        if (panelToClose != null && panelToClose.activeSelf)
        {
            panelToClose.SetActive(false);
            Debug.Log("Panel closedd with trigger");
        }

        if (windTurbineAlocation != null)
        {
            windTurbineAlocation.RemoveFan();
        }
    }
}