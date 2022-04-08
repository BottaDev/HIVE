using UnityEngine;

[CreateAssetMenu(fileName = "Attack Configuration", menuName = "ScriptableObject/Attack Configuration")]
public class AttackScriptableObject : ScriptableObject
{
    public bool isRange = false;
    public int damage = 5;
    public float attackRadius = 1.5f;
    public float attackDelay = 1.5f;

    [Header("Ranged Configuration")] 
    public Bullet bulletPrefab;
    public Vector3 bulletSpawnOffset = new Vector3(0, 1, 0);
    public LayerMask LineOfSightLayers;

    public void SetupEnemy(Enemy enemy)
    {
        (enemy.attackRadius.collider == null
            ? enemy.attackRadius.GetComponent<SphereCollider>()
            : enemy.attackRadius.collider).radius = attackRadius;

        enemy.attackRadius.attackDelay = attackDelay;
        enemy.attackRadius.damage = damage;

        if (isRange)
        {
            RangedAttackRadius rangedAttackRadius = enemy.attackRadius.GetComponent<RangedAttackRadius>();

            rangedAttackRadius.bulletPrefab = bulletPrefab;
            rangedAttackRadius.bulletSpawnOffset = bulletSpawnOffset;
            rangedAttackRadius.mask = LineOfSightLayers;
            
            rangedAttackRadius.CreateBulletPool();
        }
    }
}
