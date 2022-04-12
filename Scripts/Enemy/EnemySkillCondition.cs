using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 스킬을 사용하기 위한 조건
/// </summary>
[System.Serializable]
public class EnemySkillCondition
{
    /// <summary>
    /// 실제 사용할 스킬
    /// </summary>
    public EnemySkill skill;

    /// <summary>
    /// 스킬 사용 비중.
    /// </summary>
    /// <remarks>
    /// 스킬 비중의 총합을 가능한 1로 설정해주세요. 총합이 1이 아니여도 비중의 비율에 따라 정상 작동하지만 직관적으로 설정해주는게 좋습니다.
    /// </remarks>
    public float skillUseWeight;

    private float remainingCooldown;

    /// <summary>
    /// 스킬 사용 후 남은 쿨타임.
    /// </summary>
    public float RemainingCooldown
    {
        get => remainingCooldown;
        set
        {
            remainingCooldown = Mathf.Clamp(value, 0f, skill?.baseCoolDown ?? 0f);
        }
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(EnemySkillCondition))]
public class EnemySkillConditionDrawer : PropertyDrawer
{
    private const float fieldHeight = 20f;
    private const float intervalHeight = 2f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //EditorGUI.PropertyField(position, property, label, false);
        Rect current = position;

        current.height = fieldHeight;
        current.width = position.width * 0.63f;
        Rect skillRect = current;

        current.x += current.width + 3f;
        current.width = position.width * 0.35f;
        Rect weightRect = current;

        var skill = property.FindPropertyRelative("skill");
        var weight = property.FindPropertyRelative("skillUseWeight");

        EditorGUI.LabelField(skillRect, "Skill");
        EditorGUI.LabelField(weightRect, "Skill Weight");

        skillRect.y += fieldHeight + intervalHeight;
        weightRect.y += fieldHeight + intervalHeight;

        EditorGUI.PropertyField(skillRect, skill, GUIContent.none);
        EditorGUI.PropertyField(weightRect, weight, GUIContent.none);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (fieldHeight + intervalHeight) * 2f;
    }
}
#endif