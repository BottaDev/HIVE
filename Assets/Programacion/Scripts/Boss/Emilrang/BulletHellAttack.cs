using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHellAttack : MonoBehaviour
{
    public int numberOfProjectiles;
    public float projectileSpeed;

    public GameObject projectileBulletHell;

    public GameObject attackPoint;

    private Vector3 startPoint;
    private const float radius = 1f;

    // Start is called before the first frame update

    public void SpawnProjectile(int _numberOfProjectiles)
    {
        startPoint = attackPoint.transform.position;

        float angleStep = 360f / _numberOfProjectiles;
        float angle = 0f;

        for (int i = 0; i < _numberOfProjectiles - 1; i++)
        {
            //Direction Calculation
            float projectileDirXPosition = startPoint.x + Mathf.Sin((angle * Mathf.PI) / 180) * radius;
            float projectileDirYPosition = startPoint.y + Mathf.Cos((angle * Mathf.PI) / 180) * radius;

            Vector3 projectileVector = new Vector3(projectileDirXPosition, projectileDirYPosition, 0);
            Vector3 projectileMoveDirection = (projectileVector - startPoint).normalized * projectileSpeed;

            GameObject tempObj = Instantiate(projectileBulletHell, startPoint, Quaternion.identity);
            tempObj.GetComponent<Rigidbody>().velocity = new Vector3(projectileMoveDirection.x, 0, projectileMoveDirection.y);

            angle += angleStep;
        }
    }

    public void SpawnProjectile_KamVersion()
    {
        startPoint = attackPoint.transform.position;

        float angleStep = 360f / numberOfProjectiles;
        float angle = 0f;

        for (int i = 0; i < numberOfProjectiles - 1; i++)
        {
            //Direction Calculation
            GameObject obj = new GameObject();
            obj.transform.position = startPoint;
            obj.transform.Rotate(new Vector3(0, angle, 0));
            obj.transform.position += obj.transform.forward;

            GameObject tempObj = Instantiate(projectileBulletHell, startPoint, Quaternion.identity);
            tempObj.GetComponent<Bullet>().transform.LookAt(obj.transform);

            angle += angleStep;
            Destroy(obj);
        }
    }

}
