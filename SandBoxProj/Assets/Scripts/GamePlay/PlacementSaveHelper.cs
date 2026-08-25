using System.Collections.Generic;
using System.IO;
using UnityEngine;

// 用来序列化Vector3Int
[System.Serializable]
public struct Vec3IntSave
{
    public int x;
    public int y;
    public int z;

    public Vec3IntSave(Vector3Int v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public Vector3Int ToVector3Int()
    {
        return new Vector3Int(x, y, z);
    }
}

// 序列化PlacementData，JsonUtility只认public字段
[System.Serializable]
public class PlacementDataSave
{
    public List<Vec3IntSave> occupiedPositions;
    public int ID;
    public int PlacedObjectIndex;
    public Vec3IntSave gridPosition;

    // 从原始数据转保存对象
    public static PlacementDataSave FromOriginal(PlacementData data)
    {
        var save = new PlacementDataSave();
        save.ID = data.ID;
        save.PlacedObjectIndex = data.PlacedObjectIndex;

        save.occupiedPositions = new List<Vec3IntSave>();
        foreach (var pos in data.occupiedPositions)
        {
            save.occupiedPositions.Add(new Vec3IntSave(pos));
        }

        save.gridPosition = new Vec3IntSave(data.GridPosition);
        
        return save;
    }

    // 转回业务使用的PlacementData
    public PlacementData ToOriginal()
    {
        // 注意：原类ID、PlacedObjectIndex是private set，需要改造PlacementData！
        List<Vector3Int> posList = new List<Vector3Int>();
        foreach (var s in occupiedPositions)
        {
            posList.Add(s.ToVector3Int());
        }
        var data = new PlacementData(posList, ID, PlacedObjectIndex, gridPosition.ToVector3Int());
        return data;
    }
}

[System.Serializable]
public class DictItem
{
    public Vec3IntSave key;
    public PlacementDataSave value;
}
[System.Serializable]
public class PlacementDictWrapper
{
    public List<DictItem> items;
}

public class PlacementSaveHelper
{
    private  string _savePath;
    
    /// <summary>
    /// 保存 Dictionary<Vector3Int, PlacementData>
    /// </summary>
    public void Save(Dictionary<Vector3Int, PlacementData> dict)
    {
        if (dict.Count == 0)
        {
            Debug.Log($"无需保存 数据为0");

            return;
        }

        PlacementDictWrapper wrapper = new PlacementDictWrapper();
        wrapper.items = new List<DictItem>();

        foreach (var kvp in dict)
        {
            wrapper.items.Add(new DictItem
            {
                key = new Vec3IntSave(kvp.Key),
                value = PlacementDataSave.FromOriginal(kvp.Value)
            });
        }

        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(_savePath, json);
        Debug.Log($"保存完成：{_savePath}");
    }

    /// <summary>
    /// 读取回 Dictionary<Vector3Int, PlacementData>
    /// </summary>
    public Dictionary<Vector3Int, PlacementData> Load()
    {
        if (!File.Exists(_savePath))
        {
            Debug.LogWarning("存档文件不存在");
            return new Dictionary<Vector3Int, PlacementData>();
        }

        string json = File.ReadAllText(_savePath);
        var wrapper = JsonUtility.FromJson<PlacementDictWrapper>(json);
        var resultDict = new Dictionary<Vector3Int, PlacementData>();

        foreach (var item in wrapper.items)
        {
            Vector3Int key = item.key.ToVector3Int();
            PlacementData val = item.value.ToOriginal();
            resultDict[key] = val;
        }
        return resultDict;
    }

    public void Init(string savePath)
    {
        _savePath = savePath;

    }
}