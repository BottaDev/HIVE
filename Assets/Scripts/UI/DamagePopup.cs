using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private static int sortingOrder;
    private static Canvas canvas;
    public static DamagePopup Create(Vector3 position, float damageAmount)
    {
        if (canvas == null)
        {
            canvas = Instantiate(AssetDatabase.i.instanceCanvas);
        }
        
        Vector3 newPos = Camera.main.ScreenToWorldPoint(position);
        DamagePopup popup = Instantiate(AssetDatabase.i.damagePopup, canvas.transform);
        popup.transform.localPosition = newPos;
        
        popup.Initialize(damageAmount);

        return popup;
    }

    [SerializeField] private TextMeshProUGUI txt;
    [SerializeField] private Vector2 moveBase;
    [SerializeField] private float moveSpeed;
    private Vector3 moveVector;
    
    [SerializeField] private float disappearTimer;
    [SerializeField] private float dissapearSpeed;

    private const float DISAPPEAR_TIMER_MAX = 1f;
    
    private Color textColor;
    public void Initialize(float damageAmount)
    {
        txt.text = damageAmount.ToString();
        textColor = txt.color;
        sortingOrder++;
        moveVector = moveBase * moveSpeed;
    }

    private void Update()
    {
        transform.localPosition += moveVector * Time.deltaTime;
        moveVector -= moveVector * 8f * Time.deltaTime;

        if (disappearTimer > DISAPPEAR_TIMER_MAX * .5f)
        {
            float increaseScaleAmount = 1f;
            transform.localPosition += Vector3.one * increaseScaleAmount * Time.deltaTime;
        }
        else
        {
            float decreaseScaleAmount = 1f;
            transform.localPosition -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        }
        
        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            textColor.a -= dissapearSpeed * Time.deltaTime;
            txt.color = textColor;

            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
