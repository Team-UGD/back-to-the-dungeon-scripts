using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoShotGun : Weapon
{
    public GameObject bullet;
   // public GameObject Fireposition;

    public bool repeaterCheck;

    private int bulletshot = 7;

    public override void Fire()
    {
        if (WeaponState == State.ReadyToFire && onFire == true && cur_Bullet > 0)
        {
            SetWeaponState();

            muzzleFlash.Play();
            if (audioSource)
                audioSource.PlayOneShot(fireSound);

            onFire = false;
            float z = Fireposition.transform.rotation.z;

            for (int i = 0; i < bulletshot; i++)
            {
                var tmp = Instantiate(bullet, Fireposition.transform.position, transform.rotation).GetComponent<Bullet>(); // 총알 소환
                tmp.SetBullet(this.damage, Convert_V3ctor(z + (30 - i * 8)), max_distance, true, Bullet.Target.Enemy); // 데미지, 회전, 최대거리 등 전달
            }
            if (repeaterCheck)
                Invoke("Returntoture", attack_speed); //연사 무기 일때 활성화(주석 해제) 및 attack_speed 설정

            cur_Bullet--;

            if (cur_Bullet <= 0)
                WeaponState = State.Empty;

        }
    }


    protected override void SetEmptyMagWeaponSprite()
    {
        SetWeaponSprite("Weapon/EmptyMagWeapon/SAIGA");
    }

    protected override void SetLoadedWeaponSprite()
    {
        SetWeaponSprite("Weapon/LoadedWeapon/SAIGA");
    }


    Vector3 Convert_V3ctor(float f)
    {
        Vector3 vector3 = (Vector3)(Fireposition.transform.right); // 방향 구함
        return (Quaternion.Euler(0f, 0f, f) * vector3).normalized; // 탄환이 날아갈 각도로 발사
    }

}
