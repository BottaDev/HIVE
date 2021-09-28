using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myAnimScripts : MonoBehaviour
{
    Animator anim;
   

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.A))
        {
            anim.SetBool("isJumping",true);
            Debug.Log("pasa a jump");
        }
        else
        {
            anim.SetBool("isJumping", false);
        }


        if(Input.GetKeyDown(KeyCode.D))
        {
            anim.SetBool("isDashing", true);
            Debug.Log("pasa a dash");
        }
        else
        {
            anim.SetBool("isDashing", false);
        }



        if(Input.GetKey(KeyCode.Space))
        {
            anim.SetBool("isShooting", true);
            Debug.Log("deberia disparar");
        }
        else
        {
            anim.SetBool("isShooting", false);
        }

        if (Input.GetKey(KeyCode.W))
        {
            anim.SetBool("isRunning", true);
        }
        else
            anim.SetBool("isRunning", false);
    }
}