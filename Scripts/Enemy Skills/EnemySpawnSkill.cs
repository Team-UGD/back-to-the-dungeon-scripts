using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnSkill : EnemySkill
{
    [SerializeField] private List<Condition> enemies = new List<Condition>();

    private struct Condition
    {
        public Enemy prefab;
        public int count;
    }

    public override void TriggerSkill(Attacker user, GameObject target)
    {
        throw new System.NotImplementedException();
    }
}
