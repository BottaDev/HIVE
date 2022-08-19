using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ballrar : AI
{
    private Rigidbody rb;
    [SerializeField] int energy = 100;
    [SerializeField] int speed = 5;
    [SerializeField] int spinSpeed = 10;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected override void Attack()
    {
        if (energy >= 100)
        {
            StartCoroutine("SpinAttack");
            _agent.speed = spinSpeed;
            RotateTowards(_player.transform.position);
        }
        else
        {
            StopAllCoroutines();
            _agent.speed = speed;
        }

    }

    IEnumerator SpinAttack()
    {
        //var waitForEndOfFrame = new WaitForEndOfFrame();

        var direction = _player.transform.position - transform.position;

        rb.AddForce(direction * spinSpeed * Time.deltaTime, ForceMode.Impulse);

        yield return null;
    }
}
