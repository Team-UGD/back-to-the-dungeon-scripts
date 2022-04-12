#define BOSSPATTERN_NEW
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Skills/BossPattern")]

public partial class BossPattern : EnemySkill
{
#if BOSSPATTERN_NEW

    [SerializeField] private List<SkillSet> skillSequence = new List<SkillSet>();

    [Serializable]
    private struct SkillSet
    {
        public EnemySkill skill;
        [Min(0f)] public float waitTime;
    }

    private void OnEnable()
    {
        // 둘 다 0일 경우에만 자동으로 초기화함.
        if (castingTime > 0 || baseCoolDown > 0)
            return;

        this.castingTime = 0f;
        this.baseCoolDown = 0f;
        for (int i = 0; i < skillSequence.Count; i++)
        {
            float t = skillSequence[i].waitTime;
            castingTime += t;
            baseCoolDown += t;
        }
    }

    public override void TriggerSkill(Attacker user, GameObject target)
    {
        user.StartCoroutine(Combo(user, target));
    }

    private IEnumerator Combo(Attacker user, GameObject target)
    {
        for (int i = 0; i < skillSequence.Count; i++)
        {
            if (user.EnemyComponent.IsDead)
                yield break;

            var skill = skillSequence[i];
            skill.skill.TriggerSkill(user, target);
            if (skill.waitTime > 0f)
                yield return new WaitForSeconds(skill.waitTime);
        }
    }

#endif
}


public partial class BossPattern
{
#if BOSSPATTERN_LEGACY
    //사용할 스킬 리스트
    [SerializeField] private List<EnemySkill> skills = new List<EnemySkill>();

    [Space]
    //다음 스킬로 넘어가는 시간
    // ex) waitTime의 0번째 값이 5라면 skills의 0번째 스킬 사용 후 5초가 지난 다음 1번째 스킬을 사용
    //     현재 마지막 값은 버려짐
    [SerializeField] private List<float> waitTime = new List<float>();

    [SerializeField] private bool concurrentUse;

    private void OnEnable()
    {
        int i = 0;
        this.castingTime = 0;
        this.baseCoolDown = 0;
        while (i < skills.Count - 1)
        {
            castingTime += waitTime[i];
            baseCoolDown += waitTime[i];

            skills[i].baseCoolDown = waitTime[i];

            i++;
        }
        castingTime += 2;
        baseCoolDown += 2;
    }

    public override void TriggerSkill(Attacker user, GameObject target)
    {
        user.StartCoroutine(Combo(user, target));

        //Debug.Log("skill name : "+ this.name);
        //Debug.Log("skill count : " + skills.Count);
        //Debug.Log("skill 1 : " + skills[0]);
        //Debug.Log("waitTime count : " + waitTime.Count);
        //Debug.Log("waitTime 1 : " + waitTime[0]);
        //Debug.Log("waitTime 2 : " + waitTime[1]);
    }

    private IEnumerator Combo(Attacker user, GameObject target)
    {
        int i = 0;
        while (i < skills.Count)
        {
            skills[i].TriggerSkill(user, target);
            if (!concurrentUse)
                yield return new WaitForSeconds(waitTime[i]);
            i++;
        }
    }

#endif
}