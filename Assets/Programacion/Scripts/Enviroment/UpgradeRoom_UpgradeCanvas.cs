using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeRoom_UpgradeCanvas : MonoBehaviour
{
    [SerializeField] private Utilities_CanvasGroupReveal reveal;
    [SerializeField] private TextMeshProUGUI header;
    [SerializeField] private TextMeshProUGUI helper;
    
    private void Awake()
    {
        EventManager.Instance.Subscribe("OnUpgradeRoomCanvasStart", Reveal);
        EventManager.Instance.Subscribe("OnUpgradeRoomCanvasEnd", Hide);
    }

    void Reveal(params object[] obj)
    {
        if (UILevelUpgradePrompt.instance.choices.Count != 0 || UILevelUpgradePrompt.instance.current != null)
        {
            header.text = $"There are 3 choices before you. You may choose only one.";
            helper.gameObject.SetActive(true);
            reveal.Reveal();
        }
        else
        {
            header.text = $"You have no upgrades. Come back at another time, you may find something you like.";
            helper.gameObject.SetActive(false);
            reveal.RevealTemporary();
        }
    }

    IEnumerator Delay(Action action, float time)
    {
        yield return new WaitForSeconds(time);

        action?.Invoke();
    }
    void Hide(params object[] obj)
    {
        reveal.Hide();
    }
}
