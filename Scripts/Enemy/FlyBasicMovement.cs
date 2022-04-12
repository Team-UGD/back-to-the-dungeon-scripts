using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyBasicMovement : MonoBehaviour
{
    Rigidbody2D rigid;
    private float dir;
    private EnemyPathfinder enemyPathfinder;

    private float lastTime = 0.0f;
    private float reStartTime = 3.0f;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        enemyPathfinder = GetComponent<EnemyPathfinder>();
    }
    private void Start()
    {
        lastTime = Time.time;
        GetComponent<Entity>().OnDeath += () => enabled = false;
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
            if (Dir_x > 0)
            {
                Dir_x *= -1;
            }
            else
            {

                Dir_x = Mathf.Abs(transform.localScale.x);
            }
            transform.localScale = new Vector3(Dir_x, Dir_y, Dir_z);
        }
    }
    public void Move()
    {
        dir = transform.localScale.x;
        if (enemyPathfinder.PathfindingState == EnemyPathfinder.State.HasNoTarget)
        {
            rigid.velocity = new Vector2(dir * 2.0f, 0);
        }
    }
}