using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Weapon Change Info")]
public class WeaponChangeInfo : ScriptableObject
{
    [SerializeField] private SerializableDictionary<Weapon, WeaponInfo> weaponChangeInformations = new SerializableDictionary<Weapon, WeaponInfo>();

    /// <summary>
    /// Weapon 교체 시 필요한 정보를 반환한다.
    /// </summary>
    /// <param name="weapon">무기</param>
    /// <returns>WeaponInfo struct</returns>
    public WeaponInfo GetWeaponInfo(Weapon weapon)
    {
        if (weapon)
        {
            var weaponType = weapon.GetType();
            foreach (var key in weaponChangeInformations.Keys)
            {
                if (key.GetType() == weaponType)
                    return weaponChangeInformations[key];
            }
        }

        WeaponInfo temp = new WeaponInfo();
        temp.localScale = new Vector3(2,2,2);

        return temp;
    }

    public Queue<Weapon> GetWeaponPrefabs(IEnumerable<Type> weaponTypes)
    {
        Queue<Weapon> buffer = new Queue<Weapon>();
        var weaponPrefabs = weaponChangeInformations.Keys.ToArray();
        foreach (var weaponType in weaponTypes)
        {
            Weapon temp = weaponPrefabs.FirstOrDefault(w => w.GetType() == weaponType);
            if (temp != null)
                buffer.Enqueue(temp);
        }

        return buffer;
    }
}

[Serializable]
public struct WeaponInfo
{
    public Vector3 localPosition;
    public Vector3 localScale;
}