using UnityEngine;

using UnityEngine.UI;

public partial class Hero : Entity, ISaveable
{
    private PlayerMovement playerMovement; // Player Movement Component
    private PlayerShooter playerShooter; // Player Shooter Component
    Animator animator;
    GameObject skeletal;
    GameObject foot;
    private float damage = 1;
    
    [SerializeField]private float stamina;
    [SerializeField]private float maxStamina;
    [SerializeField] private bool loadPlayerData = true;
    //[SerializeField] private int mainVillage;

    [Header("ReloadGauge")]
    [SerializeField] private Slider reloadGauge;

    [SerializeField]
    private GameObject RespawnEffect;
    [SerializeField]
    private GameObject InvincibleEffect;

    [SerializeField]
    private bool resurrectionChance;

    [Header("Remained Life Setting")]
    [SerializeField] private int easyLevelMaxRemainedLife;
    [SerializeField] private int hardLevelMaxRemainedLife;

    public float Stamina
    {
        get 
        { 
            return stamina;
        }
        set 
        { 
            stamina = Mathf.Clamp(value,0,maxStamina);

            UIManager.Instance.SetStaminaUI(stamina,maxStamina);
        }
    }
    public float MaxStamina
    {
        get
        {
            return maxStamina;
        }
        set
        {
            maxStamina = Mathf.Max(0,value);
            Stamina = maxStamina;
        }
    }

    public bool ResurrectionChance
    {
        get { return resurrectionChance; }
        set
        {
            resurrectionChance = value;
            UIManager.Instance.SetResurrectionImage(value);
        }
    }

    public Slider ReloadGauge
    {
        get { return reloadGauge; }
    }

    public float Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    /// <summary>
    /// Hero에서 재정의한 Health프로퍼티 
    /// </summary>
    public override float Health
    {
        get => base.Health;
        protected set
        {
            base.Health = value;

            UIManager.Instance.SetHealthUI(Health, MaxHealth);
            Debug.Log($"<{nameof(Hero)}> 체력: {value}", this);
        }
    }

    

    public int MaxRemainedLife
    {
        get
        {
            switch (GameManager.Instance.Level)
            {
                case GameLevel.Easy:
                    return easyLevelMaxRemainedLife;
                case GameLevel.Hard:
                    return hardLevelMaxRemainedLife;
                default:
                    throw new System.InvalidOperationException("존재하지 않는 난이도");
            }
        }
    }

    private int remainedLife;
    public int RemainedLife
    {
        get => remainedLife;
        set
        {
            remainedLife = Mathf.Max(0, value);
            Debug.Log($"<{nameof(Hero)}> 남은 생명 횟수: {remainedLife}", this);
        }
    }

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerShooter = GetComponent<PlayerShooter>();
        animator = GetComponent<Animator>();
        skeletal = transform.Find("Skeletal").gameObject;
        foot = transform.Find("Player Foot Collider").gameObject;

        // 기타 컴포넌트 참조가 필요한 코드 추가
        OnDeath += () => {
            playerMovement.SetConstraints(1);
            playerShooter.enabled = false;
            animator.SetTrigger("Die");
            UIManager.Instance.SetActiveRestartButton();
        };

        SaveManager.Add(this, SaveKey.GameData);
    }

    //OnSceneLoaded이벤트 등록
    private void Start()
    {
        GameManager.Instance.OnSceneLoaded += SceneLoaded;

        //UIManager.Instance.OnClickRestartButton += () =>
        //{
        //    Invoke(nameof(SetPlayerActive), 1.5f);
        //};

        if (resurrectionChance)
            UIManager.Instance.SetResurrectionImage(true);
    }

    private void SceneLoaded(UnityEngine.SceneManagement.Scene? previous, UnityEngine.SceneManagement.Scene? loaded, SceneLoadingTiming when)
    {
        switch(when)
        {
            case SceneLoadingTiming.BeforeFadeOut:
                this.ChangeLayer(8);
                break;
            case SceneLoadingTiming.AfterLoading:
                //this.Health = loaded.Value.name == "Main Village" ? this.MaxHealth : this.Health;
                if (this.RemainedLife == 0)
                    this.RemainedLife = this.MaxRemainedLife;
                if (IsDead)
                {
                    IsDead = false;
                    SetPlayerActive();
                }
                UIManager.Instance.SetRemainLife(RemainedLife);
                break;
            case SceneLoadingTiming.AfterFadeIn:
                UIManager.Instance.SetPlayerUI(true);
                if(!InvincibleEffect.activeSelf)
                    this.ChangeLayer(9);
                break;
        }
    }


    //플래이어가 데미지를 입고 넉백당함
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            float dir = transform.position.x - collision.transform.position.x;

            //임시로 구현해둔 position
            Vector2 hitPosition = new Vector2(dir, 1);       //피격위치
            Vector2 hitNormal = collision.transform.position - transform.position + new Vector3(0, 0.75f);      //피격당한 표면의 법선벡터

            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
                damage = enemy.STR;
            else
                damage = 0;

            //TakeDamage(damage, hitPosition, hitNormal,true);            //iscontinuous를 true로 해서 squirrel이 데미지를 2번 입히거나 str값 조정
            TakeDamage(damage, hitPosition, hitNormal);
        }
    }

    protected override void OnEnable()
    {
        // 플레이어 데이터 로드 실패 시
        //base.OnEnable();

        UIManager.Instance.SetHealthUI(Health, MaxHealth);

        // 기타 활성화해야되는 컴포넌트 코드 추가
        playerMovement.enabled = true;
        animator.SetTrigger("Idle");
    }


    private void OnDestroy()
    {
        SaveManager.Remove(this, SaveKey.GameData);
    }

    public override void Heal(float healthToHeal)
    {
        base.Heal(healthToHeal);

        // 애니메이션 및 사운드 처리
    }

    protected override void Die()
    {
        ChangeLayer(8);

        if (resurrectionChance)
        {
            playerMovement.SetConstraints(1);
            playerShooter.enabled = false;
            animator.SetTrigger("Die");

            Instantiate(RespawnEffect, this.transform.position + new Vector3(0,1,0),Quaternion.identity);
            Invoke(nameof(SetPlayerActiveByItem),2f);
            return;
        }

        IsDead = true;
        this.RemainedLife -= 1;

        base.Die();

        if (this.RemainedLife == 0)
            this.RemainedLife = this.MaxRemainedLife;

        UIManager.Instance.SetPlayerUI(false);
        UIManager.Instance.SetRemainLife(RemainedLife);
    }



    public override void TakeDamage(float damage, Vector2 hitPosition, Vector2 hitNormal, bool isContinuous = false, float power = 1, float use = 1)
    {
        if (gameObject.layer == 8 || damage == 0)
            return;

        playerMovement.KnockBack(-hitNormal, power);
        base.TakeDamage(damage, hitPosition, hitNormal);

        if (IsDead)
            return;

        OnDamaged();

        Invoke(nameof(OffDamaged), 2f);
        Invoke(nameof(EnableMovement), 0.6f);

    }

    [System.Obsolete]
    private bool TryLoadPlayerData()
    {
        try
        {
            var data = GameManager.Instance.GameDataLoaded.Player;
            this.MaxHealth = data.maxHelath;
            this.Health = data.health;
            this.MaxStamina = data.maxStamina;
            this.resurrectionChance = data.resurrectionChance;

            Debug.Log($"<{nameof(Hero)}> {data}", this);
        }
        catch (System.NullReferenceException)
        {
            return false;
        }

        return true;
    }

    private void SetPlayerActiveByItem()
    {
        EnableMovement();
        Heal(MaxHealth);
        playerMovement.SetConstraints(2);
        playerShooter.enabled = true;
        SetPlayerInvincibleMode(5);
        animator.SetTrigger("Idle");
        resurrectionChance = false;
        UIManager.Instance.SetResurrectionImage(false);
    }

    public void SetPlayerActive()
    {
        Heal(MaxHealth);
        EnableMovement();
        playerMovement.SetConstraints(2);
        playerShooter.enabled = true;
        //ChangeLayer(9);
        animator.SetTrigger("Idle");
    }

    void EnableMovement()
    {
        if (!IsDead)
            playerMovement.enabled = true;
    }

    void DisableMovement() { playerMovement.enabled = false; }

    void OffDamaged()
    {
        if (!IsDead)
        {
            //ChangeColor(255, 255, 255, 255);
            //ChangeLayer(9);
            EnableMovement();
            animator.SetTrigger("Idle");
        }
    }

    void OnDamaged()
    {
        //ChangeLayer(8);
        //ChangeColor(77, 77, 77, 77);

        DisableMovement();
        animator.SetTrigger("Hurt");
    }

    private void ChangeLayer(int n)
    {
        gameObject.layer = n;
        skeletal.layer = n;
        foot.layer = n;
    }

    void ChangeColor(byte n1, byte n2, byte n3, byte n4)
    {
        SpriteRenderer[] sprites = skeletal.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer s in sprites)
        {
            if (s.name == skeletal.name)
                continue;
            else if (s == null)
                continue;
            s.color = new Color32(n1, n2, n3, n4);
        }
    }

    public void SetPlayerInvincibleMode(float time)
    {
        ChangeColor(240,170,170,150);
        InvincibleEffect.SetActive(true);

        CancelInvoke(nameof(SetPlayerNomalMode));
        Invoke(nameof(SetPlayerNomalMode), time);
        ChangeLayer(8);
        
    }

    private void SetPlayerNomalMode()
    {
        ChangeLayer(9);
        ChangeColor(255, 255, 255, 255);
        InvincibleEffect.SetActive(false);

    }

    
}

#region === Save System ===

public partial class Hero
{
    # region === ISaveable Interface Members ===

    string ISaveable.ID => null;

    object ISaveable.Save() => new PlayerData(this, this.IsDead ? this.MaxHealth : this.Health);

    void ISaveable.Load(object loaded)
    {
        if (!loadPlayerData)
            return;

        if (loaded is PlayerData data)
        {
            this.MaxHealth = data.maxHelath;
            this.Health = data.health;
            this.MaxStamina = data.maxStamina;
            this.ResurrectionChance = data.resurrectionChance;
            this.RemainedLife = data.remainedLife;

            Debug.Log($"<{nameof(Hero)}> {data}", this);
        }
        else
        {
            Debug.LogError($"<{nameof(Hero)}> 잘못된 데이터가 로드되었습니다. 필요한 데이터 타입: {typeof(PlayerData)}. 로드된 데이터 타입: {loaded?.GetType() ?? null}.", this);
        }
    }

    #endregion
}

[System.Serializable]
public class PlayerData
{
    public readonly float health;
    public readonly float maxHelath;
    public readonly float maxStamina;
    public readonly bool resurrectionChance;
    public readonly int remainedLife;

    public PlayerData(Hero player) : this(player, player.Health) { }

    public PlayerData(Hero player, float health)
    {
        this.health = health;
        this.maxHelath = player.MaxHealth;
        this.maxStamina = player.MaxStamina;
        this.resurrectionChance = player.ResurrectionChance;
        this.remainedLife = player.RemainedLife;
    }

    public override string ToString()
    {
        return $"Player Data {{ Health = {health}, Max Health = {maxHelath}, Max Stamina = {maxStamina}, Resurrection Item = {resurrectionChance} }}";
    }
}

#endregion
