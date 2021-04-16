using System;
using UnityEngine;
using System.Collections.Generic;

public class ObjectPooling : MonoBehaviour
{
    private readonly Dictionary<int, Pool> _pools = new Dictionary<int, Pool>();
    
    public static ObjectPooling Singleton;

    private void Awake()
    {
        Singleton = this;
    }

    public GameObject[] GeneratePool(Pool p)
    {
        var key = p.objectToPool.GetInstanceID();
        if (!_pools.ContainsKey(key))
        {
            p.Init();
            _pools.Add(key, p);
        }
        return _pools[key].PooledObjects.ToArray();
    }
    
    public GameObject GetFromPool(GameObject obj)
    {
        var key = obj.GetInstanceID();
        if (!_pools.ContainsKey(key))
            return null;
        
        var o = _pools[key].PooledObjects.Dequeue();
        _pools[key].PooledObjects.Enqueue(o);
        o.SetActive(true);
        return o;
    }
    
    public void AddToPool(GameObject obj)
    {
        var key = obj.GetInstanceID();
        if (!_pools.ContainsKey(key))
            return;
        
        obj.SetActive(false);
        _pools[key].PooledObjects.Enqueue(obj);
    }

    [Serializable]
    public class Pool
    {
        public GameObject objectToPool;
        public int poolSize;
        public Transform parent;
        public Queue<GameObject> PooledObjects;

        public Pool(GameObject objectToPool, int poolSize, Transform parent = null)
        {
            this.objectToPool = objectToPool;
            this.poolSize = poolSize;
            this.parent = parent;
        }
        
        public void Init()
        {
            PooledObjects = new Queue<GameObject>();
            for (int i = 0; i < poolSize; i++)
            {
                var obj = Instantiate(objectToPool, parent);
                PooledObjects.Enqueue(obj);
                obj.SetActive(false);
            }
        }
    }
}