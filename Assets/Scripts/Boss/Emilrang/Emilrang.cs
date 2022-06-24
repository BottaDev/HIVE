using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emilrang : Entity
{
    private Rigidbody rb;
    public Player player;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveUp();

        if (transform.position.y >= 40)
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
            Destroy(gameObject);
        }
    }
}
