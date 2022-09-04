using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPSphere : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField]
    float minStartForce, maxStartForce, speed, acceleration;

    Transform player;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = FindObjectOfType<Player>().transform;
        var startDir = new Vector3(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5, 5)).normalized;
        rb.AddForce(startDir * Random.Range(minStartForce, maxStartForce), ForceMode.Impulse);
    }

    private void Update()
    {
        transform.LookAt(Camera.main.transform);

        speed += acceleration * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.P))
        {
            var startDir = new Vector3(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5, 5)).normalized;
            //var startDir = new Vector3(1, 2, 3).normalized;
            rb.AddForce(startDir * Random.Range(minStartForce, maxStartForce), ForceMode.Impulse);
            //rb.AddForce(Vector3.forward* maxStartForce, ForceMode.Impulse);
        }

        var dir = (player.position - transform.position).normalized;
        rb.AddForce(dir * speed);
    }
}