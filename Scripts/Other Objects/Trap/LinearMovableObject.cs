using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMovableObject : MonoBehaviour
{
    public float moveSpeed = 3.0f;
    public float useX = 1.0f;
    public float useY = 0.0f;
    public float reStartTime = 1.0f;

    private float lastTime = 0.0f;
    private float dirX;
    private float dirY;
    private Rigidbody2D rigid;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        lastTime = Time.time;
    }
    public void Update()
    {
        float Dir_x = transform.localScale.x;
        float Dir_y = transform.localScale.y;
        float Dir_z = transform.localScale.z;

        float time = Time.time;
        if (time - lastTime < reStartTime)
        {
            Move();
        }
        else
        {
            lastTime = time;
            if (useX == 1.0f)
            {
                if (Dir_x > 0)
                {
                    Dir_x *= -1;
                }
                else
                {

                    Dir_x = Mathf.Abs(transform.localScale.x);
                }
            }
            else if (useY == 1.0f)
            {
                if (Dir_y > 0)
                {
                    Dir_y *= -1;
                }
                else
                {

                    Dir_y = Mathf.Abs(transform.localScale.x);
                }
            }
            transform.localScale = new Vector3(Dir_x, Dir_y, Dir_z);
        }

    }
    public void Move()
    {
        dirX = transform.localScale.x;
        dirY = transform.localScale.y;
        rigid.velocity = new Vector2(dirX * useX * moveSpeed, dirY * useY * moveSpeed);
    }
}
