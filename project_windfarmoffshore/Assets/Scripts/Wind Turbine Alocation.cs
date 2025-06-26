using UnityEngine;
using TMPro;

public class WindTurbineAlocation : MonoBehaviour
{

    public GameObject fanPrefab;


    public int rows = 5;
    public int columns = 4;
    public float spacing = 300f;
    public Vector3 startPoint = new Vector3(455.5f, 0f, 0f);

    public WindScaleManager windScaleManager;
    public WindDirectionManager windDirectionManager;


    public TMP_Text energyText;

    private GameObject[] fans;

    void Start()
    {
        fans = new GameObject[rows * columns];

        AddFan();

    }

    void Update()
    {
        float nrg = windScaleManager.NrgProduced;
        int turbineCount = GetCurrentFanCount();
        float totalEnergy = nrg * turbineCount;

        if (energyText != null)
            energyText.text = $"{totalEnergy:F1} kW";
    }

    private int GetCurrentFanCount()
    {
        int count = 0;
        foreach (var fan in fans)
        {
            if (fan != null)
                count++;
        }
        return count;
    }

    public void AddFan()
    {

        int freeIndex = GetFirstFreeGridIndex();
        if (freeIndex == -1)
        {
            Debug.Log("Grid full!");
            return;
        }

        Vector3 spawnPosition = GetPositionFromIndex(freeIndex);
        GameObject newFan = Instantiate(fanPrefab, spawnPosition, Quaternion.identity);
        newFan.SetActive(true);
        newFan.transform.SetParent(null);

        FanDragg dragScript = newFan.GetComponent<FanDragg>();
        if (dragScript != null)
            dragScript.gridManager = this;


        fans[freeIndex] = newFan;


        Transform rotatingPart = newFan.transform.Find("Dragger/pivotsiderotation/bladespivot");
        if (windScaleManager != null && rotatingPart != null)
            windScaleManager.fans.Add(rotatingPart.gameObject);

        Transform rotatingWindPart = newFan.transform.Find("Dragger/pivotsiderotation");
        if (windDirectionManager != null && rotatingWindPart != null)
            windDirectionManager.fans.Add(rotatingWindPart.gameObject);

        Debug.Log("Added eolic to " + freeIndex);
    }

    public void RemoveFan()
    {
        for (int i = fans.Length - 1; i >= 0; i--)
        {
            if (fans[i] != null)
            {
                GameObject fanToRemove = fans[i];

                Transform part = fanToRemove.transform.Find("Dragger/pivotsiderotation/bladespivot");
                if (part != null && windScaleManager != null)
                    windScaleManager.fans.Remove(part.gameObject);

                Transform partDir = fanToRemove.transform.Find("Dragger/pivotsiderotation");
                if (partDir != null && windDirectionManager != null)
                    windDirectionManager.fans.Remove(partDir.gameObject);

                Destroy(fanToRemove);
                fans[i] = null;

                Debug.Log("Eolic removed from slot " + i);
                return;
            }
        }

        Debug.Log("No eolic to remove.");
    }

    public Vector3 GetNearestGridPosition(Vector3 worldPosition)
    {
        Vector3 local = worldPosition - startPoint;
        int col = Mathf.RoundToInt(local.x / spacing);
        int row = Mathf.RoundToInt(-local.z / spacing);

        col = Mathf.Clamp(col, 0, columns - 1);
        row = Mathf.Clamp(row, 0, rows - 1);

        return startPoint + new Vector3(col * spacing, 0f, -row * spacing);
    }

    public int GetGridIndexFromPosition(Vector3 worldPosition)
    {
        Vector3 local = worldPosition - startPoint;
        int col = Mathf.RoundToInt(local.x / spacing);
        int row = Mathf.RoundToInt(-local.z / spacing);

        if (col < 0 || col >= columns || row < 0 || row >= rows)
            return -1;

        return row * columns + col;
    }

    public bool IsGridPositionFree(Vector3 worldPosition)
    {
        int index = GetGridIndexFromPosition(worldPosition);
        if (index == -1) return false;
        return fans[index] == null;
    }

    public void SnapFanToGrid(GameObject fan, Vector3 position)
    {

        ClearFanFromGrid(fan);

        int index = GetGridIndexFromPosition(position);
        if (index >= 0 && index < fans.Length)
        {
            fans[index] = fan;
        }
    }

    public void ClearFanFromGrid(GameObject fan)
    {
        bool found = false;
        for (int i = 0; i < fans.Length; i++)
        {
            if (fans[i] == fan)
            {
                fans[i] = null;
                found = true;
                Debug.Log($" Removed Fan {i}");
                break;
            }
            else if (fans[i] != null)
            {
                Debug.Log($"[DEBUG] Slot {i} has: {fans[i].name}, compared to: {fan.name}");
            }
        }

        if (!found)
        {
            Debug.LogWarning($" Fan '{fan.name}' not finded in the grid");
        }
    }
    public int GetFirstFreeGridIndex()
    {
        for (int i = 0; i < fans.Length; i++)
        {
            if (fans[i] == null)
                return i;
        }
        return -1;
    }

    public Vector3 GetPositionFromIndex(int index)
    {
        int row = index / columns;
        int col = index % columns;
        return startPoint + new Vector3(col * spacing, 0f, -row * spacing);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < rows * columns; i++)
        {
            int row = i / columns;
            int col = i % columns;

            Vector3 pos = startPoint + new Vector3(col * spacing, 0f, -row * spacing);
            Gizmos.DrawWireCube(pos, Vector3.one * 1.5f);
        }
    }
}

