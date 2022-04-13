using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Enemy Skills/BossSpreadSkill")]
public class BossSpreadSkill : EnemySkill
{

    [Header("Creation")]
    public float attackRange = 20f;
    public Bullet projectilePrefab;

    [Header("Attack")]
    [SerializeField] private float baseDamage;
    [SerializeField] private float strWeight = 1f;
    [SerializeField] private bool phase2;
    float t = 0;

    public override void TriggerSkill(Attacker user, GameObject target)
    {
        base.castingTime = 0.5f;

        float damage = baseDamage + strWeight * GetSTR(user, true);

        t = Time.time;
        GameManager.Instance.StartCoroutine(Delay(Attack, user, damage));
    }

    private void Attack(Attacker user, float damage, float f)
    {
        //Vector2[] direction = {
        //    new Vector2(1,1),
        //    new Vector2(-1,1),
        //    new Vector2(1,-1),
        //    new Vector2(-1,-1)
        //};

        Vector2[] direction = {
            new Vector2(0.1f,0.1f),
            new Vector2(-0.1f,0.1f),
            new Vector2(0.1f,-0.1f),
            new Vector2(-0.1f,-0.1f)
        };

        Vector3[] creationPosition = new Vector3[4];

        for (int i = 0; i < 4; i++)
        {
            creationPosition[i] = user.transform.position + (Vector3)direction[i];
        }

        for (int i = 0; i < 4; i++)
        {
            Bullet projectile = Instantiate(projectilePrefab, creationPosition[i], Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.right, direction[i])+f));

            projectile.SetBullet(damage, projectile.transform.right, attackRange, true, Bullet.Target.Player);
        }

    }

    IEnumerator Delay(Action<Attacker, float,float> fucn, Attacker user, float damage)
    {
        yield return new WaitForSeconds(0.8f);
        int i = 0;

        for (i = 0;i<100;i++)
        {
            if (user.EnemyComponent.IsDead == true)
                yield break;

            yield return new WaitForSeconds(0.23f);
            fucn(user, damage,i*10);

            if ((Time.time - t) > baseCoolDown)
                yield break;
        }

        //if (phase2)
        //{
        //    for (int j = 0; j < 15; j++)
        //    {
        //        yield return new WaitForSeconds(0.23f);
        //        fucn(user, damage, i * 10);
        //    }

        //    for (i = 100; i > 0; i--)
        //    {
        //        if (user.EnemyComponent.IsDead == true)
        //            yield break;

        //        yield return new WaitForSeconds(0.23f);
        //        fucn(user, damage, i * 10);
        //    }
        //}

    }

}
