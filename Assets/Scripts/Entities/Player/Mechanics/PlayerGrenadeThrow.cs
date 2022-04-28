using System;
using UnityEngine;

public class PlayerGrenadeThrow : UnlockableMechanic
{
    [Header("Energy")]
    [SerializeField] private float energyCost;
    [SerializeField] private string energyErrorMessage;
    [SerializeField] private Color energyErrorMessageColor;
    [SerializeField] private float energyErrorTimeOnScreen;
    
    [Header("Cooldown")]
    [SerializeField] private float cooldown = 3f;
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
                EventManager.Instance.Trigger(EventManager.Events.OnSendUIMessageTemporary, 
                    cooldownErrorMessage, 
                    cooldownErrorMessageColor, 
                    cooldownErrorTimeOnScreen);
            }
            else
            {
                if (!player.energy.TakeEnergy(energyCost))
                {
                    EventManager.Instance.Trigger(EventManager.Events.OnSendUIMessageTemporary, energyErrorMessage, energyErrorMessageColor, energyErrorTimeOnScreen);
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
        EventManager.Instance.Trigger(EventManager.Events.OnPlayerGrenadeCd, cooldown);
        Invoke(nameof(ResetThrow), cooldown);
    }
    
    void ResetThrow()
    {
        readyToThrow = true;
    }
}
