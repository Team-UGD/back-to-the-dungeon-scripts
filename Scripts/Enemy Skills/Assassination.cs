using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Skills/Assassination")]
public class Assassination : EnemySkill
{
    [SerializeField] private float minDistanceAfterTeleport = 1f; // 타겟 뒤로 텔레포트 후 최소한으로 멀어질 거리
    [SerializeField] private EnemySkill followUpAttack;
    [SerializeField] private AudioClip stealthSound;

    public override void TriggerSkill(Attacker user, GameObject target)
    {
        user.StartCoroutine(MoveBehindTarget(user, target));
    }

    private IEnumerator MoveBehindTarget(Attacker user, GameObject target)
    {
        if (castingTime > 0)
        {
            if (user.AudioSourceComponent != null)
                user.AudioSourceComponent.PlayOneShot(this.stealthSound);

            yield return new WaitForSeconds(castingTime);
        }

        float dir_x = Mathf.Sign(target.transform.position.x - user.transform.position.x);
        user.RigidbodyComponent.position = target.transform.position + new Vector3(dir_x * minDistanceAfterTeleport, 0.75f);

        yield return new WaitForSeconds(0.05f);

        if (followUpAttack != null)
            followUpAttack.TriggerSkill(user, target);
    }
}
