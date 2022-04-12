using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Skills/BossLaserSkill")]
public class BossLaser : EnemySkill
{
    [Header("setting")]
    //일단은 BossHand
    [SerializeField] private BossHand followingUserObject;
    [SerializeField] private float followingTime;

    // x = -11
    private Dictionary<Attacker, BossHand> followingUserObjectBuffer = new Dictionary<Attacker, BossHand>();

    public override void TriggerSkill(Attacker user, GameObject target)
    {
        if (followingUserObjectBuffer.TryGetValue(user, out var bossHand))
        {
            // 이전에 user가 생성했던 hand가 아직 scene내에 존재한다면 즉시 종료함
            if (bossHand != null)
            {
                Debug.Log("아직 Boss Hand가 존재.");
                return;
            }
            followingUserObjectBuffer.Remove(user);
        }

        var hand = Instantiate(followingUserObject, new Vector2(target.transform.position.x - 9,target.transform.position.y+1), Quaternion.identity);
        followingUserObjectBuffer.Add(user, followingUserObject);

        GameManager.Instance.StartCoroutine(SetFollowingUserObjectPosition(user,target, hand));
    }

    private IEnumerator SetFollowingUserObjectPosition(Attacker user, GameObject target, BossHand hand)
    {
        float time = 0;
        while (true)
        {
            if (followingUserObject == null)
                yield break;

            hand.transform.position = new Vector2(target.transform.position.x - 9, target.transform.position.y+1);
           
            time += 0.02f;
            if (time >= followingTime)
            {
                break;
            }

            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(1);
        //레이저 발사 처리

        Destroy(hand.gameObject);

    }
}
