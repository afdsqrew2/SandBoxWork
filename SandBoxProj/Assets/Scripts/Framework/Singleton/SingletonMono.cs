using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get { return instance; }
    }

    protected virtual void Awake()
    {
        //已经存在一个对应的单例模式对象了 不需要在有一个了
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this as T;
        DontDestroyOnLoad(this.gameObject);
    }
}
