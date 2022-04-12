using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpShotGun : Weapon
{
    public GameObject bullet; 
    // public GameObject Fireposition;

    private int bulletshot=7;
    private bool playerPressMouse0;

    public override void Fire()
    {
        //Debug.Log(onFire);
        if (WeaponState == State.ReadyToFire && onFire == true && cur_Bullet > 0)
        {
            SetWeaponState();

            muzzleFlash.Play();
            if (audioSource)
                audioSource.PlayOneShot(fireSound);

            float z = Fireposition.transform.rotation.z;

            for (int i = 0; i < bulletshot; i++)
            {
                var tmp = Instantiate(bullet, Fireposition.transform.position, transform.rotation).GetComponent<Bullet>(); // 총알 소환
                tmp.SetBullet(this.damage, Convert_V3ctor(z + (30 - i * 8)), max_distance, true, Bullet.Target.Enemy); // 데미지, 회전, 최대거리 등 전달
            }

            cur_Bullet--;

            if (cur_Bullet <= 0)
                WeaponState = State.Empty;

            Invoke("PullPumpSound", 0.5f);
        }
    }

    public override void Reload()
    {
        if (WeaponState == State.Reloading || cur_Bullet >= max_Bullet)
            return;

        IsReload = true;

        if (audioSource)
            audioSource.PlayOneShot(reloadSound);
        StartCoroutine(IESgReload());

    }

    IEnumerator IESgReload()
    {
        WeaponState = State.Reloading;

        yield return new WaitForSeconds(reload_time);

        IsReload = false;
        cur_Bullet += 1;
        WeaponState = State.ReadyToFire;

        if (cur_Bullet < max_Bullet && !playerPressMouse0)
            Invoke("Reload", 0.1f);
        playerPressMouse0 = false;
    }

    protected override void SetEmptyMagWeaponSprite()
    {
    }

    protected override void SetLoadedWeaponSprite()
    {
    }

    void PullPumpSound()
    {
        audioSource.PlayOneShot(reloadSound);
    }

    Vector3 Convert_V3ctor(float f)
    {
        Vector3 vector3 = (Vector3)(Fireposition.transform.right); // 방향 구함
        return (Quaternion.Euler(0f, 0f, f) * vector3).normalized; // 반동에 따른 회전 후 정규화
    }

    protected override void Update()
    {
        base.Update();

        if ((Input.GetKeyDown(KeyCode.Mouse0) || Input.GetAxisRaw("right trigger") == 1 ? true : false ) && IsReload == true)
        {
            playerPressMouse0 = true;
        }
    }

}
