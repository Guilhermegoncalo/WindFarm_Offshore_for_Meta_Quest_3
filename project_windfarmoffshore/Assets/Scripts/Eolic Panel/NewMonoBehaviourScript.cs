using UnityEngine;
using System.Collections.Generic;

public class TurbineTouchPanelManager : MonoBehaviour
{
    [System.Serializable]
    public class TurbineData
    {
        public GameObject turbineObject; // Objeto a clicar
        public GameObject panel;         // UI associada
        public Transform turbineBase;    // Base da turbina para posicionar o painel
    }

    public List<TurbineData> turbines = new List<TurbineData>();

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                foreach (var turbine in turbines)
                {
                    if (hit.collider.gameObject == turbine.turbineObject)
                    {
                        Debug.Log("Turbine clicked: " + turbine.turbineObject.name);

                        if (turbine.panel != null && turbine.turbineBase != null)
                        {
                            bool isActive = turbine.panel.activeSelf;
                            turbine.panel.SetActive(!isActive);

                            if (!isActive)
                            {
                                turbine.panel.transform.position = turbine.turbineBase.position + new Vector3(0, 470, 0);
                            }
                        }

                        break; // parar ao encontrar a turbina correta
                    }
                }
            }
        }
    }
}
