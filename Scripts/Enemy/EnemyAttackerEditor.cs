#if UNITY_EDITOR
using System.Linq;
using UnityEditor;

[CustomEditor(typeof(EnemyAttacker)), CanEditMultipleObjects]
public class EnemyAttackerEditor : Editor
{
    private EnemyAttacker enemyAttacker;

    private void OnEnable()
    {
        enemyAttacker = target as EnemyAttacker;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // ISkillFirePosition interface를 상속받은 Skill이 아직 Dictionary에 없을 경우 자동으로 추가해줌.
        var skillsToNeedFirePosition = enemyAttacker.skills.Where(s => s.skill is ISkillFirePosition).Select(s => s.skill).Distinct().ToArray();
        if (enemyAttacker.enemySkillFirePositions.Count != skillsToNeedFirePosition.Length)
        {
            foreach (var skill in skillsToNeedFirePosition)
            {
                if (!enemyAttacker.enemySkillFirePositions.ContainsKey(skill))
                {
                    enemyAttacker.enemySkillFirePositions.Add(skill, null);
                }
            }
        }

        // skills 리스트에서 enemySkillFirePositions의 key값이 포함된 SkillCondition이 제거된 경우 Dictionary에서도 제거함.
        foreach (var key in enemyAttacker.enemySkillFirePositions.Keys.ToArray())
        {
            if (enemyAttacker.skills.Count(s => s.skill == key) == 0)
            {
                enemyAttacker.enemySkillFirePositions.Remove(key);
            }
        }

    }
}
#endif