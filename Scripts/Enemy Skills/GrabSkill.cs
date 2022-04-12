using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


[CreateAssetMenu(menuName = "Enemy Skills/Boss Grab")]
public class GrabSkill : EnemySkill
{
    [Header("Attack")]
    [SerializeField] private float baseDamage;
    [SerializeField] private float attackTimeInterval = 0.5f;
    [SerializeField] private uint totalKeyPressCount;
    [SerializeField] private float attackRange = 20f;
    [SerializeField] private float handMoveSpeed = 5f;
    [SerializeField] private Vector3 leftAttackLocalPosition;
    [SerializeField] private Vector3 rightAttackLocalPosition;

    [Header("Creation")]
    [SerializeField] private BossHand bossHandPrefab;
    [SerializeField] private float creationDistanceFromTarget = 5f;
    [SerializeField] private float creationDuration;

    [Header("Skill")]
    [SerializeField] private SingleSwordSwing swordSwing;

    [Header("Other")]
    [SerializeField] private float singleSwingWaitTime = 2.5f;

    private Dictionary<Attacker, BossHand> createdBossHandBuffer = new Dictionary<Attacker, BossHand>();

    public override void TriggerSkill(Attacker user, GameObject target)
    {
        if (createdBossHandBuffer.TryGetValue(user, out var bossHand))
        {
            // 이전에 user가 생성했던 hand가 아직 scene내에 존재한다면 즉시 종료함
            if (bossHand != null)
            {
                Debug.Log($"[{typeof(GrabSkill)}] 아직 Boss Hand가 존재합니다.");
                return;
            }

            createdBossHandBuffer.Remove(user);
        }

        float targetPos_x = target.transform.position.x;
        // target이 user보다 왼쪽에 있는 경우
        if (targetPos_x <= user.transform.position.x)
        {
            Vector3 creationPosition = target.transform.position + new Vector3(-creationDistanceFromTarget, 0.75f);
            var leftHand = Instantiate(bossHandPrefab, creationPosition, Quaternion.identity);
            leftHand.transform.localScale = new Vector3(-1f, 1f, 1f);
            createdBossHandBuffer.Add(user, leftHand);

            InitializeBossHand(leftHand);
            leftHand.StartCoroutine(MoveHand(user, leftHand, target));
            Debug.Log($"[{typeof(GrabSkill)}] 왼손 그랩스킬 활성화");
        }
        // target이 user보다 오른쪽에 있는 경우
        else
        {
            Vector3 creationPosition = target.transform.position + new Vector3(creationDistanceFromTarget, 0.75f);
            var rightHand = Instantiate(bossHandPrefab, creationPosition, Quaternion.identity);
            createdBossHandBuffer.Add(user, rightHand);

            InitializeBossHand(rightHand);
            rightHand.StartCoroutine(MoveHand(user, rightHand, target));
            Debug.Log($"[{typeof(GrabSkill)}] 오른손 그랩스킬 활성화");
        }

    }

    private void InitializeBossHand(BossHand hand)
    {
        hand.gameObject.SetActive(true);
        hand.SetGrabSkillInfo(baseDamage, attackTimeInterval, totalKeyPressCount);
        hand.FadeIn(creationDuration);
    }

    private IEnumerator MoveHand(Attacker user, BossHand hand, GameObject target)
    {
        // 공격을 위한 보스 손 이동
        float start_x = hand.transform.position.x;
        float dir_x = Mathf.Sign(target.transform.position.x - start_x);

        float t = 0;
        while (t < creationDuration)
        {
            yield return null;
            t += Time.deltaTime;
        }

        while (Mathf.Abs(start_x - hand.transform.position.x) <= attackRange && !hand.IsHolding)
        {
            Vector3 direction = new Vector3(dir_x, 0f);
            hand.transform.position += handMoveSpeed * direction * Time.deltaTime;
            yield return null;
        }

        // 잡고 있으면 파괴 x
        // 잡고 있지 않으면서 범위를 벗어났다면 파괴 o
        if (!hand.IsHolding)
        {
            swordSwing.TriggerSkill(user, target);
            Destroy(hand.gameObject);
        }

        // 공격 시 지정된 위치로 손과 타겟을 이동
        Vector3 handTargetPosition = user.transform.position + (dir_x > 0 ? leftAttackLocalPosition : rightAttackLocalPosition);
        while (Vector2.Distance(hand.transform.position, handTargetPosition) >= 0.1f && hand.IsHolding)
        {
            Vector3 direction = (handTargetPosition - hand.transform.position).normalized;
            hand.transform.position += handMoveSpeed * direction * Time.deltaTime;
            target.transform.position += handMoveSpeed * direction * Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(singleSwingWaitTime);
        swordSwing.TriggerSkill(user, target);
    }
}
