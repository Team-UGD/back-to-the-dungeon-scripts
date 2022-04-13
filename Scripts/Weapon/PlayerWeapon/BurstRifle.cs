using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BurstRifle : Weapon
{
    public GameObject bullet;
   // public GameObject Fireposition;

    public override void Fire()
    {
        if (WeaponState == State.ReadyToFire && onFire == true && cur_Bullet > 0)
        {
            SetWeaponState();

            onFire = false;

            Vector3 ricoil = Convert_V3ctor();
            StartCoroutine(Delay(BurstFire,ricoil));

            if (cur_Bullet <= 0)
                WeaponState = State.Empty;
        }
    }

    void BurstFire(Vector3 ricoil)
    {
        muzzleFlash.Play();
        if (audioSource)
            audioSource.PlayOneShot(fireSound);
        var tmp = Instantiate(bullet, Fireposition.transform.position, transform.rotation).GetComponent<Bullet>(); // 총알 
        tmp.SetBullet(this.damage, ricoil, max_distance, true, Bullet.Target.Enemy); // 데미지, 회전, 최대거리 등 전달
        cur_Bullet--;
    }

    IEnumerator Delay(Action<Vector3> Func,Vector3 ricoil)
    {
        for (int i = 0; i < 3; i++)
        {
            Func(ricoil);
            yield return new WaitForSeconds(0.1f);
        }
    }

    protected override void SetEmptyMagWeaponSprite()
    {
        SetWeaponSprite("Weapon/EmptyMagWeapon/M16");
    }

    protected override void SetLoadedWeaponSprite()
    {
        SetWeaponSprite("Weapon/LoadedWeapon/M16");
    }

    Vector3 Convert_V3ctor()
    {
        float cur_recoil = UnityEngine.Random.Range(-recoil, recoil); // 반동값 랜덤으로 생성
        Vector3 vector3 = (Vector3)(Fireposition.transform.right); // 방향 구함
        return (Quaternion.Euler(0f, 0f, cur_recoil) * vector3).normalized; // 반동에 따른 회전 후 정규화
    }
}
