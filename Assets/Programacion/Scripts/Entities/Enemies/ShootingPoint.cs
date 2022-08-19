using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingPoint : MonoBehaviour
{
    public Transform firePoint;
    public GameObject prefab;
    public bool setRotation;
    public System.Action<GameObject> action = delegate(GameObject o) {  };
    //public SFXs soundEffect;
    public void Instantiate()
    {
        GameObject obj = Instantiate(prefab);
        obj.transform.position = firePoint.position;
        obj.transform.rotation = firePoint.rotation;
        action.Invoke(obj);

        //AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(soundEffect));
    }
}
