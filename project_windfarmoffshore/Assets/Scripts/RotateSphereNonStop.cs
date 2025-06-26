using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class RotateSphereNonStop : MonoBehaviour
{
    private Vector3 movement = new Vector3(0, 150, 0);


    void Update()
    {
        transform.Rotate(movement  * Time.deltaTime);
    }
}