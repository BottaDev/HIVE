using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GloboPLantCheating : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.T)))
            GetComponent<Animator>().SetTrigger("CrystalBreak");
        else if ((Input.GetKeyDown(KeyCode.Y)))
            GetComponent<Animator>().SetTrigger("HittedBack");
    }
}
