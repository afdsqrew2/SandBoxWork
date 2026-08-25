using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject mouseIndicator,cellingIndicator;
    [SerializeField]
    private InputMgr inputManager;
    [SerializeField]
    private Grid grid;
    
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
       EventCenter.Instance.AddEventListener<int>(E_EventType.E_LeftAreaButtonClick, LeftAreaButtonClick);
    }
    
    private void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_BuildButtonClick,StartPlacement );
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.E_LeftAreaButtonClick, LeftAreaButtonClick);
    }

    private void LeftAreaButtonClick(int buttonId)
    {
        if (buttonId == 0)
        {
            OnApplicationFocus(false);
        }
        else if (buttonId == 1)
        {
            //==1  就是load
            foreach (GameObject go  in placedGameObjects)
            {
                PoolMgr.Instance.PushObj(go);
            }
            furniData.Clear();
            placedGameObjects.Clear();

            //读取存档
            Dictionary<Vector3Int, PlacementData> dic = LocalSaverMgr.Instance.Load();
            furniData.SetData(dic);
            
            foreach (var kvp in dic)
            {
                ObjectData data = GamePlayMgr.Instance.database.objectsData[kvp.Value.ID];
                GameObject newGO = PoolMgr.Instance.GetObj(data.Name);
                newGO.transform.position = grid.CellToWorld(kvp.Value.GridPosition);
                placedGameObjects.Add(newGO);
            }
        }
        else
        {
            foreach (GameObject go  in placedGameObjects)
            {
                PoolMgr.Instance.PushObj(go);
            }
            furniData.Clear();
            placedGameObjects.Clear();
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            LocalSaverMgr.Instance.SaveFunc(furniData);
        }
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        seletedObjectIndex = GamePlayMgr.Instance.database.objectsData.FindIndex(data => data.ID == ID);
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
        GameObject newGO = PoolMgr.Instance.GetObj(GamePlayMgr.Instance.database.objectsData[seletedObjectIndex].Name);
        newGO.transform.position = grid.CellToWorld(gridPosition);

        //GridData seletedData = database.objectsData[seletedObjectIndex].ID == 0 ? floorData : furniData;
        GridData seletedData = furniData;
        seletedData.AddObjectAt(gridPosition,GamePlayMgr.Instance.database.objectsData[seletedObjectIndex],placedGameObjects.Count-1);
        
        placedGameObjects.Add(newGO);

    }

    private bool CheckPlaceValid(Vector3Int gridPosition, int selectedIndex)
    {
        //GridData selectedData = seletedObjectIndex == 0 ? floorData : furniData;
        GridData selectedData = furniData;
        return selectedData.CanPlaceObejctAt(gridPosition, GamePlayMgr.Instance.database.objectsData[selectedIndex].Size);
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

        Profiler.BeginSample("MyPieceOfCode1");
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Profiler.EndSample();

        Profiler.BeginSample("MyPieceOfCode2");

        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        Profiler.EndSample();
        Profiler.BeginSample("MyPieceOfCode3");

        bool placeValid = CheckPlaceValid(gridPosition, seletedObjectIndex);
        Profiler.EndSample();
        Profiler.BeginSample("MyPieceOfCode1");
        previewRenderer.material.color = placeValid ? Color.white : Color.red;
        mouseIndicator.transform.position = mousePosition;
        cellingIndicator.transform.position = grid.CellToWorld(gridPosition);
        Profiler.EndSample();

    }
}
