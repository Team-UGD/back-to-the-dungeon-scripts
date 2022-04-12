using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInput playerInput;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool resetDoubleJump = false;
    [SerializeField] private float basicSpeed = 4.0f;             //기본 이동 속도
    [SerializeField] private float staminaMoveSpeed = 3.0f;             //스테미나 이동 속도
    [SerializeField] private float speed = 4.0f;             //달리는 이동 속도
    [SerializeField] private float staminaDelayTime = 1.0f;             //스테미나 딜레이 시간
    [SerializeField] private bool staminaFunction = true;             //스테미나 기능 사용
    [SerializeField] private bool moveControl = true;             //움직임 기능 사용
    [SerializeField] private bool staminaUse = false;             //스테미나 사용하는가?
    [SerializeField] private float sDecreaseSpeed = 2f;
    [SerializeField] private float sIncreaseSpeed = 0.8f;
    [SerializeField] private RecordBoard recordBoard;
    [SerializeField] private int maxJumpCount = 2;
    [SerializeField] private int currentJumpCount = 0;
    [SerializeField] private bool jumpControl = true;
    [SerializeField] private bool DoJump = false;                //점프를 할까?
    [SerializeField] private bool DoLongJ;                       //높은 점프를 할까?

    [System.NonSerialized]
    private float jumpFoece = 8.0f;             //점프하는 힘
    private bool DoDownJ;                       //높은 점프를 할까?
    private float knockbackForce = 5.0f;        //넉백당하는 힘
    private Rigidbody2D rigid2D;
    private CapsuleCollider2D capsuleCollider2D;
    private Animator animator;
    private bool isGrounded;
    private bool sceneLoaded = false;
    private Hero hero;
    private bool speedChange = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigid2D = GetComponent<Rigidbody2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        playerInput = GetComponent<PlayerInput>();
        hero = GetComponent<Hero>();
    }

    private void Start()
    {

        hero.Stamina = hero.MaxStamina;
        GameManager.Instance.OnSceneLoaded += Instance_OnSceneLoaded;

    }

    private void Instance_OnSceneLoaded(UnityEngine.SceneManagement.Scene? previous, UnityEngine.SceneManagement.Scene? loaded, SceneLoadingTiming when)
    {
        switch (when)
        {
            case SceneLoadingTiming.BeforeFadeOut:
                rigid2D.constraints = RigidbodyConstraints2D.FreezeAll;
                sceneLoaded = true;
                break;
            case SceneLoadingTiming.AfterLoading:
                rigid2D.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
                break;
            case SceneLoadingTiming.AfterFadeIn:
                rigid2D.constraints = RigidbodyConstraints2D.FreezeRotation;
                sceneLoaded = false;
                break;
        }
    }

    private void Update()
    {
        //만약 basicSpeed가 달라져 speed의 기존 정보가 달라졌을 경우 speed를 바뀐 정보로 업데이트
        if (speedChange)
        {
            if (staminaFunction && staminaUse)
            {
                speed = basicSpeed + staminaMoveSpeed;
            }
            else
            {
                speed = basicSpeed;
            }
            speedChange = false;
        }

        if (jumpControl)
        {
            if (playerInput.LongJump == true && rigid2D.velocity.y > 0 && playerInput.YAxisDir != -1)
            {
                DoLongJ = true;
            }
            if (playerInput.Jump && playerInput.YAxisDir != -1)
            {
                DoJump = true;
            }
            //아래점프
            if (playerInput.YAxisDir == -1 && playerInput.Jump)
            {
                DoDownJ = true;
            }
        }
        if (resetDoubleJump)
        {
            currentJumpCount = maxJumpCount;
            resetDoubleJump = false;
        }

        // 리팩토링 된 스태미너 컨트롤 함수
        StaminaControl();

        #region === 기존 코드 ===

        //if (playerInput.Interact)
        //{
        //    interactUse = true;
        //    staminaUse = false;
        //    staminaDelay = true;
        //}
        ////(상호작용 키를 눌렀었고 그 이후에 ESC를 눌렀는가) 또는 (recordBoard가 비활성화된 경우)
        ////1.interactUse를 false로 바꾸기(상호작용 키 사용 X)
        ////2.staminaUse를 true로 바꾸기(스테미나 사용 O) 
        //if (interactUse && playerInput.Cancel || !recordBoard.IsUIActive)
        //{
        //    interactUse = false;
        //    staminaUse = true;
        //}

        //if (staminaDelay)
        //{
        //    StartCoroutine(Delay(staminaDelay));
        //}
        //if (staminaFunction && playerInput.Run && !staminaDelay)
        //{
        //    if (hero.Stamina > 0.1f)
        //    {
        //        if (speed == basicSpeed)
        //        {
        //            speed += staminaMoveSpeed;
        //        }
        //        hero.Stamina -= sDecreaseSpeed * Time.deltaTime;
        //    }
        //    else
        //    {
        //        if (speed == basicSpeed + staminaMoveSpeed)
        //        {
        //            speed -= staminaMoveSpeed;
        //            staminaDelay = true;
        //        }
        //    }
        //}
        //else if (staminaFunction && !playerInput.Run)
        //{
        //    if (speed == basicSpeed + staminaMoveSpeed)
        //    {
        //        speed -= staminaMoveSpeed;
        //        staminaDelay = true;
        //    }
        //    if (!staminaDelay)
        //    {
        //        hero.Stamina += sIncreaseSpeed * Time.deltaTime;
        //    }
        //}

        #endregion
    }

    private float lastStaminaUseTime = 0f;

    private void StaminaControl()
    {
        // 달리기 키를 눌렀을 때
        if (this.staminaFunction && playerInput.Run)
        {
            // 스태미너 사용 가능할 때
            if (hero.Stamina > 0f)
            {
                // 스피드 증가
                if (speed == basicSpeed)
                    speed += staminaMoveSpeed;

                hero.Stamina -= sDecreaseSpeed * Time.deltaTime;
                lastStaminaUseTime = Time.time; // 마지막으로 스태미너를 사용했던 시점 기록
            }
            // 스태미너 사용 불가능할 때 스피드 복구
            else
            {
                if (speed == basicSpeed + staminaMoveSpeed)
                    speed -= staminaMoveSpeed;
            }
        }
        // 달리기 키를 누르지 않았을 때
        else
        {
            // 스피드 복구
            if (speed == basicSpeed + staminaMoveSpeed)
                speed -= staminaMoveSpeed;

            // 스태미너를 마지막으로 사용했던 시점으로부터 딜레이 초과 시 스태미너 증가 시작
            if (Time.time > lastStaminaUseTime + staminaDelayTime)
                hero.Stamina += sIncreaseSpeed * Time.deltaTime;
        }
    }
    #region 기존 코드
    //private IEnumerator Delay(bool isDelay)
    //{
    //    float startTime = Time.time;
    //    float curTime = startTime;
    //    if (isDelay)
    //    {
    //        while (true)
    //        {
    //            curTime = Time.time;
    //            if (curTime - startTime >= staminaDelayTime)
    //            {
    //                staminaDelay = false;
    //                yield break;
    //            }
    //            yield return null;
    //        }
    //    }
    //    yield return null;
    //}
    #endregion
    private void FixedUpdate()
    {
        if (!DoJump)
        {
            animator.SetBool("isJump", false);
        }
        //긴 점프
        if (DoLongJ)
        {
            rigid2D.gravityScale = 1.5f;
        }
        else
        {
            rigid2D.gravityScale = 2.5f;
        }
        DoLongJ = false;      //DoLongJ 초기화

        if (DoJump && playerInput.YAxisDir != -1)
        {
            Jump();
        }
        //움직임
        if (moveControl)
        {
            Move(playerInput.XAxisDir);
        }


        #region 기존 코드

        ////마우스 포인터가 향하는 방향으로 flip해준다.
        //if (playerInput.MousePosition.x > gameObject.transform.position.x)        //마우스 포인터가 플레이어보다 오른쪽일 경우
        //{
        //    float Dir_x = Mathf.Abs(transform.localScale.x);
        //    float Dir_y = transform.localScale.y;
        //    float Dir_z = transform.localScale.z;
        //    transform.localScale = new Vector3(Dir_x, Dir_y, Dir_z);
        //}
        //else if (playerInput.MousePosition.x < gameObject.transform.position.x)   //마우스 포인터가 플레이어보다 왼쪽일 경우
        //{
        //    float Dir_x = transform.localScale.x;
        //    if (Dir_x > 0)
        //    {
        //        Dir_x *= -1;
        //    }
        //    float Dir_y = transform.localScale.y;
        //    float Dir_z = transform.localScale.z;
        //    transform.localScale = new Vector3(Dir_x, Dir_y, Dir_z);
        //}

        #endregion

        //땅에 발이 닿았는가 체크 및 땅에 닿았을 시 점프 가능횟수 초기화
        Bounds bounds = capsuleCollider2D.bounds;
        Vector2 footposition = new Vector2(bounds.center.x, bounds.min.y);
        isGrounded = Physics2D.OverlapCircle(footposition, 0.1f, groundLayer);
        if (isGrounded == true && rigid2D.velocity.y <= 0)
        {
            currentJumpCount = 0;
        }

        //아래점프
        if (DoDownJ)
        {
            Debug.Log("아래점프");
        }
    }

    /// <summary>
    /// double Jump Reset
    /// Reset - True
    /// No Reset - False
    /// </summary>
    public bool ChangeDoubleJump
    {
        get
        {
            return resetDoubleJump;
        }
        set
        {
            resetDoubleJump = value;
        }
    }
    /// <summary>
    /// Check Double Jump
    /// Can you do Double Jump = isGround
    /// </summary>
    public bool CheckDoubleJump
    {
        get
        {
            return isGrounded;
        }
    }
    [System.Obsolete]
    public float ChangeSpeed
    {
        get
        {
            return speed;
        }
        set
        {
            speed = value;
        }
    }
    /// <summary>
    /// 기본 스피드를 바꿀 수 있는 프로퍼티
    /// </summary>
    public float ChangeBasicSpeed
    {
        get
        {
            return basicSpeed;
        }
        set
        {
            speedChange = true;
            basicSpeed = Mathf.Max(value, 0);
        }
    }
    /// <summary>
    /// MaxJumpCount 설정할 수 있는 프로퍼티
    /// </summary>
    public int SettingMaxJumpCount
    {
        get
        {
            return maxJumpCount;
        }
        set
        {
            Debug.Log($"Jump Count: {value}");
            maxJumpCount = value;
        }
    }
    /// <summary>
    /// 현재 움직임 제어상태를 볼 수 있고,
    /// 움직일 수 없도록 제어할 수 있는 프로퍼티
    /// </summary>
    public bool MoveControl
    {
        get
        {
            return moveControl;
        }
        set
        {
            moveControl = value;
        }
    }
    /// <summary>
    /// 현재 움직임 상태(Run을 할 수 있을 지)를 볼 수 있고,
    /// Run을 할 수 없도록 제어할 수 있는 프로퍼티
    /// </summary>
    public bool RunControl
    {
        get
        {
            return staminaFunction;
        }
        set
        {
            staminaFunction = value;
        }
    }
    /// <summary>
    /// 외부에서 점프를 On/Off할 수 있는 프로퍼티
    /// </summary>
    public bool JumpControl
    {
        get
        {
            return jumpControl;
        }
        set
        {
            jumpControl = value;
        }
    }
    /// <summary>
    /// 땅에 닿아 있는 지 검사
    /// </summary>
    public bool Grounded
    {
        get
        {
            return isGrounded;
        }
    }
    /// <summary>
    /// 점프 했는 지 검사
    /// </summary>
    public bool IsJump
    {
        get { return DoJump; }
    }
    public void Move(float x)
    {
        //바라보는 방향으로 움직이기
        //rigid2D.velocity = new Vector3(x * moveSpeed, rigid2D.velocity.y, 0);
        rigid2D.AddForce(new Vector2(x * 100, 0), ForceMode2D.Force);
        if (Mathf.Abs(rigid2D.velocity.x) > speed)
        {
            rigid2D.velocity = new Vector2(speed * Mathf.Sign(rigid2D.velocity.x), rigid2D.velocity.y);
        }

        if (x == 0)
        {
            rigid2D.velocity = new Vector2(0, rigid2D.velocity.y);
        }

        if (Mathf.Abs(rigid2D.velocity.x) > 0.1f)
        {
            animator.SetBool("isRun", true);
        }
        else
        {
            animator.SetBool("isRun", false);
        }
    }
    private void Jump()
    {
        if (!sceneLoaded)
        {
            animator.SetBool("isJump", true);
            if (currentJumpCount < maxJumpCount)
            {
                rigid2D.velocity = Vector2.up * jumpFoece;
                currentJumpCount++;
            }
        }
        DoJump = false;       //DoJump 초기화
    }
    /// <summary>
    /// 받은 방향으로 튕겨져 나간다.
    /// </summary>
    public void KnockBack(Vector2 vector2, float power, float use = 1)
    {
        Vector2 Dir = vector2.normalized;
        rigid2D.velocity = new Vector2(0f, 0f);
        if (use == 1)
        {
            rigid2D.AddForce(Dir * knockbackForce * power, ForceMode2D.Impulse);
        }
        else if (use == 0)
        {
            rigid2D.AddForce(Vector2.up * knockbackForce * power, ForceMode2D.Impulse);
        }
    }
    /// <summary>
    /// rigid body의 constraints 설정 메서드 (1 = all, 2 = z, 3 = x)
    /// </summary>
    /// <param name="n"></param>
    public void SetConstraints(int n)
    {
        if (n == 1)
            rigid2D.constraints = RigidbodyConstraints2D.FreezeAll;
        else if (n == 2)
            rigid2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        else if (n == 3)
            rigid2D.constraints = RigidbodyConstraints2D.FreezePositionX;

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //아래점프 / 위점프
        if (DoDownJ && collision.collider.name == "ThinGround")
        {
            animator.SetBool("isJump", true);
            //기존코드
            //collision.collider.GetComponent<DownPlatform>().ChangeLayer();

            // 1/13일 변경한 코드
            collision.collider.GetComponent<DownPlatform>().DiableCollider();

            Debug.Log("아래점프2");
            DoDownJ = false;
        }
        else if (collision.collider.name == "ThinGround" && Vector2.Angle(collision.GetContact(0).normal, Vector2.up) >= 150f)
        {
            animator.SetBool("isJump", true);
            //기존 코드
            //collision.collider.GetComponent<DownPlatform>().ChangeLayer();

            // 1/13일 변경한 코드
            collision.collider.GetComponent<DownPlatform>().DiableCollider();
            rigid2D.velocity = Vector2.up * 4;

            Debug.Log("아래점프3");
        }
        else if (rigid2D.velocity.y <= 0)
        {
            animator.SetBool("isJump", false);
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        //아래점프 / 위점프
        if (DoDownJ && collision.collider.name == "ThinGround")
        {
            animator.SetBool("isJump", true);
            //기존 코드
            //collision.collider.GetComponent<DownPlatform>().ChangeLayer();

            // 1/13일 변경한 코드
            collision.collider.GetComponent<DownPlatform>().DiableCollider();
            DoDownJ = false;
        }
        else if (collision.collider.name == "ThinGround" && Vector2.Angle(collision.GetContact(0).normal, Vector2.up) >= 150f)
        {
            animator.SetBool("isJump", true);
            //기존 코드
            //collision.collider.GetComponent<DownPlatform>().ChangeLayer();

            // 1/13일 변경한 코드
            collision.collider.GetComponent<DownPlatform>().DiableCollider();
        }
        else if (rigid2D.velocity.y <= 0)
        {
            animator.SetBool("isJump", false);
        }
    }
}