using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Skills/Close Attack Skill")]
public class CloseAttackSkill : EnemySkill
{
    [SerializeField] private MeleeWeapon weaponPrefab;
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private float strWeight = 1f;
    [SerializeField] private float attackTryRange = 3f; // 공격 시도 범위가 0보다 작거나 같을 경우에는 무조건 공격 실행함.

    public override void TriggerSkill(Attacker user, GameObject target)
    {
        if (attackTryRange > 0f && Vector2.Distance(user.transform.position, target.transform.position + new Vector3(0f, 0.75f)) > attackTryRange)
        {
            return;
        }

        var meleeWeapons = user.GetComponentsInChildren<MeleeWeapon>();

        for (int i = 0; i < meleeWeapons.Length; i++)
        {
            if (meleeWeapons[i].GetType() == weaponPrefab.GetType())
            {
                float damage = baseDamage + strWeight * GetSTR(user);
                meleeWeapons[i].Attack(damage);
                return;
            }
        }
    }
}
