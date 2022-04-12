using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFireTest : MonoBehaviour
{
    //게임오브젝트 Bullet 생성
    //발사체 발사 간격
    [SerializeField] GameObject Bullet;
    [SerializeField] Transform pos;
    [SerializeField] float cooltime;
    private float currenttime;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 len = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float z = Mathf.Atan2(len.y, len.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, z);

        if (currenttime <= 0)
        {
            //
            if (Input.GetMouseButton(0))
            {
                //instantiate(원본 오브젝트, 생성위치, 회전);
                Instantiate(Bullet, pos.position, transform.rotation);
            }
            currenttime = cooltime;
        }
        currenttime -= Time.deltaTime;

    }
}
   
