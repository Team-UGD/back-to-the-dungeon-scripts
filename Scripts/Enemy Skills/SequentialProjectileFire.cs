#define TESTN
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Skills/Sequential Projectile Fire")]
public class SequentialProjectileFire : EnemySkill, ISkillFirePosition
{
    [Header("Settings")]
    public Bullet projectilePrefab;
    public uint projectileCount = 1;
    public bool isRotated = false;
    public bool scaleControl = true;

    [Header("Creation")]
    [Range(0f, 180f)] public float layoutAngle;
    public float creationWaitingTime;
    public float creationTimeInterval;
    public float creationDistanceInterval;
    public CreationDirection creationDirection = CreationDirection.Automatic;
    public AudioClip creationSound;

    [Header("Attack")]
    public float baseDamage;
    public float strWeight = 1f;
    public float attackTimeInterval;
    public float attackRange = 10f;
    public bool isContinuousAttack;
    public AudioClip fireSound;

    public Transform FirePosition { get; set; }

    public enum CreationDirection
    {
        Automatic = 0,
        Left2Right = 1,
        Right2Left = -1
    }

    public override void TriggerSkill(Attacker user, GameObject target)
    {
        if (FirePosition == null)
            FirePosition = user.transform;

        CoroutineManager.Instance.StartCoroutine(Attack(user, target));
    }

    private IEnumerator Attack(Attacker user, GameObject target)
    {
        Transform firePosition = FirePosition;

        if (creationWaitingTime > 0)
            yield return new WaitForSeconds(creationWaitingTime);

        var entity = user.EnemyComponent;

        if (entity != null && entity.IsDead)
            yield break;

        Vector3 correctionForTargetPostion = new Vector2(0f, 0.75f);
        Vector2 targetPosition = target.transform.position + correctionForTargetPostion;

        //투사체 생성
        Bullet[] projectiles = new Bullet[projectileCount];
        AudioSource userAudioPlayer = user.AudioSourceComponent;

        Vector2[] creationPostion = GetCreationPositions(user.transform.position, targetPosition, firePosition.position);
        int arrayLength = creationPostion.Length;
        if (arrayLength <= 0)
            yield break;

        for (int i = 0; i < projectiles.Length; i++)
        {
#if TEST
            Debug.LogWarning($"[{typeof(SequentialProjectileFire)}] {i + 1}번째 투사체 생성");
#endif
            if (entity != null && entity.IsDead)
                break;

            targetPosition = target.transform.position + correctionForTargetPostion;
            //Vector2 direction = (targetPosition - creationPostion[i % arrayLength]).normalized; // 생성 방향
            projectiles[i] = Instantiate(projectilePrefab, creationPostion[i % arrayLength], Quaternion.identity);

            SetProjectileRotatiton(projectiles[i], target.transform);
            // 총 시간: castingTime
            // 생성 시각: 0, creationTimeInterval, creationTimeInterval * 2, creationImteInterval * 3
            // 발사 시각: creationTimeInterval * 4 + watingTime, creationTimeInterval * 4 + watingTime + attackTimeInerval
            // 총 회전 시간: creationTimeInverval * 4 + waitingtime, creationTimeInerval * 4 + waitingTime + attackTimeInerval - creationTimeInterval
            // watingTime = castingTime - creationWatingTime - creationTimeInterval * projectileCount - attackTimeInterval * projectileCount
            // castringTime - creationTimeInerval * i - creationWaitingTime - attackTimeInterval * (projectileCount - i)
            if (isRotated)
            {
                projectiles[i].StartCoroutine(RoateProjectiles(projectiles[i], target.transform,
                    castingTime - creationWaitingTime - creationTimeInterval * i - attackTimeInterval * (projectileCount - i)));
            }

            projectiles[i].TargetInfo = Bullet.Target.Player;

            if (userAudioPlayer != null && this.creationSound != null)
                userAudioPlayer.PlayOneShot(this.creationSound);


            if (creationTimeInterval > 0)
                yield return new WaitForSeconds(creationTimeInterval);
        }

        // 생성 시간과 공격 시간을 제외한 나머지 시간동안 대기 시간을 가짐.
        float waitingTime = castingTime - creationWaitingTime - creationTimeInterval * projectileCount - attackTimeInterval * projectileCount;
        if (waitingTime > 0)
        {
            yield return new WaitForSeconds(waitingTime);
        }

        // 각 투사체를 비동기적으로 회전시켜야함. 이건 나중에 구현. 일단 발사처리부터

        // 발사
        float damage = baseDamage + strWeight * GetSTR(user, true);
        for (int i = 0; i < projectiles.Length; i++)
        {
            if (projectiles[i] == null)
                continue;
#if TEST
            Debug.LogWarning($"[{typeof(SequentialProjectileFire)}] {i + 1}번째 투사체 공격");
#endif
            Vector2 direction = SetProjectileRotatiton(projectiles[i], target.transform);
            projectiles[i].SetBullet(damage, direction, attackRange, true, isContinuousAttack);
            //Debug.Log($"<{nameof(SequentialProjectileFire)}> 발사", this);

            if (userAudioPlayer != null && this.fireSound != null)
                userAudioPlayer.PlayOneShot(fireSound);

            if (attackTimeInterval > 0)
                yield return new WaitForSeconds(attackTimeInterval);
        }
    }

    protected virtual Vector2[] GetCreationPositions(Vector2 user, Vector2 target, Vector2 firePosition)
    {
        // 투사체 생성을 위한 위치 전처리
        uint projectileCount = this.projectileCount;
        Vector2 middlePosition = firePosition;
        Vector2 intervalControl = Mathf.Sign((int)creationDirection) * Mathf.Sign(90f - layoutAngle)
            * creationDistanceInterval * (Quaternion.Euler(0f, 0f, layoutAngle) * Vector2.right).normalized;
        Vector2 current = middlePosition - (projectileCount - 1) / 2f * intervalControl;

        if (creationDirection == CreationDirection.Automatic)
        {
            Vector2 lhs = current;
            Vector2 rhs = middlePosition + (projectileCount - 1) / 2f * intervalControl;
            if ((rhs - target).magnitude < (lhs - target).magnitude)
            {
                current = rhs;
                intervalControl *= -1f;
            }
        }

        Vector2[] creationPositions = new Vector2[projectileCount];
        for (int i = 0; i < projectileCount; i++)
        {
            creationPositions[i] = current;
            current += intervalControl;
        }

        return creationPositions;
    }

    private Vector2 SetProjectileRotatiton(Bullet projectile, Transform target)
    {
        Vector2 direction = projectile.FireDirectionCorrection(target.transform, 30f);

        if (this.scaleControl)
        {
            Vector3 scale = projectile.transform.localScale;
            scale.x *= Mathf.Sign(scale.x) * Mathf.Sign(direction.x);
            projectile.transform.localScale = scale;
        }

        projectile.transform.rotation = Quaternion.Euler(0f, 0f, 
            Vector2.SignedAngle((this.scaleControl && Mathf.Sign(direction.x) < 0) ? Vector2.left : Vector2.right, direction));

        return direction;
    }

    private IEnumerator RoateProjectiles(Bullet projectile, Transform target, float rotationTime)
    {
        float time = 0;
        while (time <= rotationTime)
        {
            SetProjectileRotatiton(projectile, target);

            yield return null;
            time += Time.deltaTime;
        }
    }
}
