using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invincible : Item, IObjectValue<float>
{
    [SerializeField] float time;

    public float Value { get => time; set => time = value; }

    private void Awake()
    {
        ItemType = Type.Invincible;
        ItemName = "Invincible";
    }

    public override void GetItem(GameObject player)
    {
        player.GetComponent<Hero>().SetPlayerInvincibleMode(time);
    }
}
