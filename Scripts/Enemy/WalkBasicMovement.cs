using UnityEngine;

public class WalkBasicMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 3.0f;
    [SerializeField] LayerMask layer;

    Rigidbody2D rigid;
    private EnemyPathfinder enemyPathfinder;
    private Collider2D enemyCollider;
    private float Dir_x;
    private float Dir_y;
    private float Dir_z;

    private RaycastHit2D frontRay;
    private RaycastHit2D backRay;

    public float MoveSpeed { get { return moveSpeed; } }

    public bool OnOneBlock
    {
        get { return (frontRay.collider != null || backRay.collider != null) == false;  }
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        enemyPathfinder = GetComponent<EnemyPathfinder>();
        enemyCollider = GetComponent<Collider2D>();

        Dir_x = transform.localScale.x;
        Dir_y = transform.localScale.y;
        Dir_z = transform.localScale.z;
    }

    private void FixedUpdate()
    {
        Dir_x = transform.localScale.x;
        Dir_y = transform.localScale.y;
        Dir_z = transform.localScale.z;

        // 땅 체크
        Bounds enemyBounds = enemyCollider.bounds;
        Vector2 frontVec = new Vector2(enemyBounds.center.x + Mathf.Sign(Dir_x), enemyBounds.min.y + 0.1f);
        Vector2 backVec = new Vector2(enemyBounds.center.x - Mathf.Sign(Dir_x), enemyBounds.min.y + 0.1f);
        frontRay = Physics2D.Raycast(frontVec, Vector3.down, 0.5f, layer);
        backRay = Physics2D.Raycast(backVec, Vector2.down, 0.5f, layer);
        Debug.DrawRay(frontVec, 0.5f * Vector3.down, new Color(0, 1, 0)); //초록색
        Debug.DrawRay(backVec, 0.5f * Vector2.down, Color.green);
        //Debug.Log("Mathf.Abs(rigid.velocity.x) = " + Mathf.Abs(rigid.velocity.x));
        if (Mathf.Abs(rigid.velocity.x) >= 0.1f && frontRay.collider == null)
        {
            // 뒤에 밟을 땅이 있을 때
            if (backRay.collider != null && rigid.velocity.y <= 0 && enemyPathfinder.PathfindingState == EnemyPathfinder.State.HasNoTarget)
            {
                rigid.velocity = new Vector2(0.1f, rigid.velocity.y);
                Dir_x *= -1;
                transform.localScale = new Vector3(Dir_x, Dir_y, Dir_z);
            }
        }

        // 벽 체크
        RaycastHit2D hitInfo = Physics2D.Raycast(enemyCollider.bounds.center + new Vector3((enemyCollider.bounds.size.x + 0.2f) * Dir_x, 0f), new Vector2(Dir_x, 0f), 0.5f, layer);
        Debug.DrawRay(enemyCollider.bounds.center + new Vector3((enemyCollider.bounds.size.x + 0.2f) * Dir_x, 0f), 0.5f * new Vector2(Dir_x, 0f), Color.red);
        if (hitInfo.collider != null)
        {
            if (Vector2.Angle(hitInfo.normal, Vector2.up) >= 60f && Mathf.Abs(rigid.velocity.y) <= 0.1f && enemyPathfinder.PathfindingState == EnemyPathfinder.State.HasNoTarget)
            {
                //rigid.velocity = new Vector2(0f, rigid.velocity.y);
                Dir_x *= -1;
                transform.localScale = new Vector3(Dir_x, Dir_y, Dir_z);
            }
        }

    }
    public void Move()
    {
        if (enemyPathfinder.PathfindingState == EnemyPathfinder.State.HasNoTarget && (frontRay.collider != null || backRay.collider != null))
        {
            rigid.velocity = new Vector2(Mathf.Sign(Dir_x) * moveSpeed, rigid.velocity.y);
            //rigid.AddForce(new Vector2(Dir_x * 100, 0), ForceMode2D.Force);
            //if (Mathf.Abs(rigid.velocity.x) > moveSpeed)
            //{
            //    rigid.velocity = new Vector2(moveSpeed * Mathf.Sign(rigid.velocity.x), rigid.velocity.y);
            //}
        }
    }
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Debug.Log($"[{typeof(WalkBasicMovement)}] {name}의 법선벡터: {collision.GetContact(0).normal}, {collision.collider.name}의 각도: {Vector2.Angle(collision.GetContact(0).normal, Vector2.up)}");
    //    if (Vector2.Angle(collision.GetContact(0).normal, Vector2.up) >= 60f)
    //    {
    //        if (Mathf.Abs(rigid.velocity.y) <= 0.1 && enemyPathfinder.PathfindingState == EnemyPathfinder.State.HasNoTarget)
    //        {
    //            Debug.Log($"{name}의 방향전환 실행!");
    //            Dir_x *= -1;
    //            transform.localScale = new Vector3(Dir_x, Dir_y, Dir_z);
    //        }
    //    }
    //}
    //private void OnCollisionStay2D(Collision2D collision)
    //{
    //    if (Mathf.Abs(rigid.velocity.x) <= 0.05f && enemyPathfinder.PathfindingState == EnemyPathfinder.State.HasNoTarget)
    //    {
    //        Dir_x *= -1;
    //        transform.localScale = new Vector3(Dir_x, Dir_y, Dir_z);

    //    }
    //}
}