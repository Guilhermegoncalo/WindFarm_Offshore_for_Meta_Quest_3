using UnityEngine;

public class Turbinesopenpanel : MonoBehaviour
{
    [System.Serializable]
    public class TurbineData
    {
        public GameObject panel;
        public Transform turbineBase;
    }

    public TurbineData[] turbinas;

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            foreach (TurbineData turbina in turbinas)
            {
                if (turbina.panel != null && turbina.turbineBase != null)
                {
                    bool isActive = turbina.panel.activeSelf;
                    turbina.panel.SetActive(!isActive);

                    if (!isActive)
                    {
                        turbina.panel.transform.position = turbina.turbineBase.position + new Vector3(0, 15, 0);
                    }
                }
            }

            Debug.Log("Botão A - painéis alternados em todas as turbinas");
        }
    }
}

