using System;
using UnityEngine;

public class PlayerGrenadeThrow : MonoBehaviour
{
    public Transform cam;
    public Transform firePoint;
    public Rigidbody grenade;

    public float forwardForce;
    public float upwardsForce;
    private bool readyToThrow;
    public float throwCD;
    
    private bool mechanicActivated = true;
    public bool MechanicActivated { get => mechanicActivated; set => mechanicActivated = value; }

    private void Start()
    {
        readyToThrow = true;
    }

    public void Throw()
    {
        if (MechanicActivated && readyToThrow)
        {
            Rigidbody rb = Instantiate(grenade,firePoint.position,Quaternion.identity);
            Vector3 forwardForce = cam.forward * this.forwardForce;
            Vector3 upwardsForce = firePoint.up * this.upwardsForce;
            rb.AddForce(forwardForce + upwardsForce, ForceMode.Impulse);
        
            readyToThrow = false;
            Invoke(nameof(ResetThrow), throwCD);
        }
    }
    
    void ResetThrow()
    {
        readyToThrow = true;
    }
}
