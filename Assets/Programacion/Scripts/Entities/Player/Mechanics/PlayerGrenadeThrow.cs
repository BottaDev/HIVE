using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public class PlayerGrenadeThrow : UnlockableMechanic
{
    [Header("Energy")]
    public float energyCost;
    [SerializeField] private string energyErrorMessage;
    [SerializeField] private Color energyErrorMessageColor;
    [SerializeField] private float energyErrorTimeOnScreen;
    
    [Header("Cooldown")]
    public float cooldown = 3f;
    [SerializeField] private string cooldownErrorMessage;
    [SerializeField] private Color cooldownErrorMessageColor;
    [SerializeField] private float cooldownErrorTimeOnScreen;
    private float _currentCooldown;
    
    [Header("Assignables")]
    public Player player;

    public Transform orientation;
    public Transform firePoint;
    public Grenade grenade;

    [Header("Throw Settings")]
    
    public float forwardForce;
    public float upwardsForce;
    
    [Header("Grenade Settings")]
    public float explosionTimeDelay = 3f;
    public int damage = 2;
    public float explosionRadius = 2f;
    public bool explodeOnContact = true;
    public LayerMask hitMask;

    public bool useAddForce = false;
    public float explosionForce;
    public float explosionUpwardsForce;



    private bool readyToThrow;

    private void Start()
    {
        base.Start();
        readyToThrow = true;
    }

    private void Update()
    {
        if (!mechanicUnlocked) return;
        
        if (player.input.GrenadeThrow)
        {
            if (!readyToThrow)
            {
                //Still on cd
                EventManager.Instance.Trigger("OnSendUIMessageTemporary", 
                    cooldownErrorMessage, 
                    cooldownErrorMessageColor, 
                    cooldownErrorTimeOnScreen);
            }
            else
            {
                if (!player.energy.TakeEnergy(energyCost))
                {
                    EventManager.Instance.Trigger("OnSendUIMessageTemporary", energyErrorMessage, energyErrorMessageColor, energyErrorTimeOnScreen);
                    return;
                }
                Throw();
            }
        }
    }

    public void Throw()
    {
        AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.GrenadeThrow));

        Grenade obj = Instantiate(grenade, firePoint.position, Quaternion.identity)
            .SetParameters(explosionRadius, damage, explosionTimeDelay, explodeOnContact, hitMask)
            .SetAddForceToRigidbodies(useAddForce, explosionForce, explosionUpwardsForce);

        Rigidbody rb = obj.rb;
        Vector3 forwardForce = orientation.forward * this.forwardForce;
        Vector3 upwardsForce = orientation.up * this.upwardsForce;
        rb.AddForce(forwardForce + upwardsForce, ForceMode.Impulse);
        
        readyToThrow = false;
        EventManager.Instance.Trigger("OnPlayerGrenadeCd", cooldown);
        Invoke(nameof(ResetThrow), cooldown);
    }
    
    void ResetThrow()
    {
        readyToThrow = true;
    }
}

#region CUSTOM_EDITOR
#if UNITY_EDITOR
[CustomEditor(typeof(PlayerGrenadeThrow))]
public class KamCustomEditor_PlayerGrenadeThrow : KamCustomEditor
{
    private PlayerGrenadeThrow editorTarget;
    private void OnEnable()
    {
        editorTarget = (PlayerGrenadeThrow)target;
    }
    
    public override void GameDesignerInspector()
    {
        editorTarget.unlockedAtTheStart = EditorGUILayout.Toggle(
            new GUIContent("Start Unlock",
                "This boolean determines if this is unlocked by default."),
            editorTarget.unlockedAtTheStart);
        
        EditorGUILayout.LabelField("General", EditorStyles.centeredGreyMiniLabel);

        editorTarget.energyCost = EditorGUILayout.FloatField(
            new GUIContent(
                "Cost",
                "This is the energy cost of casting dash."),
            editorTarget.energyCost);
        
        editorTarget.cooldown = EditorGUILayout.FloatField(
            new GUIContent(
                "Cooldown",
                "Time it takes for hook to be able to be cast again. It starts the moment the hook comes back to you."),
            editorTarget.cooldown);


        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Throw Parameters", EditorStyles.centeredGreyMiniLabel);
        
        EditorGUILayout.BeginHorizontal();
        
        EditorGUILayout.LabelField(
            new GUIContent(
            "Throw Force",
            "This is the force with which the player throws the grenade."),GUILayout.MinWidth(120));
        
        EditorGUILayout.LabelField(
            new GUIContent(
                "→",
                "This is the forward force."), GUILayout.MaxWidth(15));
        editorTarget.forwardForce = EditorGUILayout.FloatField(editorTarget.forwardForce);
        
        EditorGUILayout.LabelField(
            new GUIContent(
                "↑",
                "This is the upwards force."), GUILayout.MaxWidth(15));
        editorTarget.upwardsForce = EditorGUILayout.FloatField(editorTarget.upwardsForce);
        EditorGUILayout.EndHorizontal();
        
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Grenade Parameters", EditorStyles.centeredGreyMiniLabel);
        
        editorTarget.hitMask = EditorGUILayout.MaskField(
            new GUIContent("Hit Mask",
                "This mask is what layers will trigger the grenade's explosion."),
            InternalEditorUtility.LayerMaskToConcatenatedLayersMask(editorTarget.hitMask), InternalEditorUtility.layers);
        
        EditorGUILayout.BeginHorizontal();
        editorTarget.explosionTimeDelay = EditorGUILayout.FloatField(
            new GUIContent(
                "Delay",
                "The grenade will wait this amount of time before exploding. The boolean is for if you want it to explode immediately on contact with an enemy or not. (Enemies are determined by the hit mask above)"),
            editorTarget.explosionTimeDelay);
        editorTarget.explodeOnContact = EditorGUILayout.Toggle(editorTarget.explodeOnContact, GUILayout.Width(15));
        EditorGUILayout.EndHorizontal();
        
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Explosion Parameters", EditorStyles.centeredGreyMiniLabel);
        
        editorTarget.damage = EditorGUILayout.IntField(
            new GUIContent(
                "Damage",
                "In case of the grenade not hitting anything that will make it explode, it will wait this amount of time before exploding."),
            editorTarget.damage);
        
        editorTarget.explosionRadius = EditorGUILayout.FloatField(
            new GUIContent(
                "Explosion Radius",
                "The radius of the explosion."),
            editorTarget.explosionRadius);
        
        
        EditorGUILayout.BeginHorizontal();
        editorTarget.useAddForce = EditorGUILayout.Toggle(editorTarget.useAddForce, GUILayout.Width(15));
        EditorGUILayout.LabelField(
            new GUIContent(
                "Explosion Force",
                "This is the force that gets applied to every object within the explosion IF the boolean to the left is set to true."),GUILayout.MinWidth(120));
        
        EditorGUILayout.LabelField(
            new GUIContent(
                "→",
                "This is the forward force."), GUILayout.MaxWidth(15));
        editorTarget.explosionForce = EditorGUILayout.FloatField(editorTarget.explosionForce);
        
        EditorGUILayout.LabelField(
            new GUIContent(
                "↑",
                "This is the upwards force."), GUILayout.MaxWidth(15));
        editorTarget.explosionUpwardsForce = EditorGUILayout.FloatField(editorTarget.explosionUpwardsForce);
        EditorGUILayout.EndHorizontal();
    }
}
#endif
#endregion