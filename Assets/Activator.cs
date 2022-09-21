using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activator : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
                  {
            GetComponent<Animator>().SetTrigger("Activate");
        }
    }
}
