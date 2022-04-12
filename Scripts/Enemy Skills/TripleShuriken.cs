using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Enemy Skills/Triple Shuriken")]
public class TripleShuriken : EnemySkill, ISkillFirePosition
{
    [Header("Creation")]
    public Bullet shurikenPrefab;
    public float creationDistanceInterval;

    [Header("Attack")]
    public float baseDamage;
    public float strWeight = 1f;
    public float attackRange = 10f;
    public AudioClip fireSound;

    public Transform FirePosition { get; set; }

    private bool isContinous = false;

    public override void TriggerSkill(Attacker user, GameObject target)
    {
        if (FirePosition == null)
            FirePosition = user.transform;

        user.StartCoroutine(Attack(user, target));
    }

    private IEnumerator Attack(Attacker user, GameObject target)
    {
        var entity = user.EnemyComponent;

        if (entity.IsDead)
            yield break;

        Transform firePosition = FirePosition;

        yield return new WaitForSeconds(castingTime);

        if (entity.IsDead)
            yield break;

        float damage = baseDamage + strWeight * GetSTR(user);

        AudioSource userAudioPlayer = user.AudioSourceComponent;
        if (userAudioPlayer != null)
        {
            userAudioPlayer.PlayOneShot(fireSound);
        }

        // 중심 표창
        Vector2 creationPosition = firePosition.transform.position;
        Bullet shuriken = Instantiate(shurikenPrefab, creationPosition, Quaternion.identity);

        Vector2 direction = shuriken.FireDirectionCorrection(target.transform, 30f);
        Vector2 interpolVec = creationDistanceInterval * (Quaternion.Euler(0f, 0f, 90f) * direction).normalized;

        var rotation = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.right, direction));
        shuriken.transform.rotation = rotation;
        shuriken.SetBullet(damage, direction, attackRange, true, Bullet.Target.Player, isContinous);

        // 사이드 표창1
        creationPosition -= interpolVec;
        shuriken = Instantiate(shurikenPrefab, creationPosition, rotation);
        shuriken.SetBullet(damage, direction, attackRange, true, Bullet.Target.Player, isContinous);

        // 사이드 표창2
        creationPosition += 2 * interpolVec;
        shuriken = Instantiate(shurikenPrefab, creationPosition, rotation);
        shuriken.SetBullet(damage, direction, attackRange, true, Bullet.Target.Player, isContinous);
    }
}
