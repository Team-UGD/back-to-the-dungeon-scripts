using UnityEngine;

public class Bullet : MonoBehaviour
{
    /* 
     * 총알 날아감 //bool 타입 생성 
     * --1.직선  **
     * --2.포물선 ** 
     * 
     * 맞는 물체 구별 
     * --1.몬스터 **
     * --2.사물  **
     * 
     * 파괴
     * --1.총알 **
     * 
     * 총알 발사 후 일정 시간 이후 파괴 **
     * 
     */

    public enum Target
    {
        Player,
        Enemy,
        Both
    }

    [SerializeField] protected float bulletSpeed;//총알 속도
    [SerializeField] protected float attackRange;//사거리

    protected float damage;
    protected float maxDistance;
    protected Vector2 direction;
    protected Target target = Target.Both;
    protected bool isSetup = false;
    protected bool isContinuous;

    /// <summary>
    /// 직선 포물선 구분 false 기본값=직선
    /// </summary>
    protected bool isStraight = true; //임시

    protected Rigidbody2D rigid2D;
    protected Vector2 firePos;

    public Target TargetInfo { get => target; set => target = value; }

    public bool IsContinuous { get => isContinuous; }

    private void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
    }

    protected void Start()
    {
        firePos = transform.position;

        if (isStraight == false)
        {
            rigid2D.velocity = direction * bulletSpeed;
        }

    }

    protected void FixedUpdate()
    {
        if (!isSetup)
            return;

        //총알 전진
        float dist = Vector2.Distance(firePos, transform.position);

        //if (isStraight == true)
        //{
        //    rigid2D.velocity = direction.normalized * bulletSpeed;
        //}
        if (isStraight == false)
        {
            float angle = Mathf.Atan2(rigid2D.velocity.y, rigid2D.velocity.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, 0, angle);
        }


        if (dist > maxDistance)
        {
            DestroyBullet();
        }

    }

    // 총알 충돌 시 처리
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        //    if (!collision.CompareTag("Player") && !collision.CompareTag("Enemy") && !collision.CompareTag("Projectile") && !collision.CompareTag("Item"))
        //    {
        //        DestroyBullet();
        //        return;
        //     

        if (!isSetup)
            return;

        if (collision.CompareTag("Ground"))
        {
            DestroyBullet();
            return;
        }

        switch (target)
        {
            case Target.Player:
                if (collision.CompareTag(target.ToString()))
                    Attack(collision);
                break;
            case Target.Enemy:
                if (collision.CompareTag(target.ToString()))
                    Attack(collision);
                break;
            case Target.Both:
                    Attack(collision);
                break;
        }      
    }

    protected virtual void Attack(Collider2D collision)
    {
        if (collision.gameObject.layer != 8)
        {

            Entity entity = collision.GetComponent<Entity>();
            if (entity == null)
                return;

            Vector2 hitPosition = collision.ClosestPoint(transform.position);
            Vector2 hitNormal = new Vector2(hitPosition.x - collision.transform.position.x, collision.transform.position.y).normalized;

            //거리비례 데미지
            if (Vector2.Distance(firePos, hitPosition) > attackRange)
                damage *= 0.5f;

            entity.TakeDamage(damage, hitPosition, hitNormal, isContinuous);
        }

        DestroyBullet();
    }

    /// <summary>
    /// DestroyBullet-총알 제거 함수
    /// </summary>
    protected void DestroyBullet()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Bullet의 필수 값을 초기화함.
    /// </summary>
    public void SetBullet(float damage, Vector2 direction, float maxDistance, bool isStraight,bool isContinuous)
    {
        this.damage = damage;
        this.direction = direction;
        this.maxDistance = maxDistance;
        this.isStraight = isStraight;
        this.isContinuous = isContinuous;

        if (isStraight == true)
        {
            rigid2D.velocity = direction.normalized * bulletSpeed;
        }

        isSetup = true;
    }

    /// <summary>
    /// Bullet의 필수 값을 초기화함.
    /// </summary>
    public void SetBullet(float damage, Vector2 direction, float maxDistance, bool isStraight, float bulletSpeed, bool isContinuous = false)
    {
        this.damage = damage;
        this.direction = direction;
        this.maxDistance = maxDistance;
        this.isStraight = isStraight;
        this.bulletSpeed = bulletSpeed;
        this.isContinuous = isContinuous;

        if (isStraight == true)
        {
            rigid2D.velocity = direction.normalized * bulletSpeed;
        }

        isSetup = true;
    }

    /// <summary>
    /// Bullet의 필수 값을 초기화함.
    /// </summary>
    /// <param name="target">공격 대상의 타입</param>
    public void SetBullet(float damage, Vector2 direction, float maxDistance, bool isStraight, Target target, bool isContinuous = false)
    {
        this.target = target;
        SetBullet(damage, direction, maxDistance, isStraight, isContinuous);
    }


    public void SetBullet(float damage, Vector2 direction, float maxDistance, bool isStraight, float bulletSpeed, Target target, bool isContinuous = false)
    {
        this.target = target;
        SetBullet(damage, direction, maxDistance, isStraight, bulletSpeed, isContinuous);
    }

}

