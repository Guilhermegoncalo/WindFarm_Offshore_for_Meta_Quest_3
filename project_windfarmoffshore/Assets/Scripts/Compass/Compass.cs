using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    float halfSize = 600;
    public RectTransform north;
    public RectTransform east;
    public RectTransform south;
    public RectTransform west;
    public Transform cameraTransform;

    void Update()
    {
        Vector3 forward = cameraTransform.forward;
        Vector2 forwardXZ = new Vector2(forward.x, forward.z).normalized;


        float playerAngle = -Vector2.SignedAngle(forwardXZ, Vector2.up);


        UpdateMarker(north, 0f, playerAngle);
        UpdateMarker(east, 90f, playerAngle);
        UpdateMarker(south, 180f, playerAngle);
        UpdateMarker(west, 270f, playerAngle);
    }

    void UpdateMarker(RectTransform marker, float markerBaseAngle, float playerAngle)
    {

        float angle = markerBaseAngle - playerAngle;

        angle = Mathf.Repeat(angle + 180f, 360f) - 180f;


        float posX = (halfSize / 180f) * angle;

        marker.anchoredPosition = new Vector2(posX, marker.anchoredPosition.y);
    }
}

