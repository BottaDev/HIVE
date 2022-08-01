using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingPoint : MonoBehaviour
{
    public Transform firePoint;
    public GameObject prefab;
    public bool setRotation;
    //public SFXs soundEffect;
    public void Instantiate()
    {
        GameObject obj = Instantiate(prefab);
        obj.transform.position = firePoint.position;
        obj.transform.rotation = firePoint.rotation;
        
        //AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(soundEffect));
    }
}
