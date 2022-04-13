using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Coin아이템의 클레스
/// </summary>
public class Coin : Item, IObjectValue<int>
{
    [SerializeField] int GoldAmount;

    public int Value { get => this.GoldAmount; set => this.GoldAmount = value; }

    private void Awake()
    {
        ItemType = Type.Coin;
        ItemName = "Coin";
    }

    public override void GetItem(GameObject player)
    {
        GameManager.Instance.Gold += GoldAmount;
    }

}