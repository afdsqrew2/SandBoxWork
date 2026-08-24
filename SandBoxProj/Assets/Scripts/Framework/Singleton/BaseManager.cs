using System;
using System.Reflection;
using UnityEngine;

public abstract class BaseManager<T> where T:class//,new()
{
    private static T instance;
    protected bool InstanceisNull => instance == null;
    protected static readonly object lockObj = new object();
    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                lock (lockObj)
                {
                    if (instance == null)
                    {
                        Type type = typeof(T);
                        ConstructorInfo info = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
                            null,
                            Type.EmptyTypes,
                            null);
                        if (info != null)
                            instance = info.Invoke(null) as T;
                        else
                            Debug.LogError("没有得到对应的无参构造函数");
                    }
                }
            }
            return instance;
        }
    }
}