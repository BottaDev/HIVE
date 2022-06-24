using UnityEngine;

public class PoolableObject : MonoBehaviour
{
    public ObjectPool Parent;

    public virtual void OnDisable()
    {
        try { Parent.ReturnObjectToPool(this); } catch{ }
    }
}