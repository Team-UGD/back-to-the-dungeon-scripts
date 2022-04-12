using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseStamina : Item
{
    [SerializeField] int increment;

    private void Awake()
    {
        ItemType = Type.IncreaseMoveSpeed;
        ItemName = "IncreaseMoveSpeed";
    }

    public override void GetItem(GameObject player)
    {
        player.GetComponent<Hero>().MaxStamina += increment;

    }
}

