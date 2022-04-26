using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Enemy Skills/SpreadSkill")]
public class SpreadSkill : EnemySkill
{

    [Header("Creation")]
    public float attackRange = 7f;
    public float projectileSpeed = 7f;
    public float creationDistanceFromCenter = 1f;
    public Bullet projectilePrefab;

    [Header("Attack")]
    [SerializeField] private float baseDamage;
    [SerializeField] private float strWeight = 1f;

    public override void TriggerSkill(Attacker user, GameObject target)
    {
        base.castingTime = 0.5f;

        //비행하는 몬스터가 아니라면 점프 후 공격
        JumpEnemy(user);

        float damage = baseDamage + strWeight * GetSTR(user,true);

        user.StartCoroutine(Delay( Attack, user, damage));
    }
    protected void JumpEnemy(Attacker user)
    {
        EnemyPathfinder enemyPathfinder = user.GetComponent<EnemyPathfinder>();

        if (!enemyPathfinder.canFly)
        {
            user.GetComponent<Rigidbody2D>().velocity = Vector2.up * 16;
        }
    }

    private void Attack(Attacker user, float damage)
    {
        Vector2[] direction = {
            Vector2.up ,
            Vector2.down,
            Vector2.left,
            Vector2.right,
            new Vector2(1,1),
            new Vector2(-1,1),
            new Vector2(1,-1),
            new Vector2(-1,-1)
        };

        Vector3[] creationPosition = new Vector3[8];
        for (int i = 0; i < 8; i++)
            creationPosition[i] = user.transform.position + creationDistanceFromCenter * (Vector3)direction[i];

        for (int i = 0; i < 8; i++)
        {
            Bullet projectile = Instantiate(projectilePrefab, creationPosition[i], Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.right, direction[i])));
            
            projectile.SetBullet(damage, direction[i], attackRange, true,projectileSpeed,Bullet.Target.Player);
        }
        
    }

    IEnumerator Delay( Action<Attacker, float> fucn, Attacker user, float damage)
    {
        yield return new WaitForSeconds(castingTime);

        if (user.EnemyComponent.IsDead == true)
            yield break;

        fucn(user, damage);
    }

}
