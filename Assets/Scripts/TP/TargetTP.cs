using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTP : MonoBehaviour
{
    public float health = 3;
    public PlayerTP player;

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
            StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        //player.experience++;

        yield return new WaitForSeconds(0.2f);

        Destroy(gameObject);
    }
}
