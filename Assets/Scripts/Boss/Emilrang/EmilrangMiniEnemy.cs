using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EmilrangMiniEnemy : MonoBehaviour, IDamageable
{
    public int maxHP;
    private int CurrentHealth;

    public int damage;
    public int speed = 2;
    public float timeBeforeFollow;
    public ParticleSystem deathParticles;
    public LayerMask playerMask;
    private Player _player;
    
    
    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.Subscribe("SendPlayerReference", GetPlayerReference);
        EventManager.Instance.Trigger("NeedsPlayerReference");

        CurrentHealth = maxHP;
    }
    
    void Update()
    {
        MoveForward(speed);
        timeBeforeFollow -= Time.deltaTime;
        if (timeBeforeFollow <= 0)
        {
            transform.LookAt(_player.transform);
        }
    }
    
    private void GetPlayerReference(params object[] p)
    {
        _player = (Player)p[0];
    }
    
    public void MoveForward(float speed)
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        if(CurrentHealth <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        Instantiate(deathParticles, transform.position, Quaternion.identity, transform.parent);
        Destroy(gameObject);
    }
    
    public Transform GetTransform()
    {
        return transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerMask.CheckLayer(other.gameObject.layer))
        {
            Death();
            _player.TakeDamage(damage);
        }
    }
}
