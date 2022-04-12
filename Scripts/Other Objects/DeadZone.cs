using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        KillEntity(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.attachedRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        KillEntity(collision);
    }

    private void KillEntity(Collider2D collision)
    {
        var entity = collision.GetComponent<Entity>();
        if (entity != null && !entity.IsDead)
        {
            //플레이어가 무적인 경우를 대비해 레이어 변경
            collision.gameObject.layer = 9;

            collision.attachedRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            Vector2 hitPosition = collision.ClosestPoint(transform.position);
            entity.TakeDamage(entity.MaxHealth, hitPosition, Vector2.zero);

            //if (!entity.IsDead)
            //    StartCoroutine(CheckDistance(entity));
        }
    }

    private System.Collections.IEnumerator MustKillEntity(Entity entity, Vector2 hitPosition)
    {
        while (!entity.IsDead)
        {
            entity.TakeDamage(entity.MaxHealth, hitPosition, Vector2.zero);
            yield return null;
        }
    }

    //private System.Collections.IEnumerator CheckDistance(Entity entity)
    //{
    //    while (!entity.IsDead)
    //    {
    //        yield return null;
    //        entity.TakeDamage(entity.MaxHealth, entity.transform.position, Vector2.zero);
    //    }
    //}
}