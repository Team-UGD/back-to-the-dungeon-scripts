using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseMaxHealth : Item
{
    [SerializeField] int increment;

    private void Awake()
    {
        ItemType = Type.IncreaseMaxHealth;
        ItemName = "IncreaseMaxHealth";
    }

    public override void GetItem(GameObject player)
    {
        Hero hero = player.GetComponent<Hero>();
        hero.MaxHealth += increment;
        hero.Heal(increment);
    }
}
