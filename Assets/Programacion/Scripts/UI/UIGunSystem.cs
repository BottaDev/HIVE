using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct UISlotSprites
{
    public Sprite icon;
    public Sprite background;
}

[System.Serializable]
public struct UISlot
{
    public GameObject showObjects;
    public Image background;
    public Image icon;

    public void SetIcon(Sprite icon)
    {
        if (icon != null)
        {
            this.icon.gameObject.SetActive(true);
            this.icon.sprite = icon;
        }
        else
        {
            this.icon.gameObject.SetActive(false);
        }
    }
    
    public void SetBackground(Sprite background)
    {
        if (background != null)
        {
            this.background.gameObject.SetActive(true);
            this.background.sprite = background;
        }
        else
        {
            this.background.gameObject.SetActive(false);
        }
    }
    
    public void SetActive(bool state)
    {
        showObjects.SetActive(state);
    }
}

public class UIGunSystem : MonoBehaviour
{
    [System.Serializable]
    public class UIGun
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

            foreach (var skill in skills)
            {
                skill.keybind.gameObject.SetActive(state);
            }
        }

        public void SetActive(bool state)
        {
            gun.SetActive(state);
            foreach (var skill in skills)
            {
                skill.slot.SetActive(state);
            }
        }
    }

    [System.Serializable]
    public struct UISkill
    {
        public string name;
        public UISlot slot;
        public TextMeshProUGUI keybind;

        public void SetSlot(Sprite background, Sprite icon)
        {
            slot.SetBackground(background);
            slot.SetIcon(icon);

            if (!slot.background.gameObject.activeSelf && !slot.icon.gameObject.activeSelf)
            {
                keybind.gameObject.SetActive(false);
            }
            else
            {
                keybind.gameObject.SetActive(true);
            }
        }
    }

    public UIGun left;
    public UIGun right;
    
    public Guns currentlySelected = Guns.right;
    
    public enum Guns
    {
        left, right
    }

    private void Awake()
    {
        EventManager.Instance.Subscribe("UpdatedPlayerGuns",UpdateGuns);
        EventManager.Instance.Subscribe("ShootingLeft",SelectLeft);
        EventManager.Instance.Subscribe("ShootingRight",SelectRight);
    }

    private void Start()
    {
        SelectGun(currentlySelected);
    }

    void SelectLeft(params object[] obj)
    {
        SelectGun(Guns.left);
    }
    void SelectRight(params object[] obj)
    {
        SelectGun(Guns.right);
    }
    public void SelectGun(Guns gun)
    {
        if (gun == currentlySelected) return;
        
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

    public void SetGun(Guns gunSide, PlayerGun gun)
    {
        UIGun side = null;
        switch (gunSide)
        {
            case Guns.left:
                side = left;
                break;
            case Guns.right:
                side = right;
                break;
        }


        if (gun == null)
        {
            side.SetActive(false);
        }
        else
        {
            side.SetActive(true);
            side.gun.SetBackground(gun.gunSprites.background);
            side.gun.SetIcon(gun.gunSprites.icon);
            side.skills[0].SetSlot(gun.skill1Sprites.background, gun.skill1Sprites.icon);
            side.skills[1].SetSlot(gun.skill2Sprites.background, gun.skill2Sprites.icon);
        }
    }
    public void UpdateGuns(params object[] obj)
    {
        Player player = (Player)obj[0];
        SetGun(Guns.left, player.shoot.leftGun);
        SetGun(Guns.right, player.shoot.rightGun);
    }
}
