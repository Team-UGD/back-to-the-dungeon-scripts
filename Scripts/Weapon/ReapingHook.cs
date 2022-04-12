using System.Collections;
using UnityEngine;

public class ReapingHook : MeleeWeapon, IAttackTime, IFade
{
    [Header("Basic Info")]
    [SerializeField, Min(0f)] private float barDamageRate = 0.7f;
    [SerializeField, Min(0f)] private float edgeDamageRate = 1.5f;
    [SerializeField, Min(0f)] private float FadeInTime;
    [SerializeField, Min(0f)] private float FadeOutTime;
    [SerializeField, Min(0f)] private float damage;
    [SerializeField] private AttackMode startAttackMode = AttackMode.On;

    [Header("Components")]
    [SerializeField] private Transform barStart;
    [SerializeField] private Transform barEnd;
    [SerializeField] private TrailRenderer effect;

    private bool isSwing;

    private Collider2D weaponCollider;
    private Animator animator;

    public float AttackTime { get; set; }

    private enum AttackMode
    {
        On,
        Off
    }

    private void Awake()
    {
        weaponCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        switch (startAttackMode)
        {
            case AttackMode.On:
                AttackEnable();
                break;
            case AttackMode.Off:
                AttackDisable();
                break;
        }
    }

    public override void Attack(float damage)
    {
        if (!isSwing)
        {
            this.damage = damage;
            StartCoroutine(AttackableCoroutine());
        }
    }

    public void AttackEnable()
    {
        effect.enabled = true;
        weaponCollider.enabled = true;
    }

    public void AttackDisable()
    {
        weaponCollider.enabled = false;
        effect.enabled = false;
    }

    private IEnumerator AttackableCoroutine()
    {
        isSwing = true;
        effect.enabled = true;
        weaponCollider.enabled = true;

        yield return new WaitForSeconds(AttackTime);

        weaponCollider.enabled = false;

        yield return new WaitForSeconds(effect.time);
        effect.enabled = false;

        isSwing = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var entity = collision.GetComponent<Entity>();

            Vector2 hitPosition = collision.ClosestPoint(transform.position);
            Vector2 hitNormal = new Vector2(hitPosition.x - collision.transform.position.x, collision.transform.position.y).normalized;

            entity.TakeDamage(CalcDamage(hitPosition - (Vector2)transform.position), hitPosition, hitNormal);
        }
    }

    // 타격한 낫 부위에 따른 데미지 연산
    private float CalcDamage(Vector2 hitLocalPosition)
    {
        float barLength = Vector2.Distance(barStart.position, barEnd.position);
        float startToHit = Vector2.Distance(barStart.position, hitLocalPosition);

        float damage = (startToHit > barLength ? edgeDamageRate : barDamageRate) * this.damage;

        return damage;
    }

    public float FadeIn()
    {
        animator.SetTrigger("Fade In");
        return FadeInTime;
    }

    public float FadeOut()
    {
        animator.SetTrigger("Fade Out");
        return FadeOutTime;
    }
}
