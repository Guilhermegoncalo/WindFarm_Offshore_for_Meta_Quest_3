using UnityEngine;

public class FollowCenterEyeUI : MonoBehaviour
{
    public Transform centerEye; 
    public Vector3 offset = new Vector3(0, -0.2f, 1.5f); 

    void LateUpdate()
    {
        if (centerEye == null) return;

        transform.position = centerEye.position + centerEye.forward * offset.z
                                              + centerEye.up * offset.y
                                              + centerEye.right * offset.x;

       
        transform.rotation = Quaternion.LookRotation(transform.position - centerEye.position);
    }
}
