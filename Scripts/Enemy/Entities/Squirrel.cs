using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squirrel : Enemy
{
    Animator animator;
    [SerializeField] public DashSkill dashSkill;
    [SerializeField] bool isFalling;
    Rigidbody2D rigidbody;
    EnemyPathfinder enemyPathfinder;
    WalkBasicMovement walkBasicMovement;

    [SerializeField] private bool isDash;

    private void Start()
    {
        GetComponent<EnemyAttacker>().OnSkillTriggered += s =>
        {
            if (s is DashSkill && animator)
            {
                animator.SetBool("isDash", true);
                animator.SetBool("IsFollow", true);
                animator.SetBool("IsReady", false);
                enemyPathfinder.canFindPath = false;

                isDash = true;
                walkBasicMovement.enabled = false;
                enemyPathfinder.enabled = false;

                //damage를 str을 직접조작해서 데미지를 조작
                STR = (uint)dashSkill.baseDamage;
                StartCoroutine(SetDashAnimFalse());
            }
        };
    }



    void Awake()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
        enemyPathfinder = GetComponent<EnemyPathfinder>();
        walkBasicMovement = GetComponent<WalkBasicMovement>();

        animator.SetBool("IsFollow", false);
        animator.SetBool("IsReady", true);
    }

    public override float Health { get => base.Health; protected set => base.Health = value; }

    public override void TakeDamage(float damage, Vector2 hitPosition, Vector2 hitNormal, bool isContinuous = false, float power = 1, float use = 1)
    {
        base.TakeDamage(damage, hitPosition, hitNormal);
    }

    public override void Heal(float healthToHeal)
    {

    }

    protected override void Die()
    {
        base.Die();
        animator.SetTrigger("isDie");
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (collision.gameObject.tag == "Player" && isDash)
        {
            //float dir = collision.transform.position.x - transform.position.x ;
            //Vector2 hitPosition = new Vector2(dir, 1); 
            //Vector2 hitNormal = transform.position - collision.transform.position + new Vector3(0, 0.75f);
            //collision.gameObject.GetComponent<Hero>().TakeDamage(dashSkill.DashDamage, hitPosition, hitNormal);


            //str을 조정하는 방식으로 변경
            //collision.gameObject.GetComponent<Hero>().Damage = dashSkill.DashDamage; //기존 방식
        }
        if(collision.gameObject.CompareTag("Enemy") && isDash)
        {
            Dir_x = transform.localScale.x;

            rigidbody.velocity = new Vector2(rigidbody.velocity.x * -1, rigidbody.velocity.y);
            transform.localScale = new Vector3(Dir_x * -1, 1, 1);

        }
    }

    private void Update()
    {
        if(enemyPathfinder.PathfindingState == EnemyPathfinder.State.Default && !isDash)
        {
            animator.SetBool("IsReady", false);
            animator.SetBool("IsFollow", true);
            animator.SetBool("Idle", false);
            walkBasicMovement.enabled = false;
        }
    }


    //test!
    private float Dir_x;
    private void FixedUpdate()
    {
        Dir_x = transform.localScale.x;
        Vector2 frontVec = new Vector2(rigidbody.position.x + 1.2f * Dir_x, rigidbody.position.y);
        RaycastHit2D raycastHit = Physics2D.Raycast(frontVec, Vector3.down, 1.0f, LayerMask.GetMask("Ground"));
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0)); //초록색

        if (raycastHit.collider == null && !isFalling && isDash)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x * -1, rigidbody.velocity.y);
            transform.localScale = new Vector3(Dir_x * -1,1,1);
        }

        if ((walkBasicMovement.OnOneBlock || walkBasicMovement.MoveSpeed == 0) && !isDash)
        {
            if (enemyPathfinder.PathfindingState == EnemyPathfinder.State.Default)
            {
                animator.SetBool("Idle", false);
                return;
            }
            animator.SetBool("Idle", true);
        }
        else
        {
            animator.SetBool("Idle", false);
        }
    }

    IEnumerator SetDashAnimFalse()
    {
        yield return new WaitForSeconds(dashSkill.castingTime );

        //str을 직접조작해서 데미지를 조작
        isDash = false;
        walkBasicMovement.enabled = true;
        enemyPathfinder.enabled = true;
        STR = 5;

        animator.SetBool("IsReady", true);
        animator.SetBool("IsFollow", false);
        animator.SetBool("isDash", false);
    }
}
