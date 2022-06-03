using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#region Asset Enums
public enum Songs
{
    Level1
}
public enum SFXs
{
    PlayerShot,
    GunRecharge,
    PlayerDeath,
    EnemyShot,
    EnemyDeath,
    Explosion,
    PlayerDash,
    PlayerHook,
    PlayerJump,
    GrenadeThrow
}
public enum Icons
{
    TestIcon,
    Attack,
    Defense,
    Mobility
}
#endregion
public class AssetDatabase : MonoBehaviour
{
    #region singletonSetup
    
    private static AssetDatabase instance;
    public static AssetDatabase i
    {
        get
        {
            /*
            if (instance == null)
            {
                instance = Resources.Load("AssetDatabase") as AssetDatabase;
                Initialize();
            }*/

            return instance;
        }
    }
    static void Initialize()
    {
        i.transform.parent = null;
        DontDestroyOnLoad(i);
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Initialize();
        }
    }
    #endregion

    public SFX[] sfxs;
    public Music[] music;
    public Icon[] upgradeIcons;
    public Popup popup;
    public Canvas instanceCanvas;

    public AudioClip GetSFX(SFXs name)
    {
        AudioClip clip = sfxs.First(x=> x.enumName == name).sfx;

        if (clip != null)
        {
            return clip;
        }

        throw new System.Exception("Id " + name + " not found in sfx array.");
    }
    
    public AudioClip GetSong(Songs name)
    {
        AudioClip clip = music.First(x=> x.enumName == name).music;

        if (clip != null)
        {
            return clip;
        }

        throw new System.Exception("Id " + name + " not found in music array.");
    }
    
    public Sprite GetUpgradeIcon(Icons name)
    {
        Sprite upgradeIcon = upgradeIcons.First(x=> x.enumName == name).icon;

        if (upgradeIcon != null)
        {
            return upgradeIcon;
        }

        throw new System.Exception("Id " + name + " not found in music array.");
    }
}


[System.Serializable]
public struct SFX
{
    public string name;
    public SFXs enumName;
    public AudioClip sfx;
}

[System.Serializable]
public struct Music
{
    public string name;
    public Songs enumName;
    public AudioClip music;
}

[System.Serializable]
public struct Icon
{
    public string name;
    public Icons enumName;
    public Sprite icon;
}