using UnityEngine;

public class GamePlayMgr  : SingletonMono<GamePlayMgr>
{
    [SerializeField]
    public ObjectsDatabaseSO database;
    

    // Start is called before the first frame update
    void Start()
    {
        PoolMgr.isOpenLayout = true;

        for (int i = 0; i < database.objectsData.Count; i++)
        {
            ObjectData data = database.objectsData[i];
            PoolMgr.Instance.WarmUp(data.Name, data.Prefab, data.WarmUpCount);
        }
    }

}
