using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class UIGunSight : MonoBehaviour
{
    public static UIGunSight instance;
    [SerializeField] private Image hit;
    [SerializeField] private float hitAnimTime = 0.25f;

    [Header("Sights")]
    public List<GunSights> gunSights;
    private SightTypes current;
    
    public enum SightTypes
    {
        Default, Shotgun
    }

    [System.Serializable]
    public struct GunSights
    {
        public string name;
        public SightTypes type;
        public GameObject sight;
    }
    
    private void Start()
    {
        instance = this;
        hit.gameObject.SetActive(false);
    }

    public void Hit()
    {
        StopAllCoroutines();
        StartCoroutine(HitAnim());
    }

    IEnumerator HitAnim()
    {
        hit.gameObject.SetActive(true);
        yield return new WaitForSeconds(hitAnimTime);
        hit.gameObject.SetActive(false);
    }
    
    public void SetGunsight(SightTypes type)
    {
        if (current == type) return;
        
        current = type;
        
        foreach (var sight in gunSights)
        {
            sight.sight.SetActive(false);
        }
        
        gunSights.First(x => x.type == type).sight.SetActive(true);
    }
}
