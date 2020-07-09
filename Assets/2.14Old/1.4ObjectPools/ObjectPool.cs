using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public PooledObject objPrefab;
    List<PooledObject> availableObjects = new List<PooledObject>();
    internal void AddObject(PooledObject pooledObject)
    {
        availableObjects.Add(pooledObject);
        pooledObject.gameObject.SetActive(false);
    }

    public PooledObject GetObject()
    {
        PooledObject pooledObject;
        int lastAvailableIndex = availableObjects.Count - 1;
        if (lastAvailableIndex >= 0)
        {
            pooledObject = availableObjects[lastAvailableIndex];
            availableObjects.RemoveAt(lastAvailableIndex);
            pooledObject.gameObject.SetActive(true);
        }
        else
        {
            pooledObject = Instantiate<PooledObject>(objPrefab);
            pooledObject.transform.SetParent(transform, false);
            pooledObject.pool = this;
        }
        return pooledObject;
    }

    internal static ObjectPool GetPool(PooledObject pooledObject)
    {
        GameObject obj;
        ObjectPool pool;
        if (Application.isEditor)
        {
            obj = GameObject.Find(pooledObject.name + " Pool");
            if (obj)
            {
                pool = obj.GetComponent<ObjectPool>();
                if (pool)
                {
                    return pool;
                }
            }
        }
        obj = new GameObject(pooledObject.name + " Pool");
        DontDestroyOnLoad(obj);

        pool = obj.AddComponent<ObjectPool>();
        pool.objPrefab = pooledObject;
        return pool;
    }
}