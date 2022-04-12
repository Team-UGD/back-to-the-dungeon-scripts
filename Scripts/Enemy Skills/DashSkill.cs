using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Enemy Skills/DashSkill")]
public class DashSkill : EnemySkill
{

    [Header("Attack")]
    [SerializeField] public float baseDamage;
    [SerializeField] private float strWeight = 1f;
    [SerializeField] float dashSpeed;
    private float dashDamage;

    public float DashDamage { get { return dashDamage; } private set { dashDamage = value; } }

    public override void TriggerSkill(Attacker user, GameObject target)
    {

        DashDamage = baseDamage + strWeight * GetSTR(user,true);

        //Rigidbody2D rigidbody = user?.GetComponent<Rigidbody2D>();
        float dir = -(user.transform.position.x - target.transform.position.x);
        Vector2 vec = new Vector2(dir,0).normalized;

        if (user.RigidbodyComponent)
        {
            //대쉬시 반대방향을 바라보고 대쉬를 하는 경우를 방지하기 위한 코드
            user.transform.localScale = new Vector3(vec.x, 1, 1);

            user.RigidbodyComponent.velocity = new Vector2(0, 0);
            user.StartCoroutine(Delay(Dash, vec, user.RigidbodyComponent, user));
        }
        
    }

    private void Dash(Vector2 vec, Rigidbody2D rigidbody)
    {
        if (rigidbody != null)
            rigidbody.AddForce(vec * dashSpeed, ForceMode2D.Impulse);
    }

    IEnumerator Delay(Action<Vector2, Rigidbody2D> Func, Vector2 vec, Rigidbody2D rigidbody,Attacker user)
    {
        yield return new WaitForSeconds(castingTime-2);     //준비 시간
        Func(vec, rigidbody);

        if (user.EnemyComponent.IsDead == true)
            yield break;

        yield return new WaitForSeconds(castingTime-1);     //실행 시간
        if(rigidbody != null)
            rigidbody.velocity = new Vector2(0, 5);
    }


}