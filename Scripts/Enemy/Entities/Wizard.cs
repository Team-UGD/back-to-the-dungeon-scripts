using UnityEngine;

public class Wizard : Enemy
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GetComponent<EnemyAttacker>().OnSkillTriggered += s =>
        {
            if (s is RangedAutoAttack && animator)
                animator.SetTrigger("FireBall");
        };
    }

    protected override void Die()
    {
        base.Die();

        animator.SetTrigger("Die");
    }
}
