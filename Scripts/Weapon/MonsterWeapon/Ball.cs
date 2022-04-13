using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : Entity
{
    [SerializeField]
    LayerMask GroundLayer;
    private float damage = 20.0f;
    public float reStartTime = 8.0f;
    public GameObject ballExplosion;

    [System.NonSerialized]
    CircleCollider2D circleCollider;
    private float startTime;
    Rigidbody2D rigid2d;
    public Vector2 userPos;
    private SpriteRenderer sprite;
    private Animator animator;
    private PlayerMovement playerMovement; 
    private void Start()
    {
        MaxHealth = 40f;
        startTime = Time.time;
    }
    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        rigid2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }
    public override float Health
    {
        get => base.Health;

        protected set
        {
            base.Health = value;
            if ( health> 0 && health <= 30)
            {
                Color start = Color.white;
                Color goal = Color.red;
                sprite.color = Color.Lerp(start, goal, (35 - health) / 35);
            }
            else if (health <= 0)
            {
                Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
                Vector2 curPos = gameObject.transform.position;
                sprite.color = Color.white;
                Destroy(gameObject);
                GameObject explosion = Instantiate(ballExplosion, curPos, rotation);
            }
        }
    }
    public override void TakeDamage(float damage, Vector2 hitPosition, Vector2 hitNormal, bool isContinuous = false, float power = 1, float use = 1)
    {
        base.TakeDamage(damage, hitPosition, hitNormal, isContinuous);
    }

    private void Update()
    {
        float time = Time.time;
        if (time - startTime > reStartTime)
        {
            animator.SetBool("isDie", true);
            Destroy(gameObject, 0.5f);
        }
        
    }
    private void FixedUpdate()
    {

        Bounds bounds = circleCollider.bounds;
        Vector2 GroundCheck = new Vector2(bounds.center.x, bounds.min.y);

        //낭떠러지에 떨어질 시 오브젝트 삭제
        RaycastHit2D raycast = Physics2D.Raycast(GroundCheck, Vector2.down, GroundLayer);
        Debug.DrawRay(GroundCheck, Vector3.down, new Color(0, 1, 0)); //초록색

        if (raycast.collider == null)
        {
            animator.SetBool("isDie", true);
            Destroy(gameObject, 0.5f);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.collider.CompareTag("Player"))
        {
            Attack(collision);
        }
    }
    private void Attack(Collision2D collision)
    {
        Vector2 hitPosition = collision.transform.position;
        Vector2 hitNormal = collision.GetContact(0).normal;
        collision.collider.GetComponent<Entity>()?.TakeDamage(damage, hitPosition, hitNormal, false, 3);
        Destroy(gameObject);
    }
    public void SetBullet(Vector2 shootPos,Vector2 Pos)
    {
        userPos = Pos;
        rigid2d.AddForce(shootPos * 2f, ForceMode2D.Impulse);
    }
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Debug.Log("count = " + count);
    //    if (collision.gameObject.layer == LayerMask.NameToLayer("Bullet"))
    //    {
    //        count -= 1;
    //    }
    //}
}
