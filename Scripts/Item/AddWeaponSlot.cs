using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddWeaponSlot : Item
{
    private void Awake()
    {

    }

    public override void GetItem(GameObject player)
    {
        PlayerShooter playerShooter = player.GetComponent<PlayerShooter>();

        if (playerShooter.WeaponSlotCapacity > 2)
            Debug.Log("Already Maximum WeaponSlotCapacity");
        else
            playerShooter.WeaponSlotCapacity += 1;

    }
}
