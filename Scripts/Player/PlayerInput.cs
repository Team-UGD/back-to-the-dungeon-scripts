using UnityEngine;

/// <summary>
/// 플레이어 오브젝트를 위한 사용자 입력 관리 클래스
/// </summary>
public class PlayerInput : MonoBehaviour
{
    public const string XAxisName = "Horizontal";
    public const string YAxisName = "Vertical";
    public const string JumpButtonName = "Jump";
    public const string FireButtonName = "Fire1";
    public const string ReloadButtonName = "Reload";
    public const string InteractButtonName = "Interact";
    public const string CancelButtonName = "Cancel";
    public const string RunButtonName = "Run";

    private float xAxisDir;
    private float yAxisDir;
    private bool jump;
    private bool longJump;
    private bool fire;
    private bool reload;
    private bool interact;
    private bool run;
    private bool cancel;

    /// <summary>
    /// 입력한 키의 X축 방향 정보를 반환한다.<br/>
    /// 왼쪽: -1, 정지: 0, 오른쪽: +1
    /// </summary>
    public float XAxisDir { get => enabled ? xAxisDir : default; }

    /// <summary>
    /// 입력한 키의 y축 방향 정보를 반환한다.<br/>
    /// 아래: -1, 정지: 0, 위: +1
    /// </summary>
    public float YAxisDir { get => enabled ? yAxisDir : default; }

    /// <summary>
    /// 점프 키를 누르면 true를 반환한다.
    /// </summary>
    public bool Jump { get => enabled ? jump : default; }

    /// <summary>
    /// 점프 키를 누르고 있으면 true를 반환한다.
    /// 점프 키를 눌렀다가 떼었으면 false를 반환한다.
    /// </summary>
    public bool LongJump { get => enabled ? longJump : default; }

    /// <summary>
    /// 발사키를 누르면 true를 반환한다.
    /// </summary>
    public bool Fire { get => enabled ? fire : default; }

    /// <summary>
    /// 장전키를 누르면 true를 반환한다.
    /// </summary>
    public bool Reload { get => enabled ? reload : default; }

    public bool Interact { get => enabled ? interact : default; }

    public bool Run { get => enabled ? run : default; }

    public bool Cancel { get => enabled ? cancel : default; }

    /// <summary>
    /// 현재 마우스 월드 좌표를 반환한다.
    /// </summary>
    public Vector2 MousePosition
    {
        get
        {
            Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Debug.DrawRay(transform.position, mouseWorldPosition - (Vector2)transform.position, Color.red);

            return mouseWorldPosition;
        }
    }

    private void Update()
    {
        xAxisDir = Input.GetAxisRaw(XAxisName);
        yAxisDir = Input.GetAxisRaw(YAxisName);
        jump = Input.GetButtonDown(JumpButtonName);
        fire = Input.GetButton(FireButtonName) || Input.GetAxisRaw("right trigger") == 1 ? true : false;
        reload = Input.GetButtonDown(ReloadButtonName);
        interact = Input.GetButton(InteractButtonName);
        run = Input.GetButton(RunButtonName);
        cancel = Input.GetButtonDown(CancelButtonName);

        if (Input.GetButton(JumpButtonName))
        {
            longJump = true;
        }
        else if (Input.GetButtonUp(JumpButtonName))
        {
            longJump = false;
        }
    }
}
