using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Skills/Ranged Auto Attack")]
public class RangedAutoAttack : EnemySkill, ISkillFirePosition
{
    [Header("Creation")]
    public Bullet projectilePrefab;
    public float creationDistanceFromCenter = 1f;

    [Header("Attack")]
    public float baseDamage = 10f;
    public float strWeight = 1f;
    public float attackRange = 7f;

    [Header("Fire")]
    public float correctionTolerance = 30f;
    public bool isChangeSpeed = false;
    public float projectileSpeed;
    public AudioClip fireSound;

    public Transform FirePosition { get; set; }

    public override void TriggerSkill(Attacker user, GameObject target)
    {
        user.StartCoroutine(Attack(user, target));
    }

    private IEnumerator Attack(Attacker user, GameObject target)
    {
        Transform firePosition = FirePosition;

        yield return new WaitForSeconds(castingTime);

        // user의 사망 여부 체크. 선택사항.
        if (user.EnemyComponent != null && user.EnemyComponent.IsDead)
            yield break;

        float damage = baseDamage + strWeight * GetSTR(user, true);

        #region [Legacy Code]
        //Vector3 targetPosition = target.transform.position + new Vector3(0, 1f);
        //Vector2 direction = ((Vector2)(targetPosition - user.transform.position)).normalized;

        //float angle = Vector2.Angle(Vector2.right, direction);
        ////Debug.Log($"{user.name}: {angle}도");
        //if (angle > 30f && angle < 150f)
        //{
        //    targetPosition = target.transform.position + new Vector3(0, 1.75f);
        //    direction = ((Vector2)(targetPosition - user.transform.position)).normalized;
        //}

        //Vector3 creationPosition = user.transform.position + creationDistanceFromCenter * (Vector3)direction;
        //if (firePosition)
        //{
        //    creationPosition = firePosition.position;
        //}
        #endregion


        // 투사체 생성 및 발사
        Vector2 direction = target.transform.position - user.transform.position;
        Vector2 creationPosition = firePosition != null ? (Vector2)firePosition.position : (Vector2)user.transform.position + (creationDistanceFromCenter * direction);

        Bullet projectile = Instantiate(projectilePrefab, creationPosition, Quaternion.identity);

        direction = projectile.FireDirectionCorrection(target.transform, correctionTolerance);
        //creationPosition = firePosition != null ? (Vector2)firePosition.position : creationPosition + (creationDistanceFromCenter * direction);

        //projectile.transform.position = creationPosition;
        projectile.transform.rotation = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.right, direction));

        if (!isChangeSpeed)
            projectile.SetBullet(damage, direction, attackRange, true, Bullet.Target.Player);
        else
            projectile.SetBullet(damage, direction, attackRange, true, projectileSpeed, Bullet.Target.Player);

        if (user.AudioSourceComponent != null && this.fireSound != null)
            user.AudioSourceComponent.PlayOneShot(this.fireSound);
    }
}