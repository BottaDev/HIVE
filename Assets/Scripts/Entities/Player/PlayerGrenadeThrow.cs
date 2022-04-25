using System;
using UnityEngine;

public class PlayerGrenadeThrow : MonoBehaviour
{
    public Player player;

    public Transform orientation;
    public Transform firePoint;
    public Grenade grenade;

    [Header("Throw Settings")]
    public float forwardForce;
    public float upwardsForce;
    public float throwCD;

    [Header("Grenade Settings")]
    public float explosionTimeDelay = 3f;
    public float damage = 2f;
    public float explosionRadius = 2f;
    public bool explodeOnContact = true;
    public LayerMask hitMask;



    private bool readyToThrow;
    [SerializeField] private bool mechanicActivated = false;
    public bool MechanicActivated { get => mechanicActivated; set => mechanicActivated = value; }

    private void Start()
    {
        readyToThrow = true;
    }

    private void Update()
    {
        if (player.input.GrenadeThrow && MechanicActivated && readyToThrow)
        {
            Throw();
        }
    }

    public void Throw()
    {
        AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.GrenadeThrow));

        Grenade obj = Instantiate(grenade, firePoint.position, Quaternion.identity);
        obj.SetParameters(explosionRadius, damage, explosionTimeDelay, explodeOnContact, hitMask);

        Rigidbody rb = obj.rb;
        Vector3 forwardForce = orientation.forward * this.forwardForce;
        Vector3 upwardsForce = orientation.up * this.upwardsForce;
        rb.AddForce(forwardForce + upwardsForce, ForceMode.Impulse);
        
        readyToThrow = false;
        Invoke(nameof(ResetThrow), throwCD);
    }
    
    void ResetThrow()
    {
        readyToThrow = true;
    }
}
