using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;
using UnityEngine.UI;
using System.Linq;


public partial class PlayerShooter : MonoBehaviour, IEnumerable<Weapon>, ISaveable
{
    #region === Serialized Fields ===

    [SerializeField] private WeaponChangeInfo playerWeaponChangeInfo;
    [SerializeField, Tooltip("시작 무기를 설정하고 싶다면 프리팹을 할당하세요.")] private Weapon weapon; // current used weapon
    [SerializeField, Tooltip("저장된 무기 정보로 무기 슬롯을 구성하려면 체크하세요.")] private bool loadWeaponData;
    [SerializeField, Range(1, 5)] private byte weaponSlotCapacity = 2;
    [SerializeField] private Transform gunPivot;
    [SerializeField] private CCDSolver2D leftArmCCDSolver;
    [SerializeField] private CCDSolver2D rightArmCCDSolver;
    [SerializeField] private float weaponSwapDelay = 1f; // 나중에 없어질 예정

    #endregion

    #region === Public Properties ===

    /// <summary>
    /// 현재 사용 중인 무기의 보호 수준을 높이기 위해 타입 정보만 반환함.
    /// </summary>
    public Type CurrentWeapon { get => weapon.GetType(); }

    /// <summary>
    /// 무기 슬롯의 개수
    /// </summary>
    public byte WeaponSlotCapacity
    {
        get => weaponSlotCapacity;
        set
        {
            if (value > 5 || value < 1)
                throw new ArgumentException($"WeaponSlotCount의 값은 1이상 5이하의 값이어야합니다. 입력된 값: {value}");

            weaponSlotCapacity = value;
            if (weaponSlot.Count > weaponSlotCapacity)
            {
                weaponSlot.GetRange(weaponSlotCapacity, weaponSlot.Count - weaponSlotCapacity).ForEach(w => Destroy(w.gameObject));
                weaponSlot.RemoveRange(weaponSlotCapacity, weaponSlot.Count - weaponSlotCapacity);
                if (CurrentWeaponSlotNumber >= weaponSlotCapacity)
                {
                    SwapWeapon(0);
                }

                UIManager.Instance.SetWeaponSlotUI(this, weaponSlot);
            }
        }
    }

    public byte WeaponCount
    {
        get => (byte)weaponSlot.Count;
    }

    /// <summary>
    /// 현재 사용 중인 무기의 슬롯 인덱스
    /// </summary>
    public byte CurrentWeaponSlotNumber { get; private set; }

    /// <summary>
    /// 무기 스왑 가능 여부 설정
    /// </summary>
    public bool CanSwap { get; set; } = true;

    /// <summary>
    /// 발사 가능 여부
    /// </summary>
    public bool CanFire { get; set; } = true;

    /// <summary>
    /// 조준 가능 여부
    /// </summary>
    public bool CanAim { get; set; } = true;


    #endregion

    #region === Public Methods ===

    [Obsolete]
    public bool TryLoadWeapon()
    {
        try
        {
            PlayerWeaponData playerWeaponData = GameManager.Instance.GameDataLoaded.PlayerWeapon;

            if (playerWeaponData == null && playerWeaponData.weaponSlotCapacity < 1)
                return false;

            this.weapon = null;

            Debug.Log($"<{nameof(PlayerShooter)}> {playerWeaponData}", this);

            this.WeaponSlotCapacity = playerWeaponData.weaponSlotCapacity;

            var weaponTypes = playerWeaponData.WeaponTypes;
            var weaponPrefabs = playerWeaponChangeInfo.GetWeaponPrefabs(weaponTypes);

            byte i = 0;

            // Save되어있던 무기 개수만큼 Dequeue함.
            while (weaponPrefabs.Count != 0)
            {
                // 기존에 무기가 있을 시 기존 무기와 교체함.
                if (i < this.WeaponCount)
                {
                    this.ChangeWeapon(weaponPrefabs.Dequeue(), i);
                }
                // Save된 무기 개수가 현재 슬롯의 무기 개수보다 클 경우 새롭게 추가함.
                else
                {
                    this.AddWeapon(weaponPrefabs.Dequeue());
                }
                i++;
            }

            // Save되어있던 무기를 전부 추가후 무기 슬롯에 기존 무기가 남아있을 경우 파괴함.
            while (i < this.WeaponCount)
            {
                this.ChangeWeapon(null, i);
            }

        }
        catch (NullReferenceException)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 선택된 슬롯의 무기로 스왑한다.
    /// </summary>
    public void SwapWeapon(byte slotIndex)
    {
        if (!CanSwap)
            return;


        if (slotIndex < weaponSlot.Count && weaponSlot[slotIndex] != weapon && weaponSlot[slotIndex] != null)
        {
            StartCoroutine(SwapWeaponDelay(weaponSlot[slotIndex]));
        }
    }

    /// <summary>
    /// 무기 슬롯에 무기를 추가한다.
    /// </summary>
    /// <param name="newWeapon">추가할 무기</param>
    /// <exception cref="InvalidOperationException">슬롯 용량 초과 시 예외가 발생합니다.</exception>
    public void AddWeapon(Weapon newWeapon)
    {
        try
        {
            AddWeapon(newWeapon, (byte)weaponSlot.Count);
        }
        catch (InvalidOperationException e)
        {
            throw e;
        }
    }

    /// <summary>
    /// 원하는 무기 슬롯 공간에 무기를 삽입한다. 기존 무기들은 한칸씩 뒤로 밀린다.
    /// </summary>
    /// <param name="newWeapon">추가할 무기</param>
    /// <param name="slotIndex">원하는 슬롯 인덱스</param>
    /// <exception cref="InvalidOperationException">슬롯 용량 초과 시 예외가 발생합니다.</exception>
    public void AddWeapon(Weapon newWeapon, byte slotIndex)
    {
        if (weaponSlot.Count >= weaponSlotCapacity)
        {
            throw new InvalidOperationException($"무기 슬롯 용량을 초과하여 추가할 수는 없습니다. 무기 슬롯 개수: {weaponSlotCapacity}");
        }
        
        var created = CreateWeapon(newWeapon);

        try
        {
            weaponSlot.Insert(slotIndex, created);
        }
        catch (ArgumentOutOfRangeException)
        {
            weaponSlot.Add(created);
        }

        if (slotIndex == this.CurrentWeaponSlotNumber)
        {
            SwapWeapon(slotIndex);
        }
        else
        {
            this.CurrentWeaponSlotNumber = (byte)FindWeaponSlotIndex(weapon);
        }

        UIManager.Instance.SetWeaponSlotUI(this, weaponSlot);
    }

    /// <summary>
    /// 선택된 슬롯의 기존 무기를 파괴하고 새 무기로 교체한다. 새 무기는 GameObject, Prefab 모두 가능하다.<br/>
    /// newWeapon이 null일 경우 slotNumber에 해당되는 무기를 파괴한다.
    /// </summary>
    /// <param name="newWeapon">교체할 새 무기</param>
    /// <exception cref="ArgumentException">무기 슬롯에는 최소 1개 이상의 무기가 존재해야합니다.</exception>
    /// <exception cref="ArgumentOutOfRangeException">슬롯 인덱스가 무기 슬롯의 범위를 벗어날 시 예외가 발생합니다.</exception>
    public void ChangeWeapon(Weapon newWeapon, byte slotIndex = 0)
    {
        if (newWeapon == null)
        {
            if (weaponSlot.Count < 2)
                throw new ArgumentException($"무기 슬롯에는 최소 1개 이상의 무기가 존재해야합니다.", nameof(slotIndex));

            if (slotIndex >= weaponSlot.Count)
                throw new ArgumentOutOfRangeException(nameof(slotIndex), $"{nameof(slotIndex)}의 값이 무기 슬롯의 인덱스 범위를 벗어나 파괴가 불가능합니다. {nameof(slotIndex)}의 값: {slotIndex}, 무기 슬롯의 무기 개수: {weaponSlot.Count}");

            Destroy(weaponSlot[slotIndex].gameObject);
            weaponSlot.RemoveAt(slotIndex);
            weaponSlot.RemoveAll(w => w == null); // null은 모두 제거해줌

            UIManager.Instance.SetWeaponSlotUI(this, weaponSlot);

            if (slotIndex == this.CurrentWeaponSlotNumber)
            {
                if (weaponSlot.Count <= this.CurrentWeaponSlotNumber)
                {
                    SwapWeapon(0);
                }
                else
                {
                    SwapWeapon(this.CurrentWeaponSlotNumber);
                }
            }
            else if (slotIndex < this.CurrentWeaponSlotNumber)
            {
                SwapWeapon((byte)(this.CurrentWeaponSlotNumber - 1));
            }

            return;
        }

        if (slotIndex >= weaponSlot.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(slotIndex), $"{nameof(slotIndex)}의 값이 무기 슬롯의 인덱스 범위를 벗어나 교체가 불가능합니다. {nameof(slotIndex)}의 값: {slotIndex}, 무기 슬롯의 무기 개수: {weaponSlot.Count}");
        }

        // 새 무기를 생성 및 할당
        var created = CreateWeapon(newWeapon);

        var temp = previousWeapon;
        var weaponToDestory = weaponSlot[slotIndex];
        weaponSlot[slotIndex] = created;

        weaponToDestory.gameObject.SetActive(false);
        Destroy(weaponToDestory.gameObject);

        if (slotIndex == CurrentWeaponSlotNumber)
        {
            if (!CanSwap)
            {
                CanSwap = true;
                SwapWeapon(slotIndex);
                CanSwap = false;
            }
            else
            {
                SwapWeapon(slotIndex);
            }
        }

        previousWeapon = temp;
        UIManager.Instance.SetWeaponSlotUI(this, weaponSlot);
    }

    /// <summary>
    /// 원하는 무기에 해당되는 무기 슬롯 인덱스를 찾는다.
    /// </summary>
    /// <param name="weapon">찾고자 하는 무기</param>
    /// <param name="isSimpleCheck">false이면 Weapon Instance Matching 실패 시 Type Matching도 진행함</param>
    /// <returns>무기 슬롯 인덱스. 발견 실패 시 -1</returns>
    public int FindWeaponSlotIndex(Weapon weapon, bool isSimpleCheck = true)
    {
        int idx = weaponSlot.FindIndex(w => w == weapon);

        if (idx >= 0 || isSimpleCheck)
            return idx;

        // 찾고자 하는 무기를 찾지 못한 상태에서 단순체크가 아닌 경우 타입 체킹도 시도한다.
        idx = weaponSlot.FindIndex(w => w.GetType() == weapon.GetType());
        return idx;
    }

    //DeepCopy
    public Weapon DeepCopy()
    {
        String name = CurrentWeapon.Name;
        Weapon result = Resources.Load<Weapon>("Weapon/" + name);

        return result;
    }

    #endregion

    #region === IEnumerable Interface Members ===

    /// <summary>
    /// 무기 슬롯에 있는 무기를 반환
    /// </summary>
    /// <returns></returns>
    public IEnumerator<Weapon> GetEnumerator()
    {
        return ((IEnumerable<Weapon>)weaponSlot).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)weaponSlot).GetEnumerator();
    }

    #endregion
}


#region === Save System ===

public partial class PlayerShooter
{
    string ISaveable.ID => null;

    object ISaveable.Save() => new PlayerWeaponData(this, this.WeaponSlotCapacity);

    void ISaveable.Load(object loaded)
    {
        if (!loadWeaponData)
            return;

        if (loaded is PlayerWeaponData data)
        {
            if (data.weaponSlotCapacity < 1)
                return;

            Debug.Log($"<{nameof(PlayerShooter)}> {data}", this);

            this.weapon = null;
            this.WeaponSlotCapacity = data.weaponSlotCapacity;

            var weaponTypes = data.WeaponTypes;
            var weaponPrefabs = playerWeaponChangeInfo.GetWeaponPrefabs(weaponTypes);
            byte i = 0;
            // Save되어있던 무기 개수만큼 Dequeue함.
            while (weaponPrefabs.Count != 0)
            {
                // 기존에 무기가 있을 시 기존 무기와 교체함.
                if (i < this.WeaponCount)
                {
                    this.ChangeWeapon(weaponPrefabs.Dequeue(), i);
                }
                // Save된 무기 개수가 현재 슬롯의 무기 개수보다 클 경우 새롭게 추가함.
                else
                {
                    this.AddWeapon(weaponPrefabs.Dequeue());
                }
                i++;
            }

            // Save되어있던 무기를 전부 추가후 무기 슬롯에 기존 무기가 남아있을 경우 파괴함.
            while (i < this.WeaponCount)
            {
                this.ChangeWeapon(null, i);
            }
        }
        else
        {
            Debug.LogError($"<{nameof(PlayerShooter)}> 잘못된 데이터가 로드되었습니다. 필요한 데이터 타입: {typeof(PlayerWeaponData)}. 로드된 데이터 타입: {loaded?.GetType() ?? null}", this);
        }
    }
}

[Serializable]
public class PlayerWeaponData
{
    private Type[] weaponTypes;
    public readonly byte weaponSlotCapacity;
    public readonly byte weaponCount;

    public PlayerWeaponData(IEnumerable<Weapon> weapons, byte weaponSlotCapacity)
    {
        weaponTypes = weapons.Select(w => w.GetType()).ToArray();
        this.weaponSlotCapacity = weaponSlotCapacity;
        this.weaponCount = (byte)weapons.Count();
    }

    public IEnumerable<Type> WeaponTypes
    {
        get
        {
            foreach (var weapon in weaponTypes)
                yield return weapon;
        }
    }

    public override string ToString()
    {
        string weapons = string.Empty;
        for (int i = 0; i < weaponTypes.Length; i++)
        {
            weapons += $"{weaponTypes[i].Name}, ";
        }
        weapons = weapons.Trim(',', ' ');

        return $"PlayerWeapon Data {{ Weapon Slot Capacity = {weaponSlotCapacity}, Weapons[{weaponTypes.Length}] = {{ {weapons} }} }}";
    }
}

#endregion


#region === Private Members ===

public partial class PlayerShooter
{
    #region [Private Fields not to be showed in inspector]

    // Weapon Slot
    private List<Weapon> weaponSlot = new List<Weapon>();
    private Weapon previousWeapon;
    private bool isSwaping;

    // Other Player Components
    private PlayerInput playerInput;
    private Animator playerAnimator;

    //Reload Gauge
    private Slider reloadGauge;
    private RectTransform rectTransform;
    private bool isLeft;
    private float height = 2.5f;
    private float remainTime;

    #endregion

    #region [Unity Event Methods]

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<Animator>();


        reloadGauge = GetComponent<Hero>().ReloadGauge;
        rectTransform = reloadGauge.GetComponent<RectTransform>();
        reloadGauge.gameObject.SetActive(false);
        Vector3 pos = transform.position;
        rectTransform.position = new Vector3(pos.x, pos.y + height, 0);
        isLeft = false;

        if (weaponSlotCapacity == 0)
            weaponSlotCapacity = 1;

        // 시작 무기 설정
        if (weapon == null)
            weapon = Resources.Load<Weapon>("Weapon/Pistol");

        // 현재 weapon이 prefab일 경우 게임 도중 수정되는걸 방지하기 위해
        var temp = this.weapon;
        this.weapon = null;
        AddWeapon(temp);

        SaveManager.Add(this, SaveKey.GameData);

        #region === 시작 무기 설정 Legacy ===
        //bool created = false;
        //if (this.weaponLoad)
        //{
        //    //created = TryLoadWeapon();
        //}
        //if (!created)
        //{
        //    if (weapon == null)
        //        weapon = Resources.Load<Weapon>("Weapon/Pistol");

        //    // 현재 weapon이 prefab일 경우 게임 도중 수정되는걸 방지하기 위해
        //    var temp = this.weapon;
        //    this.weapon = null;
        //    AddWeapon(temp);
        //}
        #endregion
    }

    private void OnDestroy()
    {
        SaveManager.Remove(this, SaveKey.GameData);
    }

    private void Start()
    {
        //weaponSlot.Add(weapon);
        //UIManager.Instance.SetWeaponSlotUI(this, weaponSlot);
        GameManager.Instance.OnSceneLoaded += SceneLoaded;

        UIManager.Instance.ActivatePause += () => { CanFire = false; };
        UIManager.Instance.InactivatePause += () => { CanFire = true; };
    }

    private void Update()
    {
        // SaveSystem Test
        //SaveOrLoad();

        if (weaponSlot.Count > weaponSlotCapacity)
        {
            weaponSlot.GetRange(weaponSlotCapacity, weaponSlot.Count - weaponSlotCapacity).ForEach(w => Destroy(w.gameObject));
            weaponSlot.RemoveRange(weaponSlotCapacity, weaponSlot.Count - weaponSlotCapacity);
            if (CurrentWeaponSlotNumber >= weaponSlotCapacity)
            {
                SwapWeapon(0);
            }

            UIManager.Instance.SetWeaponSlotUI(this, weaponSlot);
        }

        WeaponSwapCheck();
        //Debug.Log($"키 체크: {Input.inputString}");

        if (!weapon)
            return;


        // 무기 회전 및 스케일 값 조정
        if (CanAim)
        {
            RotateWeapon();
            FlipPlayer();
        }

        // ReloadGauge 회전 및 스케일 값 조정
        RotateReloadGauge();

        // 무기에 대한 사용자 입력 감지 및 실행
        if (CanFire && playerInput.Fire)
        {
            weapon.Fire();
        }
        else if (playerInput.Reload)
        {
            weapon.Reload();
        }

        if (weapon) // null 참조 에러를 방지하기 위해 임시로 추가
        {
            weapon.IsFire = playerInput.Fire;
            weapon.ChangeState();
        }
    }

    #endregion

    #region [Private Methods]

    // 무기 Save 시스템 테스트용 메서드
    [Obsolete]
    private void SaveOrLoad()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            PlayerWeaponData playerWeaponData = new PlayerWeaponData(weaponSlot, weaponSlotCapacity);
            SaveSystem.SavePlayerWeapon(playerWeaponData);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            PlayerWeaponData playerWeaponData = SaveSystem.LoadPlayerWeapon();
            this.WeaponSlotCapacity = playerWeaponData.weaponSlotCapacity;

            var weaponTypes = playerWeaponData.WeaponTypes;
            var weaponPrefabs = playerWeaponChangeInfo.GetWeaponPrefabs(weaponTypes);

            byte i = 0;

            // Save되어있던 무기 개수만큼 Dequeue함.
            while (weaponPrefabs.Count != 0)
            {
                // 기존에 무기가 있을 시 기존 무기와 교체함.
                if (i < this.WeaponCount)
                {
                    ChangeWeapon(weaponPrefabs.Dequeue(), i);
                }
                // Save된 무기 개수가 현재 슬롯의 무기 개수보다 클 경우 새롭게 추가함.
                else
                {
                    AddWeapon(weaponPrefabs.Dequeue());
                }
                i++;
            }

            // Save되어있던 무기를 전부 추가후 무기 슬롯에 기존 무기가 남아있을 경우 파괴함.
            while (i < this.WeaponCount)
            {
                ChangeWeapon(null, i);
            }
        }
    }

    private void SceneLoaded(UnityEngine.SceneManagement.Scene? previous, UnityEngine.SceneManagement.Scene? loaded, SceneLoadingTiming when)
    {
        switch (when)
        {
            case SceneLoadingTiming.BeforeFadeOut:
                this.enabled = false;
                break;
            case SceneLoadingTiming.AfterFadeIn:
                this.enabled = true;
                break;
        }
    }

    // 무기 스왑 키 입력 체크
    private void WeaponSwapCheck()
    {
        //1/18 변경된 코드
        int idx2 = 0;

        if (GameManager.Instance.IsControllerConnection) //패드 활성화
        {
            if (Input.GetAxisRaw("Dpad Horizontal") > 0)
                idx2 = weaponSlotCapacity > CurrentWeaponSlotNumber + 1 ? CurrentWeaponSlotNumber + 1 : 0;
            else if (Input.GetAxisRaw("Dpad Horizontal") < 0)
                idx2 = CurrentWeaponSlotNumber - 1 < 0 ? weaponSlotCapacity - 1 : CurrentWeaponSlotNumber - 1;
            else
                return;

            SwapWeapon((byte)idx2);
            return;
        }

        //기존 코드
        string key = Input.inputString;
        // 1 ~ 5 숫자키를 눌렀을 시 무기 교체
        if (int.TryParse(key, out int idx))
        {
            idx--;
            SwapWeapon((byte)idx);
        }
        // 이전 무기로 교체(임시로 설정. InputManager 활용으로 대체할 것)
        else if (key == "q" || key == "Q")
        {
            for (byte i = 0; i < weaponSlot.Count; i++)
            {
                if (weaponSlot[i] == previousWeapon)
                {
                    SwapWeapon(i);
                    break;
                }
            }
        }

    }

    // 실제 무기 스왑 동작
    private IEnumerator SwapWeaponDelay(Weapon newWeapon)
    {
        if (isSwaping)
            yield break;

        isSwaping = true;

        float swapTime = newWeapon.Swap_time;

        yield return new WaitForSeconds(swapTime / 4);

        // 기존 무기 비활성화
        if (weapon != null)
        {
            weapon.IsReload = false; //ReloadGauge초기화를 위한 IsReload = false
            previousWeapon = weapon;
            weapon.gameObject.SetActive(false);
            weapon = null;
        }

        yield return new WaitForSeconds(swapTime / 2);

        // Weapon Change Delay 시간 동안 newWeapon이 null이 된 경우
        if (newWeapon == null)
        {
            // 이전 무기 체크
            for (byte i = 0; i < weaponSlot.Count; i++)
            {
                if (weaponSlot[i] == previousWeapon)
                {
                    newWeapon = weaponSlot[i];
                    CurrentWeaponSlotNumber = i;
                    break;
                }
            }

            // 이전 무기 역시 파괴된 경우 null이 아닌 무기를 앞에서부터 선택
            if (newWeapon == null)
            {
                for (byte i = 0; i < weaponSlot.Count; i++)
                {
                    if (weaponSlot[i] != null)
                    {
                        newWeapon = weaponSlot[i];
                        CurrentWeaponSlotNumber = i;
                        break;
                    }
                }
            }
        }
        else
        {
            for (byte i = 0; i < weaponSlot.Count; i++)
            {
                if (weaponSlot[i] == newWeapon)
                    CurrentWeaponSlotNumber = i;
            }
        }
        // 무기 교체 작업
        weapon = newWeapon;
        weapon.gameObject.SetActive(true);

        // 각 무기의 왼쪽 손잡이와 오른쪽 손잡이를 각 팔의 CCD Solver에 할당함.
        var leftHandle = weapon.transform.Find("Left Handle");
        var rightHandle = weapon.transform.Find("Right Handle");

        leftArmCCDSolver.GetChain(0).target = leftHandle;
        rightArmCCDSolver.GetChain(0).target = rightHandle;

        UIManager.Instance.SetWeaponSlotUI(this, weaponSlot);

        yield return new WaitForSeconds(swapTime / 4);

        isSwaping = false;
    }

    // 새롭게 사용할 무기 생성, 배치, 초기화
    // 반환 값: 슬롯에 추가된 무기
    private Weapon CreateWeapon(Weapon newWeapon)
    {
        // 프리팹 여부 체크
        if (newWeapon.gameObject.scene.name == null)
        {
            newWeapon = Instantiate(newWeapon);
        }
        newWeapon.gameObject.SetActive(false);

        WeaponInfo weaponInfo = playerWeaponChangeInfo.GetWeaponInfo(newWeapon);
        if (weaponInfo.localScale == Vector3.zero)
        {
            Debug.LogError($"{playerWeaponChangeInfo.GetType()}에 설정된 {weaponInfo.GetType()}의 Local Scale이 Zero Vector 입니다.\nLocal Scale : {weaponInfo.localScale}");
        }

        newWeapon.transform.parent = gunPivot;
        newWeapon.transform.localPosition = weaponInfo.localPosition;
        newWeapon.transform.localScale = weaponInfo.localScale;

        return newWeapon;
    }

    private void FlipPlayer()
    {
        if (GameManager.Instance.IsControllerConnection)
        {
            if (Input.GetAxis("right stick Horizontal") > 0)
            {
                float Dir_x = Mathf.Abs(transform.localScale.x);
                float Dir_y = transform.localScale.y;
                float Dir_z = transform.localScale.z;
                transform.localScale = new Vector3(Dir_x, Dir_y, Dir_z);
            }
            else if (Input.GetAxis("right stick Horizontal") < 0)
            {
                float Dir_x = transform.localScale.x;
                if (Dir_x > 0)
                {
                    Dir_x *= -1;
                }
                float Dir_y = transform.localScale.y;
                float Dir_z = transform.localScale.z;
                transform.localScale = new Vector3(Dir_x, Dir_y, Dir_z);
            }
        }
        else
        {
            //마우스 포인터가 향하는 방향으로 flip해준다.
            if (playerInput.MousePosition.x > gameObject.transform.position.x)        //마우스 포인터가 플레이어보다 오른쪽일 경우
            {
                float Dir_x = Mathf.Abs(transform.localScale.x);
                float Dir_y = transform.localScale.y;
                float Dir_z = transform.localScale.z;
                transform.localScale = new Vector3(Dir_x, Dir_y, Dir_z);
            }
            else if (playerInput.MousePosition.x < gameObject.transform.position.x)   //마우스 포인터가 플레이어보다 왼쪽일 경우
            {
                float Dir_x = transform.localScale.x;
                if (Dir_x > 0)
                {
                    Dir_x *= -1;
                }
                float Dir_y = transform.localScale.y;
                float Dir_z = transform.localScale.z;
                transform.localScale = new Vector3(Dir_x, Dir_y, Dir_z);
            }
        }
    }

    // 사용 중인 무기 회전
    private void RotateWeapon()
    {
        Vector2 directionToRotate;
        float rotateDegree;

        //변경한 코드
        if (GameManager.Instance.IsControllerConnection)   //패드 활성화
        {
            // 패드 무기 회전
            directionToRotate = new Vector2(Input.GetAxis("right stick Horizontal"), Input.GetAxis("right stick Vertical") * -1);


            rotateDegree = Mathf.Atan2(directionToRotate.y, directionToRotate.x) * Mathf.Rad2Deg;
            weapon.transform.rotation = Quaternion.Euler(0f, 0f, rotateDegree);

            if (rotateDegree >= 45 && rotateDegree <= 135)
                playerAnimator.SetBool("isLookUp", true);
            else
                playerAnimator.SetBool("isLookUp", false);

            // 패드 좌우, 상하반전 처리
            if (Input.GetAxis("right stick Horizontal") > 0)
            {
                weapon.transform.localScale = new Vector3(-Mathf.Abs(weapon.transform.localScale.x), Mathf.Abs(weapon.transform.localScale.y), weapon.transform.localScale.z);
            }
            else if (Input.GetAxis("right stick Horizontal") < 0)
            {
                weapon.transform.localScale = new Vector3(Mathf.Abs(weapon.transform.localScale.x), -Mathf.Abs(weapon.transform.localScale.y), weapon.transform.localScale.z);
            }
        }
        else                                        //패드 비활성화
        {
            // 마우스 무기 회전
            directionToRotate = playerInput.MousePosition - (Vector2)weapon.transform.position;

            rotateDegree = Mathf.Atan2(directionToRotate.y, directionToRotate.x) * Mathf.Rad2Deg;
            weapon.transform.rotation = Quaternion.Euler(0f, 0f, rotateDegree);

            if (rotateDegree >= 45 && rotateDegree <= 135)
                playerAnimator.SetBool("isLookUp", true);
            else
                playerAnimator.SetBool("isLookUp", false);

            // 마우스 좌우, 상하반전 처리
            if (playerInput.MousePosition.x > transform.position.x)
            {
                weapon.transform.localScale = new Vector3(-Mathf.Abs(weapon.transform.localScale.x), Mathf.Abs(weapon.transform.localScale.y), weapon.transform.localScale.z);
            }
            else
            {
                weapon.transform.localScale = new Vector3(Mathf.Abs(weapon.transform.localScale.x), -Mathf.Abs(weapon.transform.localScale.y), weapon.transform.localScale.z);
            }
        }

        ////기존 코드            
        //// 마우스 무기 회전
        //directionToRotate = playerInput.MousePosition - (Vector2)weapon.transform.position;

        //rotateDegree = Mathf.Atan2(directionToRotate.y, directionToRotate.x) * Mathf.Rad2Deg;
        //weapon.transform.rotation = Quaternion.Euler(0f, 0f, rotateDegree);

        //if (rotateDegree >= 45 && rotateDegree <= 135)
        //    playerAnimator.SetBool("isLookUp", true);
        //else
        //    playerAnimator.SetBool("isLookUp", false);

        //// 마우스 좌우, 상하반전 처리
        //if (playerInput.MousePosition.x > transform.position.x)
        //{
        //    weapon.transform.localScale = new Vector3(-Mathf.Abs(weapon.transform.localScale.x), Mathf.Abs(weapon.transform.localScale.y), weapon.transform.localScale.z);
        //}
        //else
        //{
        //    weapon.transform.localScale = new Vector3(Mathf.Abs(weapon.transform.localScale.x), -Mathf.Abs(weapon.transform.localScale.y), weapon.transform.localScale.z);
        //}

    }


    /// <summary>
    /// RelodaGauge의 Scale을 조정해주는 메서드
    /// </summary>
    private void RotateReloadGauge()
    {

        if (transform.localScale.x < 0 && !isLeft)
        {
            rectTransform.localScale = new Vector3(-rectTransform.localScale.x, rectTransform.localScale.y, rectTransform.localScale.z);
            isLeft = true;
        }
        else if (transform.localScale.x > 0 && isLeft)
        {
            rectTransform.localScale = new Vector3(-rectTransform.localScale.x, rectTransform.localScale.y, rectTransform.localScale.z);
            isLeft = false;
        }

        if (weapon.IsReload)
        {
            reloadGauge.gameObject.SetActive(true);
            remainTime += Time.deltaTime;
            reloadGauge.value = remainTime / weapon.Reload_time;
        }
        else
        {
            reloadGauge.gameObject.SetActive(false);
            remainTime = 0;
        }
    }

    #endregion
}

#endregion
