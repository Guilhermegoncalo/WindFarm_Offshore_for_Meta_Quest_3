using UnityEngine;

public class FanDragg : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = false;
    private bool possibleDrag = false;
    private Vector3 mouseDownPosition;
    private Vector3 offset;
    private Vector3 lastValidPosition;
    public WindTurbineAlocation gridManager;

    public bool IsDragging => isDragging;

    void Start()
    {
        mainCamera = Camera.main;

        if (gridManager == null)
        {
            gridManager = FindObjectOfType<WindTurbineAlocation>();
        }
    }

    void OnMouseDown()
    {
        possibleDrag = true;
        mouseDownPosition = Input.mousePosition;
        offset = transform.position - GetMouseWorldPosition();
        lastValidPosition = transform.root.position;

        if (gridManager != null)
        {
            gridManager.ClearFanFromGrid(transform.root.gameObject);
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;

            if (gridManager != null)
            {
                Vector3 snappedPosition = gridManager.GetNearestGridPosition(transform.position);
                int gridIndex = gridManager.GetGridIndexFromPosition(snappedPosition);

                if (gridIndex != -1 && gridManager.IsGridPositionFree(snappedPosition))
                {
                    transform.root.position = snappedPosition;
                    gridManager.SnapFanToGrid(transform.root.gameObject, snappedPosition);
                }
                else
                {
                    Debug.Log("Posição inválida ou ocupada. Reposicionada para a última válida.");
                    transform.root.position = lastValidPosition;
                    gridManager.SnapFanToGrid(transform.root.gameObject, lastValidPosition);
                }
            }
        }

        possibleDrag = false;
    }

    void Update()
    {
        if (possibleDrag && !isDragging)
        {
            float distance = Vector3.Distance(Input.mousePosition, mouseDownPosition);
            if (distance > 10f)
            {
                isDragging = true;
            }
        }

        if (isDragging)
        {
            Vector3 mousePos = GetMouseWorldPosition() + offset;
            transform.root.position = new Vector3(mousePos.x, transform.root.position.y, mousePos.z);
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }
}