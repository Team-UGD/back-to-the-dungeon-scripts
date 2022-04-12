using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Skills/EnemyBossSmashSkill")]
public class BossSmash : EnemySkill
{
    [SerializeField] private Vector3 playerPosition;

    [System.NonSerialized]
    public float baseDamage = 15f;
    public AudioClip SmashSound;
    private Vector3 creationPosition;

    [Header("Creation")]
    [SerializeField] private BossHand bossHandPrefab;
    [SerializeField] private float creationDistanceFromTarget = 7f;

    private Dictionary<Attacker, BossHand> createdBossHandBuffer = new Dictionary<Attacker, BossHand>();

    public override void TriggerSkill(Attacker user, GameObject target)
    {

        if (createdBossHandBuffer.TryGetValue(user, out var bossHand))
        {
            // 이전에 user가 생성했던 hand가 아직 scene내에 존재한다면 즉시 종료함
            if (bossHand != null)
            {
                Debug.Log($"[{typeof(GrabSkill)}] 아직 Boss Hand가 존재합니다.");
                Destroy(bossHand.gameObject);
                return;
            }
            createdBossHandBuffer.Remove(user);
        }

        float targetPos_x = target.transform.position.x;
        // target이 user보다 왼쪽에 있는 경우
        if (targetPos_x <= user.transform.position.x)
        {
            playerPosition = target.transform.position + new Vector3(0, 1f);
            creationPosition = playerPosition + new Vector3(0, creationDistanceFromTarget);
            var leftHand = Instantiate(bossHandPrefab, creationPosition, Quaternion.identity);
            leftHand.transform.localScale = new Vector3(-1f, 1f, 1f);
            createdBossHandBuffer.Add(user, leftHand);

            InitializeBossHand(leftHand, target);
            leftHand.StartCoroutine(SmashHand(user, leftHand, target));
            Debug.Log($"[{typeof(GrabSkill)}] 왼손 그랩스킬 활성화");
        }
        // target이 user보다 오른쪽에 있는 경우
        else
        {
            playerPosition = target.transform.position + new Vector3(0, 1f);
            creationPosition = playerPosition + new Vector3(0, creationDistanceFromTarget);
            var rightHand = Instantiate(bossHandPrefab, creationPosition, Quaternion.identity);
            createdBossHandBuffer.Add(user, rightHand);

            InitializeBossHand(rightHand, target);
            rightHand.StartCoroutine(SmashHand(user, rightHand, target));
            Debug.Log($"[{typeof(GrabSkill)}] 오른손 그랩스킬 활성화");
        }
    }
    private void InitializeBossHand(BossHand hand, GameObject target)
    {
        hand.gameObject.SetActive(true);
        hand.SetSmashSkillInfo(baseDamage, target);
    }

    private IEnumerator SmashHand(Attacker user, BossHand hand, GameObject target)
    {
        yield return new WaitForSeconds(0.5f);

        hand.bossHandRigidbody.velocity = new Vector2(0, -20);
    }
}