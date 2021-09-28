using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : MonoBehaviour
{
    public float speed = 10f;
    public GameObject impactParticles;
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

            GameObject p = Instantiate(impactParticles, transform.position, Quaternion.identity);
            p.transform.eulerAngles = transform.eulerAngles * -1;

            Destroy(gameObject);
        }
        else if(other.gameObject.layer == 6)
        {
            GameObject p = Instantiate(impactParticles, transform.position, Quaternion.identity);
            p.transform.eulerAngles = transform.eulerAngles * -1;

            Destroy(gameObject);
        }

    }
}
