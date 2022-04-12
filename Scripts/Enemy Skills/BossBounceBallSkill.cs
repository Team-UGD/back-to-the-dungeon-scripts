using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;

[CreateAssetMenu(menuName ="Enemy Skills/BossBounceBallSkill")]
public class BossBounceBallSkill : EnemySkill
{
    [Header("Setting")]
    [SerializeField] private int attackRange = 8;
    [SerializeField] private int ballNum = 6;
    [SerializeField] private float power = 5f;
    [SerializeField] private float grabWaitTime = 2f;

    [Header("Skill and Prefab")]
    [SerializeField] private BounceBall bounceBall;
    [SerializeField] private GrabSkill grabSkill;

    private Vector2 shootPos;
    Random rand = new Random();
    public override void TriggerSkill(Attacker user, GameObject target)
    {
        Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
        float damage = GetSTR(user);
        for (int i = 0; i < ballNum; i++)
        {
            shootPos= new Vector2(rand.Next((int)user.transform.position.x - attackRange, (int)user.transform.position.x + attackRange), user.transform.position.y + 5);

            BounceBall ball = Instantiate(bounceBall, shootPos, rotation);
            ball.StartCoroutine(Attack(target, ball, shootPos));                                              
        }
        user.StartCoroutine(Wait(user, target));
    }
    private IEnumerator Wait(Attacker user, GameObject target)
    {
        yield return new WaitForSeconds(grabWaitTime);
        grabSkill.TriggerSkill(user, target); 
    }
    private IEnumerator Attack(GameObject target,BounceBall ball, Vector2 pos)
    {
        ball.effect.enabled = true;
        Vector2 toPos = target.transform.position;
        ball.transform.GetComponent<Rigidbody2D>().AddForce((toPos - pos).normalized * power,ForceMode2D.Impulse);
        yield return null;
    }
    private float GetSTR(GameObject user)
    {
        float temp = user?.GetComponent<IStrikingPower>().STR ?? 0f;
        float str = temp < 0f ? 0f : temp;

        return str;
    }
}
