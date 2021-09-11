using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 3;
    public Player player;

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
            StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        player.experience++;

        yield return new WaitForSeconds(0.2f);

        Destroy(gameObject);
    }
}
