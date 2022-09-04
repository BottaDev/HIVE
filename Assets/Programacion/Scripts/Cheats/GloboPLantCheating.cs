using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GloboPlantCheating : MonoBehaviour
{
    [SerializeField]
    HPSphere orbs;
    [SerializeField]
    int minOrbAmount, maxOrbAmount;
    [SerializeField]
    Transform spawnPoint;

    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.T)))
            GetComponent<Animator>().SetTrigger("CrystalBreak");
        else if ((Input.GetKeyDown(KeyCode.Y)))
            GetComponent<Animator>().SetTrigger("HittedBack");
        else if (Input.GetKeyDown(KeyCode.U))
        {
            anim.SetTrigger("CrystalBreak");

            for (int i = 0; i < Random.Range(minOrbAmount, maxOrbAmount + 1); i++)
            {
                Instantiate(orbs, spawnPoint.position, Quaternion.identity);
            }
        }
    }
}