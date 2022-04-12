using System.Collections;
using UnityEngine;

public class BringerOfDeathSword : MeleeWeapon
{
    [SerializeField] private float attackTime = 1f;

    private bool isSwing;
    private Animator animator;
    private float damage;

    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
    }

    public override void Attack(float damage)
    {
        if (!isSwing)
        {
            this.damage = damage;
            StartCoroutine(Swing());
        }
    }

    private IEnumerator Swing()
    {
        animator.SetTrigger("Close Attack");
        isSwing = true;

        yield return new WaitForSeconds(attackTime);

        isSwing = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var entity = collision.GetComponent<Entity>();

            Vector2 hitPosition = collision.ClosestPoint(transform.position);
            Vector2 hitNormal = new Vector2(hitPosition.x - collision.transform.position.x, collision.transform.position.y).normalized;

            entity.TakeDamage(damage, hitPosition, hitNormal);
        }
    }
}
