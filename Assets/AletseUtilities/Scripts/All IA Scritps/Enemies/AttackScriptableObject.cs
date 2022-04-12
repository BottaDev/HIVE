using UnityEngine;

[CreateAssetMenu(fileName = "Attack Configuration", menuName = "ScriptableObject/Attack Configuration")]
public class AttackScriptableObject : ScriptableObject
{
    public bool IsRanged = false;
    public int Damage = 5;
    public float AttackRadius = 1.5f;
    public float AttackDelay = 1.5f;

    // Ranged Configs
    public Bullet BulletPrefab;
    public Vector3 BulletSpawnOffset = new Vector3(0, 1, 0);
    public LayerMask LineOfSightLayers;

    public AttackScriptableObject ScaleUpForLevel(ScalingScriptableObject Scaling, int Level)
    {
        AttackScriptableObject scaledUpConfiguration = CreateInstance<AttackScriptableObject>();

        scaledUpConfiguration.IsRanged = IsRanged;
        scaledUpConfiguration.Damage = Mathf.FloorToInt(Damage * Scaling.DamageCurve.Evaluate(Level));
        scaledUpConfiguration.AttackRadius = AttackRadius;
        scaledUpConfiguration.AttackDelay = AttackDelay;

        scaledUpConfiguration.BulletPrefab = BulletPrefab;
        scaledUpConfiguration.BulletSpawnOffset = BulletSpawnOffset;
        scaledUpConfiguration.LineOfSightLayers = LineOfSightLayers;

        return scaledUpConfiguration;
    }

    public void SetupEnemy(EnemyIA enemyIa)
    {
        (enemyIa.AttackRadius.Collider == null ? enemyIa.AttackRadius.GetComponent<SphereCollider>() : enemyIa.AttackRadius.Collider).radius = AttackRadius;
        enemyIa.AttackRadius.AttackDelay = AttackDelay;
        enemyIa.AttackRadius.Damage = Damage;

        if (IsRanged)
        {
            RangedAttackRadius rangedAttackRadius = enemyIa.AttackRadius.GetComponent<RangedAttackRadius>();

            rangedAttackRadius.BulletPrefab = BulletPrefab;
            rangedAttackRadius.BulletSpawnOffset = BulletSpawnOffset;
            rangedAttackRadius.Mask = LineOfSightLayers;

            rangedAttackRadius.CreateBulletPool();
        }
    }
}
