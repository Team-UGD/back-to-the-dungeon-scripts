#define BETA1
using Pathfinding;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 장애물을 피해 타겟의 위치로 이동시키는 클래스
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Seeker), typeof(EnemyDetection))]
public class EnemyPathfinder : MonoBehaviour
{
    #region [Serialzied Fields]

    //public Transform target; // 나중에 EnemyDetection에서 target의 위치정보를 받을 예정

    // Pathfinding
    public bool canFindPath = true;
    public float repathRate = 0.5f;

    // Movement
    public float maxSpeed = 4f;
    public bool canFly = true;
    public bool canJump = true;
    public float maxJumpWidth = 2f;
    public float jumpHeight = 4f;
    public float timeToReachMaxSpeed = 0.4f;
    public float pickNextWaypointDistance = 2f;
    public float slowDownDistance = 3f;
    public float endReachedDistance = 1.5f;
    /// <summary>
    /// 오브젝트가 사용할 중력 설정
    /// </summary>
    public GravityType gravitySetting = GravityType.Default;

    // Settings
    public bool alwaysDrawGizmos = false;
    public LayerMask obstacleLayer;

    #endregion

    #region [Private Fields not to be showed in inspector]

    private Path path;
    private Seeker seeker;
    private Rigidbody2D rigid;
    private Collider2D enemyCollider;
    private EnemyDetection detection;

    private int currentWaypoint = 0;
    private float timeSpentForMove = 0f;

    private float lastJumpTime = 0f;
    private float jumpDelayTime = 2f;

    private float minJumpHeight = 0.8f;

    #endregion

    #region [Custom Types]

    /// <summary>
    /// 오브젝트가 경로를 찾기 위한 상태
    /// </summary>
    public enum State
    {
        Default,
        SlowDown,
        EndReached,
        HasNoTarget
    }

    public enum GravityType
    {
        Default,
        UseRigidbodySettings
    }

    #endregion

    #region [Public Properties]

    /// <summary>
    /// 오브젝트의 현재 경로 찾기 상태
    /// </summary>
    public State PathfindingState { get; private set; } = State.HasNoTarget;

    /// <summary>
    /// 이동하려는 방향
    /// </summary>
    public Vector2 DesiredMoveDirection { get; private set; }

    public bool IsJumping { get; private set; }

    public float MaxSpeed { get => maxSpeed; set => maxSpeed = value; }

    public float JumpHeight { get => jumpHeight; set => jumpHeight = value; }

    public float MaxJumpWidth { get => maxJumpWidth; set => maxJumpWidth = value; }

    public float GravityScale { get => rigid.gravityScale; set => rigid.gravityScale = value; }

    #endregion

    #region [Private Properties]

    private float TimeSpentForMove
    {
        get => timeSpentForMove;
        set
        {
            timeSpentForMove = Mathf.Clamp(value, 0, timeToReachMaxSpeed - Time.fixedDeltaTime);
        }
    }

    //땅에 발이 닿았는가 체크 및 땅에 닿았을 시 true 반환
    private bool IsGrounded
    {
        get
        {
            Bounds bounds = enemyCollider.bounds;
            Vector2 footposition = new Vector2(bounds.center.x, bounds.min.y);
            bool isGrounded = Physics2D.OverlapCircle(footposition, 0.1f, obstacleLayer);
            if (isGrounded == true && rigid.velocity.y <= 0)
            {
                return true;
            }

            return false;
        }
    }

    // 앞에 밟을 수 있는 땅이 존재하는지 체크
    private bool IsExistingGroundInFront
    {
        get
        {
            float checkDistance = 1f;
            if (canJump)
                checkDistance = jumpHeight - 0.5f;

            Vector2 currentVelocity = rigid.velocity;
            Bounds bounds = enemyCollider.bounds;
            Vector2 front = new Vector2(bounds.center.x + 0.5f * Mathf.Sign(transform.localScale.x), bounds.min.y);
            var hitInfo = Physics2D.Raycast(front, Vector2.down, checkDistance, obstacleLayer);
            if (alwaysDrawGizmos)
                Debug.DrawRay(front, new Vector3(0f, -checkDistance), Color.green);

            if (!hitInfo.collider && currentVelocity.y <= 0f)
            {
                return false;
            }

            return true;
        }
    }

    #endregion

    #region [Unity Event Methods]

    private void Awake()
    {
        seeker = GetComponent<Seeker>();
        rigid = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<Collider2D>();
        detection = GetComponent<EnemyDetection>();
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(UpdatePath), 0f, repathRate);
    }

    private void Start()
    {
        var entity = GetComponent<Entity>();
        if (entity != null)
            entity.OnDeath += () => base.enabled = false;

        //InvokeRepeating(nameof(UpdateGraph), 0f, repathRate);
    }

    private void UpdateGraph()
    {
        var guo = new GraphUpdateObject(enemyCollider.bounds);
        AstarPath.active.UpdateGraphs(guo);
    }

    private void Update()
    {
        if (!canFindPath)
            return;

        ApplyGravityScale();
        ChangePathfindingState();
    }

    private void FixedUpdate()
    {
        if (!canFindPath || path == null || detection.Target == null)
            return;

        // IndexOutOfRangeException 방지
        if (currentWaypoint >= path.vectorPath.Count)
            return;

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rigid.position).normalized; // 현재 Waypoint를 향한 방향벡터
        //Vector2 currentVelocity = rigid.velocity;

        if (canFly)
        {
            Move(direction);
        }
        else
        {
            // 땅을 밟고 있을 때만 점프한다.
            if (canJump && IsGrounded && Time.time >= lastJumpTime + jumpDelayTime)
            {
                Jump(); // 추가 조건 처리는 내부에서 한다.
            }

            if (IsExistingGroundInFront)
                MoveX(direction.x);
            else if (!canJump)
                rigid.velocity = new Vector2(0f, rigid.velocity.y);

            IsJumping = !IsGrounded;
        }

        //Debug.Log($"<{nameof(EnemyPathfinder)}> 방향: {direction}, 속도: {rigid.velocity}", this);

        // 좌우반전
        if (Mathf.Abs(rigid.velocity.x) > 0.1f)
            transform.localScale = new Vector3(Mathf.Sign(rigid.velocity.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(Mathf.Sign(detection.Target.transform.position.x - rigid.position.x) * Mathf.Abs(transform.localScale.x),
                transform.localScale.y, transform.localScale.z);

        float distance = Vector2.Distance(rigid.position, path.vectorPath[currentWaypoint]);
        if (distance < pickNextWaypointDistance)
        {
            currentWaypoint++;
            TimeSpentForMove = 0f;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!alwaysDrawGizmos)
            return;

        Handles.color = Color.blue;
        Handles.DrawWireDisc(transform.position, transform.forward, slowDownDistance, 1f);

        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, transform.forward, endReachedDistance, 1f);
    }

    private void OnDrawGizmosSelected()
    {
        if (alwaysDrawGizmos)
            return;

        Handles.color = Color.blue;
        Handles.DrawWireDisc(transform.position, transform.forward, slowDownDistance, 1f);

        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, transform.forward, endReachedDistance, 1f);
    }
#endif


    #endregion

    #region [Private Methods]

    // 경로 갱신
    private void UpdatePath()
    {
        if (canFindPath && seeker.IsDone() && detection.Target != null)
            seeker.StartPath(rigid.position, detection.Target.transform.position, OnPathComplete);
    }

    // 경로 갱신 성공 시 발동되는 메서드
    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
            TimeSpentForMove = 0f;
        }
    }

    // 현재 패스파인딩 상태를 변경
    private void ChangePathfindingState()
    {
        if (detection.Target == null)
        {
            PathfindingState = State.HasNoTarget;
        }
        else if (Vector2.Distance(transform.position, detection.Target.transform.position) <= endReachedDistance)
        {
            PathfindingState = State.EndReached;
        }
        else if (Vector2.Distance(transform.position, detection.Target.transform.position) <= slowDownDistance)
        {
            PathfindingState = State.SlowDown;
        }
        else
        {
            PathfindingState = State.Default;
        }
    }

    // GravitySetting에 따라 Rigidbody2D의 Gravity Scale 값을 적용
    private void ApplyGravityScale()
    {
        switch (gravitySetting)
        {
            case GravityType.Default:
                if (canFly)
                    rigid.gravityScale = 0f;
                else
                    rigid.gravityScale = 2.5f;
                break;
            case GravityType.UseRigidbodySettings:
                if (!canFly && rigid.gravityScale < 0.1f)
                    rigid.gravityScale = 0.1f;
                break;
            default:
                break;
        }
    }

    #region [Move]

    // 가속 이동 처리
    private void Move(Vector2 direction)
    {
        Rigidbody2D rigid = this.rigid;

        direction = direction.normalized;
        //Vector2 v0 = rigid.velocity;
        //Vector2 v1 = (Time.fixedDeltaTime / timeToReachMaxSpeed * maxSpeed + v0.magnitude) * direction;
        float delta_v = maxSpeed / timeToReachMaxSpeed;

        Vector2 acceleration = delta_v * direction;

        // 가속도 방향벡터를 3배 크기로 표시
        if (alwaysDrawGizmos)
            Debug.DrawRay(rigid.position, 3f * acceleration.normalized, Color.yellow);

        MoveOnState(acceleration);

        // 최고속력 제한
        if (rigid.velocity.magnitude > maxSpeed)
        {
            rigid.velocity = maxSpeed * rigid.velocity.normalized;
        }
    }

    private void MoveX(float xDir)
    {
        Rigidbody2D rigid = this.rigid;

        if (xDir == 0)
            return;

        xDir = Mathf.Sign(xDir);
        //float v0 = rigid.velocity.x;
        //float v1 = (Time.fixedDeltaTime / timeToReachMaxSpeed * maxSpeed + Mathf.Abs(v0)) * xDir;

        float delta_v = maxSpeed / timeToReachMaxSpeed;

        Vector2 acceleration = new Vector2(delta_v * xDir, 0f);

        //Debug.Log($"<{nameof(EnemyPathfinder)}> 가속도: {acceleration}", this);

        MoveOnState(acceleration);

        // 최고속력 제한
        if (Mathf.Abs(rigid.velocity.x) > maxSpeed)
        {
            rigid.velocity = new Vector2(maxSpeed * Mathf.Sign(rigid.velocity.x), rigid.velocity.y);
        }
    }

    private void MoveOnState(Vector2 acceleration)
    {
        switch (PathfindingState)
        {
            case State.Default:
                rigid.AddForce(rigid.mass * acceleration);
                DesiredMoveDirection = acceleration.normalized;
                TimeSpentForMove += Time.fixedDeltaTime;
                break;
            case State.SlowDown:
                //float distance = Vector2.Distance(rigid.position, target.position);
                //Vector2 expectedPosition = Vector2.Lerp(rigid.position, target.position, Mathf.Clamp01((distance - endReachedDistance) / distance));
                //float smoothTime = Vector2.Dot(expectedPosition - rigid.position, 0.5f * rigid.velocity) / Mathf.Pow(rigid.velocity.magnitude, 2);
                //Vector2 temp = Vector2.zero;
                //rigid.position = Vector2.SmoothDamp(rigid.position, target.position, ref temp, smoothTime, 0f);

                rigid.velocity = Vector2.Lerp(rigid.velocity, Vector2.zero, 2 * Time.fixedDeltaTime);
                break;
            case State.EndReached:
                rigid.velocity = Vector2.zero;
                break;
            default:
                break;
        }
    }

    #endregion

    #region [Jump]
    private void Jump()
    {
        //Vector2 currentPosition = (Vector2)enemyCollider.bounds.min + Vector2.up;
        //Vector2 currentPosition = Vector2.Lerp(rigid.position, enemyCollider.bounds.min, 0.5f);
        Vector2 currentPosition = rigid.position;

        Vector2 displacementToNextWaypoint = (Vector2)path.vectorPath[currentWaypoint] - currentPosition;
        Vector2 displacementToTarget = (Vector2)detection.Target.transform.position - currentPosition;

        // 두 변위의 x축 방향이 다른 경우
        if (displacementToNextWaypoint.x * displacementToTarget.x <= 0f)
            return;

        bool isExistingGorundInFront = IsExistingGroundInFront;

        var frontInfo = Physics2D.Raycast(currentPosition, displacementToTarget.normalized, 1f, obstacleLayer);

        // 전방에 장애물이 없고 점프할 최소높이보다 작을 때 앞에 땅이 존재하는 경우
        //if (frontInfo.collider == null && displacementToNextWaypoint.y < minJumpHeight && isExistingGorundInFront)
        //{
        //    //rigid.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * rigid.velocity.x, rigid.velocity.y);
        //    return;
        //}

        // 점프를 위한 최소 x변위
        if (Mathf.Abs(displacementToNextWaypoint.x) < 0.3f)
            displacementToNextWaypoint.x = Mathf.Sign(displacementToNextWaypoint.x) * 0.3f;

        // 점프 시 변위 조절 해야함. 특히 x값
        // 조건1. 변위의 x성분의 최대값은 maxJumpWidth를 넘지 말아야함
        // 조건2. 기본 변위의 x성분으로만으로는 점프 시 한계가 보임. 최솟값을 설정하던가 적절한 보정값을 줘야함

        // 현재 위치로부터 최대 높이일 때의 상대적인 위치
        Vector2 relativeVertex = new Vector2(Mathf.Abs(displacementToNextWaypoint.x) < maxJumpWidth + 0.5f ?
            displacementToNextWaypoint.x : Mathf.Sign(displacementToNextWaypoint.x) * (maxJumpWidth + 0.5f), jumpHeight - 1f);

        // 플랫폼 사이의 점프 실행 시 천장과의 충돌로 인해 예상 점프 궤적을 벗어나는 사태 방지
        if (!isExistingGorundInFront)
        {
            Bounds bounds = enemyCollider.bounds;
            float interpolateY = bounds.center.y + bounds.size.y / 2f - currentPosition.y;
            var ceilingInfo = Physics2D.Linecast(currentPosition, currentPosition + relativeVertex + new Vector2(0f, interpolateY + 1f), obstacleLayer);
            if (alwaysDrawGizmos)
                Debug.DrawLine(currentPosition, currentPosition + relativeVertex + new Vector2(0f, interpolateY + 1f), Color.cyan, 0.5f);
            if (ceilingInfo.collider)
                return;
        }

        // 점프 가능 여부 조사를 위한 포물선의 끝점
        Vector2 end = new Vector2(currentPosition.x + relativeVertex.x * 2.5f, currentPosition.y - 1.5f * relativeVertex.y);
        if (PhysicsUtility.QuadraticFormula(currentPosition, currentPosition + relativeVertex, end.y, out var solutions))
        {
            if (solutions.Length == 2)
            {
                end.x = displacementToNextWaypoint.x >= 0 ? solutions[1] : solutions[0];
            }
        }

        // 점프 가능 여부 조사
        var groundInfo = PhysicsUtility.ParabolaLinecast(currentPosition, currentPosition + relativeVertex, end, obstacleLayer);
        if (alwaysDrawGizmos)
            PhysicsUtility.DrawParabolaLine(currentPosition, currentPosition + relativeVertex, end, Color.red);

        // 점프가 가능할 시 점프 실행
        if (groundInfo.collider && Vector2.Angle(Vector2.up, groundInfo.normal) <= 45)
        {
            //rigid.velocity = new Vector2(rigid.velocity.x / 2f, 0f); // 실제 힘을 가하기 전 속도를 줄임. 일종의 점프 준비 과정.
            lastJumpTime = Time.time;
            JumpForce(new Vector2(relativeVertex.x, jumpHeight));
        }
    }

    private void JumpForce(Vector2 maxHeightDisplacement)
    {
        Rigidbody2D rigid = this.rigid;

        // m*k*g*h = m*v^2/2 (단, k == gravityScale) <= 역학적 에너지 보존 법칙 적용
        float v_y = Mathf.Sqrt(2 * rigid.gravityScale * -Physics2D.gravity.y * maxHeightDisplacement.y);
        // 포물선 운동 법칙 적용
        float v_x = maxHeightDisplacement.x * v_y / (2 * maxHeightDisplacement.y);

        Vector2 force = rigid.mass * (new Vector2(v_x, v_y) - rigid.velocity);
        rigid.AddForce(force, ForceMode2D.Impulse);
    }

    #endregion

#if BETA
    // 가속 이동 처리
    private void Move(Vector2 direction)
    {
        Rigidbody2D rigid = this.rigid;

        Vector2 topVelocity = maxSpeed * direction.normalized;
        Vector2 acceleration = (topVelocity - rigid.velocity) / (timeToReachMaxSpeed - TimeSpentForMove);

        // 가속도 방향벡터를 3배 크기로 표시
        if (alwaysDrawGizmos)
            Debug.DrawRay(rigid.position, 3f * acceleration.normalized, Color.yellow);

        switch (PathfindingState)
        {
            case State.Default:
                rigid.AddForce(rigid.mass * acceleration);
                DesiredMoveDirection = acceleration.normalized;
                TimeSpentForMove += Time.fixedDeltaTime;
                break;
            case State.SlowDown:
                //float distance = Vector2.Distance(rigid.position, target.position);
                //Vector2 expectedPosition = Vector2.Lerp(rigid.position, target.position, Mathf.Clamp01((distance - endReachedDistance) / distance));
                //float smoothTime = Vector2.Dot(expectedPosition - rigid.position, 0.5f * rigid.velocity) / Mathf.Pow(rigid.velocity.magnitude, 2);
                //Vector2 temp = Vector2.zero;
                //rigid.position = Vector2.SmoothDamp(rigid.position, target.position, ref temp, smoothTime, 0f);

                rigid.velocity = Vector2.Lerp(rigid.velocity, Vector2.zero, 2 * Time.fixedDeltaTime);   
                break;
            case State.EndReached:
                rigid.velocity = Vector2.zero;
                break;
            default:
                break;
        }

        // 최고속력 제한
        if (rigid.velocity.magnitude > maxSpeed)
        {
            rigid.velocity = maxSpeed * rigid.velocity.normalized;
        }
    }
#endif

#if BETA
    private bool JumpDefault()
    {
        RaycastHit2D obstacleInfo = Physics2D.Raycast(rigid.position, new Vector2(detection.Target.transform.position.x - rigid.position.x, 0f), maxJumpWidth, obstacleLayer);

        // 장애물 감지 레이캐스트 표시
        if (alwaysDrawGizmos)
            Debug.DrawRay(rigid.position, new Vector2(detection.Target.transform.position.x, 0f).normalized * maxJumpWidth, Color.green);

        if (obstacleInfo.collider != null)
        {
            //Debug.Log($"[{typeof(EnemyPathfinder)}] 점프!");

            // Ground 감지를 위한 점프 궤적 예측
            Vector2 relativeVertex = new Vector2(obstacleInfo.point.x - rigid.position.x, jumpHeight - 1f);
            Vector2 end = new Vector2(rigid.position.x + relativeVertex.x * 2.5f, rigid.position.y - 1.5f * relativeVertex.y);
            if (PhysicsUtility.QuadraticFormula(rigid.position, rigid.position + relativeVertex, end.y, out var solutions))
            {
                if (solutions.Length == 2)
                {
                    end.x = relativeVertex.x >= 0 ? solutions[1] : solutions[0];
                }
            }

            // 예측된 점프 궤적을 바탕으로 Ground 감지
            RaycastHit2D groundInfo = PhysicsUtility.ParabolaLinecast(rigid.position, rigid.position + relativeVertex, end, obstacleLayer);

            if (alwaysDrawGizmos)
                PhysicsUtility.DrawParabolaLine(rigid.position, rigid.position + relativeVertex, end, Color.red, duration: 3f);

            // Ground가 착지 가능할 것으로 예측된다면 점프 실행
            if (groundInfo.collider != null && Vector2.Angle(Vector2.up, groundInfo.normal) <= 45)
            {
                JumpForce(new Vector2(relativeVertex.x, jumpHeight));

                return true;
            }

        }
        return false;
    }
#endif

    #endregion
}
