using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPotion: Item, IObjectValue<int>
{
    [SerializeField] int healAmount;

    public int Value { get => healAmount; set => healAmount = value; }

    private void Awake()
    {
        ItemType = Type.Potion;
        ItemName = "Potion";
    }

    public override void GetItem(GameObject player)
    {
        player.GetComponent<Hero>().Heal(healAmount);
    }
}
