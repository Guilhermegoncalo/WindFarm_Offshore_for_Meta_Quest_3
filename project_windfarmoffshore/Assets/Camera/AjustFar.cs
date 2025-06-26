using UnityEngine;

public class AdjustFarPlane : MonoBehaviour
{
    public Camera _centerEyeCamera;
    public Camera _leftEyeCamera;
    public Camera _rightEyeCamera;
    public float farClipDistance = 1000f;

    void Start()
    {
        // Certifica-se de que as câmeras estão atribuídas
        if (_centerEyeCamera == null)
            _centerEyeCamera = Camera.main;  // Ou qualquer lógica para obter a câmera central

        // Ajusta o farClipPlane para as 3 câmeras de realidade aumentada
        SetFarClipPlane(farClipDistance);
    }

    void Update()
    {
        // Pode atualizar dinamicamente se necessário
        SetFarClipPlane(farClipDistance);
    }

    private void SetFarClipPlane(float farDistance)
    {
        // Ajusta o farClipPlane de cada câmera, caso existam
        if (_centerEyeCamera != null)
            _centerEyeCamera.farClipPlane = farDistance;

        if (_leftEyeCamera != null)
            _leftEyeCamera.farClipPlane = farDistance;

        if (_rightEyeCamera != null)
            _rightEyeCamera.farClipPlane = farDistance;
    }
}
