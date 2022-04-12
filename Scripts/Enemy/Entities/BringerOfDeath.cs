using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BringerOfDeath : Enemy
{
    private Animator animator;
    private SpriteRenderer sprite;
    private EnemyAttacker attacker;
    private Rigidbody2D rigid;

    [Header("boss hpbar")]
    [SerializeField] private bool useBossHpBar;
    [SerializeField] private GameObject baseHealthBar;


    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        attacker = GetComponent<EnemyAttacker>();
        rigid = GetComponent<Rigidbody2D>();

    }

    private void Start()
    {
        attacker.OnSkillTriggered += OnBringerOfDeathSkillTriggered;

        if(useBossHpBar)
        {
            baseHealthBar.SetActive(false);
            UIManager.Instance.EnabledBossHealthUI(true);
            //UIManager.Instance.EnabledRemainEnemyUI(false);
        }

        if (useBossHpBar)
        {
            UIManager.Instance.EnabledBossHealthUI(true);
            UIManager.Instance.EnabledRemainEnemyUI(false);
        }
    }

    private void Update()
    {
        animator.SetBool("is Walk", rigid.velocity.magnitude >= 0.1f);
    }

    private void OnBringerOfDeathSkillTriggered(EnemySkill skillTriggered)
    {
        if (skillTriggered is Assassination)
        {
            StartCoroutine(Fade(skillTriggered.castingTime));
        }
        else if (skillTriggered is SpellSkill)
        {
            animator.SetTrigger("Spell Cast");
        }
    }

    public override float Health
    {
        get => base.Health;

        protected set
        {
            base.Health = value;

            if (useBossHpBar)
            {
                UIManager.Instance.SetBossHealthUI(Health, MaxHealth);
                return;
            }

            if (healthBar != null)
                healthBar.value = Health / MaxHealth;
            else
                throw new System.NullReferenceException($"Enemy class - healthBar field is null. Please check inspector");
        }
    }

    protected override void Die()
    {
        base.Die();

        health = 0;

        UIManager.Instance.EnabledBossHealthUI(false);
        UIManager.Instance.EnabledRemainEnemyUI(true);
    }


    private IEnumerator Fade(float castingTime)
    {
        animator.SetTrigger("Fade Out");
        attacker.RigidbodyComponent.constraints = RigidbodyConstraints2D.FreezeAll;
        gameObject.layer = 12;

        yield return new WaitForSeconds(1f);
        sprite.enabled = false;

        yield return new WaitForSeconds(castingTime - 1f);

        sprite.enabled = true;
        attacker.RigidbodyComponent.constraints = RigidbodyConstraints2D.FreezeRotation;
        animator.SetFloat("Fade In Speed", 3f);
        animator.SetTrigger("Fade In");

        yield return new WaitForSeconds(0.3f);

        animator.SetFloat("Fade In Speed", 1f);
        gameObject.layer = 7;

    }
}
