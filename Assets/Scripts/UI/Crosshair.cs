using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Crosshair : MonoBehaviour
{
    public static Crosshair instance;
    [SerializeField] private Image crosshair;
    [SerializeField] private Image hit;
    [SerializeField] private float hitAnimTime = 0.25f;

    private void Start()
    {
        instance = this;
        hit.gameObject.SetActive(false);
    }

    public void Hit()
    {
        StartCoroutine(HitAnim());
    }

    IEnumerator HitAnim()
    {
        hit.gameObject.SetActive(true);
        yield return new WaitForSeconds(hitAnimTime);
        hit.gameObject.SetActive(false);
    }
}
