using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astronaut : Enemy
{
    Animator animator;
    Animator skeletalAnimator;
    GameObject skeletal;
    EnemyDetection enemyDetection;

    void Awake()
    {
        skeletal = transform.Find("Skeletal").gameObject;
        animator = GetComponent<Animator>();
        enemyDetection = GetComponent<EnemyDetection>();
        skeletalAnimator = skeletal.GetComponent<Animator>();

        animator.SetBool("IsFollow", false);
        animator.SetBool("IsReady", true);
        skeletalAnimator.SetBool("bomb", false);

        OnDeath += () =>
        {
            animator.SetTrigger("isDie");
            skeletalAnimator.SetTrigger("isDie");
            skeletal.SetActive(false);
            if (enemyDetection.Target)
                Instantiate(Resources.Load<GameObject>("Projectile/Bomb"), transform.position, Quaternion.identity);
        };
    }


    protected override void Die()
    {
        base.Die();
    }
    private void Update()
    {
        if (enemyDetection.Target && !IsDead)
        {
            skeletal.SetActive(true);
            animator.SetBool("IsFollow", true);
            animator.SetBool("IsReady", false);
            skeletalAnimator.SetBool("bomb", true);
        }
        else
        {
            skeletal.SetActive(false);
            animator.SetBool("IsFollow", false);
            animator.SetBool("IsReady", true);
            skeletalAnimator.SetBool("bomb", false);
        }
    }
}
