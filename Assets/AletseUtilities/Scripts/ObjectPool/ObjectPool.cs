using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private readonly PoolableObject _prefab;
    private readonly int _size;
    private List<PoolableObject> AvailableObjectsPool;

    private ObjectPool(PoolableObject poolablePrefab, int Size)
    {
        this._prefab = poolablePrefab;
        this._size = Size;
        AvailableObjectsPool = new List<PoolableObject>(Size);
    }

    public static ObjectPool CreateInstance(PoolableObject Prefab, int Size)
    {
        ObjectPool pool = new ObjectPool(Prefab, Size);

        GameObject poolGameObject = new GameObject(Prefab + " Pool");
        pool.CreateObjects(poolGameObject);

        return pool;
    }

    private void CreateObjects(GameObject parent)
    {
        for (int i = 0; i < _size; i++)
        {
            PoolableObject poolableObject = GameObject.Instantiate(_prefab, Vector3.zero, Quaternion.identity, parent.transform);
            poolableObject.Parent = this;
            poolableObject.gameObject.SetActive(false);
        }
    }

    public PoolableObject GetObject()
    {
        PoolableObject instance = AvailableObjectsPool[0];

        AvailableObjectsPool.RemoveAt(0);

        instance.gameObject.SetActive(true);

        return instance;
    }

    public void ReturnObjectToPool(PoolableObject Object)
    {
        AvailableObjectsPool.Add(Object);
    }
}
