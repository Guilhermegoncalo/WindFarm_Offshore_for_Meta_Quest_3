using UnityEngine;

public class AdjustFarPlane : MonoBehaviour
{
    public Camera _centerEyeCamera;
    public Camera _leftEyeCamera;
    public Camera _rightEyeCamera;
    public float farClipDistance = 1000f;

    void Start()
    {
        // Certifica-se de que as c�meras est�o atribu�das
        if (_centerEyeCamera == null)
            _centerEyeCamera = Camera.main;  // Ou qualquer l�gica para obter a c�mera central

        // Ajusta o farClipPlane para as 3 c�meras de realidade aumentada
        SetFarClipPlane(farClipDistance);
    }

    void Update()
    {
        // Pode atualizar dinamicamente se necess�rio
        SetFarClipPlane(farClipDistance);
    }

    private void SetFarClipPlane(float farDistance)
    {
        // Ajusta o farClipPlane de cada c�mera, caso existam
        if (_centerEyeCamera != null)
            _centerEyeCamera.farClipPlane = farDistance;

        if (_leftEyeCamera != null)
            _leftEyeCamera.farClipPlane = farDistance;

        if (_rightEyeCamera != null)
            _rightEyeCamera.farClipPlane = farDistance;
    }
}
