using UnityEngine;

public class ClickOrGrabOculus : MonoBehaviour
{
    public GameObject panelToToggle;
    public float clickThreshold = 0.25f;
    public Rigidbody targetRigidbody; 

    private float grabStartTime;

    public void OnGrabStart()
    {
        grabStartTime = Time.time;

       
        if (targetRigidbody != null)
        {
            targetRigidbody.isKinematic = true;
        }
    }

    public void OnGrabEnd()
    {
        float heldDuration = Time.time - grabStartTime;

        if (heldDuration <= clickThreshold)
        {
           
            if (panelToToggle != null)
                panelToToggle.SetActive(!panelToToggle.activeSelf);
        }
        else
        {
            
            if (targetRigidbody != null)
            {
                targetRigidbody.isKinematic = false;
            }
        }
    }
}
