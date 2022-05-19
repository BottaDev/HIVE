using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct UISlot
{
    public Image background;
    public Image icon;

    public void SetIcon(Sprite icon)
    {
        this.icon.sprite = icon;
    }
    
    public void SetBackground(Sprite background)
    {
        this.background.sprite = background;
    }
}

public class UIGunSystem : MonoBehaviour
{
    [System.Serializable]
    public struct UIGun
    {
        public string name;
        public UISlot gun;
        public List<UISkill> skills;
        public List<GameObject> selection;

        public void SetSelected(bool state)
        {
            foreach (var selectedObj in selection)
            {
                selectedObj.gameObject.SetActive(state);
            }
        }
    }

    [System.Serializable]
    public struct UISkill
    {
        public string name;
        public UISlot slot;
        public TextMeshProUGUI keybind;
    }

    public UIGun left;
    public UIGun right;
    
    public Guns currentlySelected = Guns.right;
    
    public enum Guns
    {
        left, right
    }

    private void Start()
    {
        SelectGun(currentlySelected);
    }

    public void SelectGun(Guns gun)
    {
        currentlySelected = gun;
        
        switch (gun)
        {
            case Guns.left:
                left.SetSelected(true);
                right.SetSelected(false);
                break;
            case Guns.right:
                left.SetSelected(false);
                right.SetSelected(true);
                break;
        }
    }
}
