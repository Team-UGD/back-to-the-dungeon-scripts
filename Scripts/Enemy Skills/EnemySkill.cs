using System;
using UnityEngine;

public abstract class EnemySkill : ScriptableObject
{
    [Header("Skill Base Info")]
    public string skillName = "New Skill";
    public float baseCoolDown = 1f;
    public float castingTime;
    public float levelWeight = 1f;

    /// <summary>
    /// 스킬을 발동한다.
    /// </summary>
    /// <param name="user">Skill 사용자</param>
    [Obsolete("EnemySkill.TriggerSkill(user, target)을 사용해주세요.")]
    public virtual void TriggerSkill(GameObject user) { }

    /// <summary>
    /// 스킬을 발동한다. abstract로 변경될 예정
    /// </summary>
    /// <param name="user">Skill 사용자</param>
    /// <param name="target">공격할 목표</param>
    public abstract void TriggerSkill(Attacker user, GameObject target);

    /// <summary>
    /// void TriggerSkill() 메서드를 실행할 수 있는지 체크한다. <br/>
    /// user, enemyDetection, EnemyDetection.Target에 대한 null 처리를 하는 메서드이다.
    /// </summary>
    /// <param name="user">Skill 사용자</param>
    /// <param name="enemyDetection">user의 EnemyDetection 컴포넌트</param>
    /// <returns>필요한 요소 중 하나라도 null이라면 false를 반환한다.</returns>
    [Obsolete]
    protected bool CheckCanTriggerSkill(GameObject user, out EnemyDetection enemyDetection)
    {        
        if (!user)
        {
            enemyDetection = null;
            return false;
        }

        enemyDetection = user.GetComponent<EnemyDetection>();
        if (!enemyDetection || !enemyDetection.Target)
            return false;

        return true;
    }


    /// <summary>
    /// 스킬 user의 공격력을 반환한다.
    /// </summary>
    /// <param name="user">skill을 사용하는 MonoBehaviour 객체</param>
    /// <param name="checkDestroyed">user null 체크. user가 null이 아님이 보장되어 있다면 기본값을 사용하세요.</param>
    /// <returns></returns>
    protected uint GetSTR(Attacker user, bool checkDestroyed = false)
    {
        if (checkDestroyed && (user == null || user.EnemyComponent == null))
            return 0;

        return user.EnemyComponent.STR;
    }
}
