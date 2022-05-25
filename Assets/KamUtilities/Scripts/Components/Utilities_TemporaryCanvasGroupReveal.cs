using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities_TemporaryCanvasGroupReveal : MonoBehaviour
{
    [SerializeField] private CanvasGroup obj;
    [SerializeField] private float revealTime;
    [SerializeField] private float decreasingAlphaValue;

    public void Reveal()
    {
        StopAllCoroutines();
        StartCoroutine(RevealCoroutine());
    }

    private IEnumerator RevealCoroutine()
    {
        while (obj.alpha < 1)
        {
            obj.alpha += decreasingAlphaValue;
            yield return new WaitForEndOfFrame();
        }
        
        yield return new WaitForSeconds(revealTime);
        
        while (obj.alpha > 0)
        {
            obj.alpha -= decreasingAlphaValue;
            yield return new WaitForEndOfFrame();
        }
    }
}
