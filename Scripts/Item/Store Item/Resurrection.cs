using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resurrection : Item
{
    private void Awake()
    {
        ItemType = Type.Resurrection;
        ItemName = "Resurrection";
    }

    public override void GetItem(GameObject player)
    {
        player.GetComponent<Hero>().ResurrectionChance = true;
        UIManager.Instance.SetResurrectionImage(true);
    }
}
