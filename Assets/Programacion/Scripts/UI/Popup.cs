using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    private static int sortingOrder;
    
    public static Popup Create(Vector3 position, string text, Color color)
    {
        Popup popup = Instantiate(AssetDatabase.i.popup, position, Quaternion.identity);
        popup.originalSize = popup.transform.localScale.x;
        popup.SetColor(color);
        popup.Initialize(text);

        return popup;
    }
    public static Popup Create(Vector3 position, string text, Color color, Vector2 moveBase)
    {
        Popup popup = Instantiate(AssetDatabase.i.popup, position, Quaternion.identity);
        popup.originalSize = popup.transform.localScale.x;
        popup.SetMovement(moveBase);
        popup.SetColor(color);
        popup.Initialize(text);

        return popup;
    }

    [SerializeField] private TextMeshProUGUI txt;
    [SerializeField] private Vector3 moveVector;
    
    [SerializeField] private float disappearTimer;
    [SerializeField] private float dissapearSpeed;

    private const float DISAPPEAR_TIMER_MAX = 1f;

    public float originalSize { get; set; }
    private Color textColor;
    public void Initialize(string text)
    {
        txt.text = text;
        textColor = txt.color;
        sortingOrder++;
    }

    public void SetColor(Color color)
    {
        txt.color = color;
    }
    public Popup ChangeSize(float size)
    {
        float scale = originalSize * size;
        transform.localScale = new Vector3(scale, scale, scale);
        return this;
    }

    public Popup SetMovement(Vector3 moveSpeed)
    {
        this.moveVector = moveSpeed;
        return this;
    }

    public void SetParent(Transform parent)
    {
        transform.parent = parent;
    }


    private void Update()
    {
        transform.localPosition += moveVector * Time.deltaTime;
        moveVector -= moveVector * 8f * Time.deltaTime;

        if (disappearTimer > DISAPPEAR_TIMER_MAX * .5f)
        {
            float increaseScaleAmount = transform.localScale.x;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        }
        else
        {
            float decreaseScaleAmount = transform.localScale.x;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
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
