using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class LocalSaverMgr  : SingletonMono<LocalSaverMgr>
{
    private PlacementSaveHelper mSaveHelper = new PlacementSaveHelper();

    private void Start()
    {
        mSaveHelper.Init( Path.Combine(Application.persistentDataPath, "placement_dict.json"));
    }
    
    public void SaveFunc(GridData data)
    {
        Dictionary<Vector3Int, PlacementData> dic = data.GetPlacedObjects();
        mSaveHelper.Save(dic);
    }

    public Dictionary<Vector3Int, PlacementData>  Load()
    {
        return mSaveHelper.Load();
    }

}
