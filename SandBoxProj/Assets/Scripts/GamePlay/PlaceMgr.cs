using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject mouseIndicator,cellingIndicator;
    [SerializeField]
    private InputMgr inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDatabaseSO database;

    private int seletedObjectIndex = -1;

    [SerializeField]
    private GameObject gridVisualization;

    private GridData floorData, furniData;
    private Renderer previewRenderer;

    private List<GameObject> placedGameObjects = new List<GameObject>();
    private void Start()
    {
        StopPlacement();
        floorData = new GridData();
        furniData = new GridData();
        previewRenderer = cellingIndicator.GetComponentInChildren<Renderer>();
    }

    private void OnEnable()
    {
       EventCenter.Instance.AddEventListener<int>(E_EventType.E_BuildButtonClick, StartPlacement);
    }
    
    private void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_BuildButtonClick,StartPlacement );
    }
    
    public void StartPlacement(int ID)
    {
        StopPlacement();
        seletedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if (seletedObjectIndex < 0)
        {
            Debug.LogError($"no id found {ID}");
        }
        gridVisualization.gameObject.SetActive(true);
        cellingIndicator.SetActive(true);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if(inputManager.IsPointerOverUI())
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        bool placeValid = CheckPlaceValid(gridPosition, seletedObjectIndex);
        if (!placeValid)
        {
            return;
        }
        
        //GameObject newGO = Instantiate(database.objectsData[seletedObjectIndex].Prefab);
        GameObject newGO = PoolMgr.Instance.GetObj(database.objectsData[seletedObjectIndex].Name);
        newGO.transform.position = grid.CellToWorld(gridPosition);

        //GridData seletedData = database.objectsData[seletedObjectIndex].ID == 0 ? floorData : furniData;
        GridData seletedData = furniData;
        seletedData.AddObjectAt(gridPosition,database.objectsData[seletedObjectIndex].Size,
            database.objectsData[seletedObjectIndex].ID,placedGameObjects.Count-1);
        
        placedGameObjects.Add(newGO);

    }

    private bool CheckPlaceValid(Vector3Int gridPosition, int selectedIndex)
    {
        //GridData selectedData = seletedObjectIndex == 0 ? floorData : furniData;
        GridData selectedData = furniData;
        return selectedData.CanPlaceObejctAt(gridPosition, database.objectsData[selectedIndex].Size);
    }

    private void StopPlacement()
    {
        //throw new NotImplementedException();
        seletedObjectIndex = -1;
        gridVisualization.gameObject.SetActive(false);
        cellingIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
    }


    private void Update()
    {
        if (seletedObjectIndex < 0)
        {
            return;
        }

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        
        bool placeValid = CheckPlaceValid(gridPosition, seletedObjectIndex);

        previewRenderer.material.color = placeValid ? Color.white : Color.red;
        
        mouseIndicator.transform.position = mousePosition;
        cellingIndicator.transform.position = grid.CellToWorld(gridPosition);
    }
}
