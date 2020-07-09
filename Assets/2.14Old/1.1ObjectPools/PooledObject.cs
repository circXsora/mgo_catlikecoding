using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PooledObject : MonoBehaviour
{

    public ObjectPool pool;

    public void ReturnToPool()
    {
        if (pool)
        {
            pool.AddObject(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    internal T GetPooledInstance<T>() where T : PooledObject
    {
        if (!poolInstanceForPrefab)
        {
            poolInstanceForPrefab = ObjectPool.GetPool(this);
        }
        return (T)poolInstanceForPrefab.GetObject();
    }
    
    private void Start()
    {
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }
    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
    }
    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        ReturnToPool();
    }
    [System.NonSerialized]
    ObjectPool poolInstanceForPrefab;
    
}
