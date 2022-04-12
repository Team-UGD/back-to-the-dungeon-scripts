using UnityEngine;
using System.Collections;

public class BringerOfDeathSpell : MonoBehaviour
{
    [SerializeField] private float attackWaitTime = 0.5f;
    [SerializeField] private float attackTime = 1.6f;
    private float damage;
    private Animator animator;
    private bool isAttacking;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Attack(float damage)
    {
        if (!isAttacking)
        {
            this.damage = damage;
            StartCoroutine(RealAttack());
        }
    }

    private IEnumerator RealAttack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(attackWaitTime);

        animator.SetTrigger("Spell");

        yield return new WaitForSeconds(attackTime);
        isAttacking = false;
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
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
