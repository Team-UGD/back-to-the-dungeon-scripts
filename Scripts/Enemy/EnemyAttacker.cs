using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 적의 공격 기능
/// </summary>
public class EnemyAttacker : Attacker
{
    // Serialized Fields
    public float attackAttemptRate = 1f;
    [SerializeField] private GameObject fixedTarget;

    /// <summary>
    /// 사용할 스킬 리스트
    /// </summary>
    [Space]
    public List<EnemySkillCondition> skills = new List<EnemySkillCondition>();

    public SerializableDictionary<EnemySkill, Transform> enemySkillFirePositions = new SerializableDictionary<EnemySkill, Transform>();

    /// <summary>
    /// 스킬 사용 시 발동할 이벤트
    /// </summary>
    public event SkillTriggerDelegate OnSkillTriggered;


    // Private Fields
    private EnemyDetection detection;

    private float lastAttackTime;
    private float attackRate;

    // 타입

    // 실행할 스킬 반환 프로퍼티
    private EnemySkillCondition SkillToTrigger
    {
        get
        {
            // 사용 가능한 스킬 필터링
            var skillsToUse = skills.Where(s => s != null && s.skill != null && s.skillUseWeight > 0).ToArray();

            // 스킬 비중에 따른 확률 계산 후 발동할 스킬 선택
            float totalWeightOfSkills = skillsToUse.Sum(s => s.skillUseWeight);
            float probability = Random.Range(0f, 1f);
            //Debug.Log(probability);
            float cumulativeProb = 0f;

            EnemySkillCondition skillToTrigger = null;

            foreach (var skill in skillsToUse)
            {
                float tempWeight = skill.skillUseWeight;
                tempWeight /= totalWeightOfSkills;
                //Debug.Log(tempWeight);
                cumulativeProb += tempWeight;

                if (probability <= cumulativeProb)
                {
                    skillToTrigger = skill;
                    break;
                }
            }

            return skillToTrigger;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        detection = GetComponent<EnemyDetection>();
    }

    private void Update()
    {
        // Entity의 파생클래스가 게임오브젝트에 컴포넌트로 존재할 때만 사망 여부에 따라 Attack() 사용.
        if (skills.Count <= 0)
            return;

        if (enemy != null && enemy.IsDead)
            return;

        if (detection != null && detection.Target == null || detection == null && fixedTarget == null)
            return;

        if (Time.time >= lastAttackTime + attackRate)
        {
            Attack();
        }

    }

    private void Attack()
    {
        EnemySkillCondition skillToTrigger = SkillToTrigger;

        // 스킬 사용
        if (skillToTrigger != null && skillToTrigger.RemainingCooldown <= 0f)
        {
            if (skillToTrigger.skill is ISkillFirePosition && enemySkillFirePositions.ContainsKey(skillToTrigger.skill))
            {
                (skillToTrigger.skill as ISkillFirePosition).FirePosition = enemySkillFirePositions[skillToTrigger.skill];
            }

            //skillToTrigger.skill.TriggerSkill(gameObject);
            if (fixedTarget != null)
                skillToTrigger.skill.TriggerSkill(this, fixedTarget);
            else
                skillToTrigger.skill.TriggerSkill(this, detection.Target);

            skillToTrigger.RemainingCooldown = skillToTrigger.skill.baseCoolDown;
            //Debug.Log($"[{typeof(EnemyAttacker)}] 스킬 발동. 쿨타임: {skillToTrigger.skill.baseCoolDown}");
            OnSkillTriggered?.Invoke(skillToTrigger.skill);
            StartCoroutine(CalculateCooldown(skillToTrigger));

            attackRate = skillToTrigger.skill.castingTime + attackAttemptRate;
        }
        else
        {
            attackRate = attackAttemptRate;
        }

        lastAttackTime = Time.time;
    }


    private IEnumerator CalculateCooldown(EnemySkillCondition skillCondition)
    {
        while (skillCondition.RemainingCooldown > 0)
        {
            yield return null;
            skillCondition.RemainingCooldown -= Time.deltaTime;
            //Debug.Log($"[{typeof(EnemyAttacker)}] 스킬 이름: {skillCondition.skill.skillName} 남은 쿨타임: {skillCondition.RemainingCooldown}");
        }
    }
}

/// <summary>
/// 스킬 사용 대리자
/// </summary>
/// <param name="skillTriggered">실행 된 스킬</param>
public delegate void SkillTriggerDelegate(EnemySkill skillTriggered);