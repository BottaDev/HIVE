using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : MonoBehaviour
{
    public float speed = 10f;
    private float damage = 1f;
    
    private void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            Entity enemy = other.gameObject.GetComponent<Entity>();
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if(other.gameObject.layer == 6)
            Destroy(gameObject);
    }
}
