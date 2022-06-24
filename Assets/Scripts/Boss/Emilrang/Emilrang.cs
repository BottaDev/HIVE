using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Emilrang : Entity
{
    private Rigidbody rb;
    public Player player;
    public float speed;
    public float attackEvery;
    public int projectileAmount;
    public BulletHellAttack[] attackPoints;
    public UnityEvent onDeath;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(AttackCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        MoveUp();

        if (transform.position.y >= 215)
        {
            speed = 0;
        }

        var playerRigidbody = player.GetComponent<Rigidbody>();

    }

    public void MoveUp()
    {
        transform.position += transform.up * speed * Time.deltaTime;
    }

    public override void TakeDamage(int damage)
    {
        CurrentHealth -= damage;

        if(CurrentHealth <= 0)
        {
            onDeath.Invoke();
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    IEnumerator AttackCoroutine()
    {
        while (true)
        {
            Attack();
            yield return new WaitForSeconds(attackEvery);
        }
        
    }

    void Attack()
    {
        foreach (var point in attackPoints)
        {
            point.SpawnProjectile_KamVersion();
        }
    }
}
