using UnityEngine;

public class LookDirectionMovement : MonoBehaviour
{
    public float speed = 3.0f;

    void Update()
    {
       
        Vector2 input = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        if (input.sqrMagnitude < 0.01f)
            return;

       
        Transform headTransform = Camera.main.transform;

      
        Vector3 headForward = headTransform.forward;
        Vector3 headRight = headTransform.right;

      
        headForward.y = 0;
        headRight.y = 0;
        headForward.Normalize();
        headRight.Normalize();

       
        Vector3 moveDirection = headForward * input.y + headRight * input.x;

       
        transform.position += moveDirection * speed * Time.deltaTime;
    }
}