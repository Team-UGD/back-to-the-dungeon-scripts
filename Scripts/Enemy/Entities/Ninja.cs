using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ninja : Enemy
{
    private Animator animator;
    private EnemyPathfinder pathfinder;
    private Rigidbody2D rigid;
    private SpriteRenderer sprite;

    private Color spriteColor;
    private float timeAfterDie;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        pathfinder = GetComponent<EnemyPathfinder>();
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        GetComponent<EnemyAttacker>().OnSkillTriggered += OnSkillTriggered;
        spriteColor = sprite.color;
    }

    private void Update()
    {
        if (IsDead)
        {
            timeAfterDie += Time.deltaTime;
            sprite.color = Color.Lerp(spriteColor, Color.clear, Mathf.InverseLerp(0f, this.timeToDestroy, timeAfterDie));

            return;
        }

        if (rigid.velocity.magnitude >= 0.1f)
            animator.SetBool("isRun", true);
        else
            animator.SetBool("isRun", false);

        if (pathfinder.IsJumping)
            animator.SetBool("isJump", true);
        else
            animator.SetBool("isJump", false);
    }

    protected override void Die()
    {
        base.Die();

        if (animator.GetBool("isRun") && !animator.GetBool("isJump"))
        {
            animator.SetTrigger("Die");
        }
    }

    private void OnSkillTriggered(EnemySkill skillTriggered)
    {
        if (skillTriggered is RangedAutoAttack || skillTriggered is TripleShuriken)
        {
            if (animator != null)
                animator.SetTrigger("Attack1");
        }
        else if (skillTriggered is NinjaSequentialShuriken)
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack2");
                StartCoroutine(StopMove(skillTriggered.castingTime - 1f));
            }
        }
    }

    private IEnumerator StopMove(float time)
    {
        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(time);
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
