using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class Weapon : MonoBehaviour
{

    [Header("WeaponInfo")]
    [SerializeField] protected int damage;
    [SerializeField] [Space(10f)] protected float attack_speed;
    [SerializeField] protected float recoil;
    [SerializeField] protected float reload_time;
    [SerializeField] protected float swap_time;
    [SerializeField] protected float max_distance;
    [SerializeField] [Space(10f)] protected int max_Bullet;
    [SerializeField] protected int cur_Bullet;
    protected SpriteRenderer sprite;

    [Header("WeaponSource")]
    [SerializeField] protected AudioClip fireSound;
    [SerializeField] protected AudioClip reloadSound;
    protected AudioSource audioSource;
    public GameObject Fireposition;


    protected ParticleSystem muzzleFlash;
    private float lastAttackTime;
    private bool isFire;
    private bool isReload;

    /// <summary>
    /// 발사 체크 함수
    /// 마우스를 클릭하고 클릭해제하기 전까지 false가 됨
    /// </summary>
    [SerializeField] public bool onFire = true;
    protected Transform Transform;

    public float Swap_time
    {
        get { return swap_time; }
    }

    /// <summary>
    /// PlayerInput의 Fire의 값을 반환하는 프로퍼티
    /// </summary>
    public bool IsFire
    {
        get { return isFire; }
        set { isFire = value; }
    }

    public float Reload_time
    {
        get { return reload_time; }
        set { reload_time = value; }
    }
    public bool IsReload { set; get; }

    public State WeaponState { get; protected set; }

    public enum State
    {
        ReadyToFire,
        OnFire,
        Empty,
        Reloading
    }

    protected virtual void OnEnable()
    {
        WeaponState = State.ReadyToFire;
    }

    //반동
    // Start is called before the first frame update
    protected virtual void  Start()
    {
        Transform = GetComponent<Transform>();
        muzzleFlash = transform.Find("Fire Position").GetChild(0).GetComponent<ParticleSystem>();
        sprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        cur_Bullet = max_Bullet;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //총알수를 보여주는 UI 설정
        UIManager.Instance.SetBulletUI(cur_Bullet, max_Bullet);

        if (cur_Bullet == 0)
            Reload();
    }

    /// <summary>
    /// 발사 함수 각 무기의 특성에 맞게 작성하면 됨
    /// instantiate를 사용하여 총알을 소환하는 식으로 구현
    /// 예시 코드는 Pistol.cs 참고
    /// </summary>
    public abstract void Fire();

    protected void SetWeaponState()
    {
        WeaponState = State.OnFire;

        CancelInvoke(nameof(SetWeaponStateToReady));
        Invoke("SetWeaponStateToReady",attack_speed);
    }

    void SetWeaponStateToReady()
    {

        if (WeaponState == State.Reloading)
            return;
        WeaponState = State.ReadyToFire;
    }


    //현재 사용되지 않는 메서드
    public virtual void TempFire()
    {
        if (WeaponState == State.ReadyToFire)
        {
            WeaponState = State.OnFire;
        }
        else if (WeaponState == State.OnFire && Time.time >= lastAttackTime + attack_speed)
        {
            WeaponState = State.ReadyToFire;
            lastAttackTime = Time.time;
        }
    }


    /// <summary>
    /// onFire 변수 상태 변환 함수
    /// 클릭 감지용
    /// 클릭을 하고 있으면 False를 반환
    /// 클릭을 하지 않고 있으면 True를 반환함
    /// </summary>
    public void ChangeState()
    {
        onFire = !GetMouseInput();
    }

    /// <summary>
    /// 마우스 인풋 리턴 메소드
    /// </summary>
    /// <returns>마우스 인풋</returns>
    public bool GetMouseInput()
    {
        return IsFire;
    }

    /// <summary>
    /// 장전함수
    /// 각 무기마다 설절된 장전시칸 이후에
    /// 장전이 완료되게끔 만들어져있음
    /// </summary>
    public virtual void Reload()
    {
        if (WeaponState == State.Reloading || cur_Bullet >= max_Bullet)
            return;

        IsReload = true;
    
        if(audioSource)
            audioSource.PlayOneShot(reloadSound);

        SetEmptyMagWeaponSprite();
        StartCoroutine(IEReload());
    }
    protected IEnumerator IEReload()
    {
        WeaponState = State.Reloading;

        yield return new WaitForSeconds(reload_time);

        SetLoadedWeaponSprite();
        IsReload = false;
        cur_Bullet = max_Bullet;

        WeaponState = State.ReadyToFire;
    }

    protected abstract void SetLoadedWeaponSprite();

    protected abstract void SetEmptyMagWeaponSprite();

    protected void SetWeaponSprite(string path)
    {
        sprite.sprite = Resources.Load<Sprite>(path);
    }

    /// <summary>
    /// 마우스 클릭 중일 때 onFire를 다시 True로 만들어주는 함수
    /// 연사무기 구현 때 Invoke와 함께 사용하면 유용
    /// </summary>
    private void Returntoture()
    {
        onFire = true;
    }


    public Vector2 Difference()
    {
        return transform.position - Fireposition.transform.position;
    }
}
