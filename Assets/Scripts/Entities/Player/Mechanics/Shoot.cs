using System;
using System.Collections;
using EZCameraShake;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public class Shoot : MonoBehaviour
{
    [Header("Assignables")]
    [SerializeField] private Player player;

    [Header("Objects")]
    public Transform firePointLeft;
    public Transform firePointRight;
    public PlayerGunStorage gunStorage;

    [Header("Guns")]
    public PlayerGunStorage.Guns startingLeftGun = PlayerGunStorage.Guns.Default;
    public PlayerGunStorage.Guns startingRightGun = PlayerGunStorage.Guns.None;
    public PlayerGunStorage.Guns currentLeftGun;
    public PlayerGunStorage.Guns currentRightGun;
    
    public PlayerGun leftGun { get; set; }
    public PlayerGun rightGun { get; set; }
    

    [Header("Default Aim")]
    public LayerMask defaultLayerMask;
    public float defaultSpread;
    public PlayerAim defaultAim { get; private set; }

    public enum Gun
    {
        Left, Right
    }
    private void Start()
    {
        if (Player.SavedPlayer == null)
        {
            SetGun(Gun.Left, startingLeftGun);
            SetGun(Gun.Right, startingRightGun);
        }
        else
        {
            SetGun(Gun.Left, Player.SavedPlayer.leftGun);
            SetGun(Gun.Right, Player.SavedPlayer.rightGun);
        }
        
        defaultAim = new PlayerAim(player, player.shoot.firePointRight, defaultLayerMask, defaultSpread);
    }

    private void Update()
    {
        if (leftGun != null)
        {
            bool left = player.input.ShootingLeft;
            leftGun.InputCheck(left);
            
            if (left)
            {
                leftGun.currentlySelected = true;
                EventManager.Instance.Trigger("ShootingLeft");
            }
            else
            {
                leftGun.currentlySelected = false;
            }
        }
        
        if (rightGun != null)
        {
            bool right = player.input.ShootingRight;
            rightGun.InputCheck(right);
            
            if (right)
            {
                rightGun.currentlySelected = true;
                EventManager.Instance.Trigger("ShootingRight");
            }
            else
            {
                rightGun.currentlySelected = false;
            }
        }
    }

    public PlayerGun GetGun(Gun side)
    {
        switch (side)
        {
            case Gun.Left:
                return leftGun;
            case Gun.Right:
                return rightGun;
        }

        throw new Exception($"Could not find gun of type \"{side.ToString()}\"");
    }
    public void SetGun(Gun side, PlayerGunStorage.Guns gun)
    {
        SetGun(side, gunStorage.GetGun(gun));
    }
    
    public void SetGun(Gun side, PlayerGun gun)
    {
        gunStorage.TurnOffAllGuns();
        switch (side)
        {
            case Gun.Left:
                leftGun = gun;
                currentLeftGun = gunStorage.GetGunEnum(gun);
                if (leftGun != null)
                {
                    leftGun.Initialize(player);
                    GameStats.leftGunDMG = gun.damage;
                }
                else
                {
                    GameStats.leftGunDMG = 0;
                }
                break;
            case Gun.Right:
                rightGun = gun;
                
                currentRightGun = gunStorage.GetGunEnum(gun);
                if (rightGun != null)
                {
                    rightGun.Initialize(player);
                    GameStats.rightGunDMG = gun.damage;
                }
                else
                {
                    GameStats.rightGunDMG = 0;
                }
                break;
        }
        
        leftGun?.gameObject.SetActive(true);
        rightGun?.gameObject.SetActive(true);
        UIExtraInfoScreen.i.UpdateStats();
        EventManager.Instance.Trigger("UpdatedPlayerGuns", player);
    }
}

#region CUSTOM_EDITOR
#if UNITY_EDITOR
[CustomEditor(typeof(Shoot))]
public class KamCustomEditor_Shoot : KamCustomEditor
{
    private Shoot editorTarget;
    private void OnEnable()
    {
        editorTarget = (Shoot)target;
    }
    
    public override void GameDesignerInspector()
    {
        EditorGUILayout.LabelField("Starting Guns", EditorStyles.centeredGreyMiniLabel);
        editorTarget.startingLeftGun = (PlayerGunStorage.Guns) EditorGUILayout.EnumPopup(new GUIContent(
            "Left",
            "The gun the player starts with in their left gun slot."),
            editorTarget.startingLeftGun);
        
        editorTarget.startingRightGun = (PlayerGunStorage.Guns) EditorGUILayout.EnumPopup(new GUIContent(
                "Right",
                "The gun the player starts with in their right gun slot."),
            editorTarget.startingRightGun);
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Default aiming parameters", EditorStyles.centeredGreyMiniLabel);
        
        editorTarget.defaultLayerMask = EditorGUILayout.MaskField(
            new GUIContent("Shootable Mask",
                "These layers are the ones that you will be able to aim at."),
            InternalEditorUtility.LayerMaskToConcatenatedLayersMask(editorTarget.defaultLayerMask), InternalEditorUtility.layers);
        
        editorTarget.defaultSpread = EditorGUILayout.FloatField(
            new GUIContent(
                "Spread",
                "This is the default shooting spread."),
            editorTarget.defaultSpread);
    }
}
#endif
#endregion