using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;


public partial class Store : MonoBehaviour, IUiActiveCheck
{
    #region === Serialized Fields ===

    [SerializeField] private StoreUI weaponStoreUI;
    [SerializeField] private StoreUI itemStoreUI;
    [SerializeField] private Button[] weaponSlotSelectionBtns = new Button[10];
    [SerializeField] private float weaponSlotSelectionBtnPlacementInterval = 250;
    [SerializeField, HideInInspector] private SerializableDictionary<StoreItemSlot, StoreItemData<Weapon>> saleWeapons = new SerializableDictionary<StoreItemSlot, StoreItemData<Weapon>>();
    [SerializeField, HideInInspector] private SerializableDictionary<StoreItemSlot, StoreItemData<Item>> saleItems = new SerializableDictionary<StoreItemSlot, StoreItemData<Item>>();

    #endregion

    #region === Public Static Members ===

    public static bool SetCanChangeItemCount(int buildIndex)
    {
        //Store.LoadStoreData();
        if (!canChangeItemCount.ContainsKey(buildIndex))
        {
            Store.canChangeItemCount[buildIndex] = true;
            return true;
        }

        return canChangeItemCount[buildIndex];
    }

    #endregion

    #region === Public Properties ===

    [System.Obsolete]
    public bool IsStoreActive { get => this.isStoreActive; }

    #endregion

    #region === Public Methods ===

    public void SetLeftItemsCount(IEnumerable<KeyValuePair<StoreItemSlot, int>> changed)
    {
        //Store.LoadStoreData();

        int buildIndex = gameObject.scene.buildIndex;
        if (!Store.canChangeItemCount.ContainsKey(buildIndex) || !Store.canChangeItemCount[buildIndex])
            throw new System.InvalidOperationException("현재 Scene에서는 Store의 항목을 변경할 수 없습니다.");

        if (changed.Any(p => !saleItems.ContainsKey(p.Key)))
            throw new KeyNotFoundException("changed 열거자의 key값에 Store에 존재하는 아이템 슬롯과 매칭되지 않는 아이템 슬롯이 존재합니다.");

        if (changed.Any(p => p.Key.Count == null))
            throw new System.InvalidOperationException("changed 열거자의 key값에 아이템 슬롯의 아이템 개수를 변경할 수 없는 아이템 슬롯이 존재합니다.");    

        foreach (var pair in changed)
        {
            SetSlotItemCount(pair.Key, pair.Value);
        }

        Store.canChangeItemCount[buildIndex] = false;
        //GameManager.Instance.GameDataLoaded.StoreData = new StoreData(Store.leftItemCount, Store.canChangeItemCount);
    }

    public int this[StoreItemSlot key]
    {
        get
        {
            if (!saleItems.ContainsKey(key))
                throw new KeyNotFoundException("key 값에 해당하는 상점 슬롯 이름에 대한 아이템 개수를 찾을 수 없습니다.");

            return leftItemCount[key.name];
        }
    }

    #endregion
}


#region === Interface Members ===

public partial class Store
{
    #region === IUiActiveCheck Interface Members ===

    public bool IsUIActive { get => this.isStoreActive; }

    #endregion
}

#endregion


[System.Serializable]
public struct StoreItemData<T> where T : MonoBehaviour
{
    public T itemPrefab;
    public int price;
    public int count;
}

#region === Private Members ===

public partial class Store
{
    #region === Private Static Members ===

    private static Dictionary<string, int> leftItemCount = new Dictionary<string, int>();
    private static Dictionary<int, bool> canChangeItemCount = new Dictionary<int, bool>();

    // TODO: 기존 GameManager.Instance.GameDataLoaded.StoreData에 할당하던 과정을 storeDataAgent
    private static StoreDataSaveListener storeDataAgent = new StoreDataSaveListener(leftItemCount, canChangeItemCount);

    [System.Obsolete]
    private static void LoadStoreData()
    {
        if (Store.leftItemCount == null)
        {
            Store.leftItemCount = new Dictionary<string, int>();

            try
            {
                foreach (var pair in GameManager.Instance.GameDataLoaded.StoreData.LeftItemCounts)
                {
                    Store.leftItemCount[pair.Key] = pair.Value;
                }
                Debug.Log($"<{nameof(Store)}> Store 남은 아이템 데이터 로드 성공");
            }
            catch (System.NullReferenceException)
            {
                Debug.Log($"<{nameof(Store)}> Store 남은 아이템 데이터 로드 실패");
            }
        }


        if (Store.canChangeItemCount == null)
        {
            Store.canChangeItemCount = new Dictionary<int, bool>();

            try
            {
                foreach (var pair in GameManager.Instance.GameDataLoaded.StoreData.CanChangeItemCount)
                {
                    Store.canChangeItemCount[pair.Key] = pair.Value;
                    //Debug.Log($"<{nameof(Store)}> {pair.key}의 Store 아이템 개수 변경 가능: {pair.Value}");
                }
                Debug.Log($"<{nameof(Store)}> Store 아이템 개수 변경 가능 데이터 로드 성공");
            }
            catch (System.NullReferenceException)
            {
                Debug.Log($"<{nameof(Store)}> Store 아이템 개수 변경 가능 데이터 로드 실패");
            }
        }
    }

    #endregion

    #region === Private Fields ===

    private string countPrefix = "Left Item";

    private bool isStoreActive;
    private bool isPlayerOutOfStore = true;
    private float lastStoreCloseTime;
    private float tolerance = 0.1f;

    private StoreItemSlot selectedBuyBtn;
    private StoreUI currentStoreUI;

    // Player Components
    private PlayerInput playerInput;
    private PlayerShooter playerShooter;
    private Rigidbody2D playerRigid;
    private PlayerMovement playerMovement;

    private float playerSpeed;

    //private int changedCountForItemCount;

    #endregion

    #region === Unity Event Methods ===

    private void Awake()
    {
        //Store.LoadStoreData();
    }

    private void Start()
    {
        SetWeaponBuyUI();

        SetItemBuyUI();
    }

    // Update is called once per frame
    private void Update()
    {
        // Store가 활성화 되었을 때 esc키를 누르면
        if (isStoreActive && playerInput.Cancel)
        {
            if (currentStoreUI == weaponStoreUI)
                CloseSlotSelectionUI();

            //weaponStoreUI.gameObject.SetActive(false);
            currentStoreUI.gameObject.SetActive(false);
            currentStoreUI = null;
            isStoreActive = false;

            //playerInput.enabled = true;
            //playerRigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            this.PlayerControl(true);
            lastStoreCloseTime = Time.time;

            UIManager.Instance.IsOtherUIActive = false;
        }
        // Store가 비활성화 되었을 때
        else if (!isStoreActive && Time.time >= lastStoreCloseTime + tolerance)
        {
            // Player가 상점 안에 있을 경우
            if (!isPlayerOutOfStore)
            {
                if (playerInput.Interact)
                {
                    //weaponStoreUI.gameObject.SetActive(true);
                    UIManager.Instance.IsOtherUIActive = true;

                    StoreUIOn(this.weaponStoreUI, this.saleWeapons);
                    isStoreActive = true;

                    this.PlayerControl(false);
                    //playerInput.enabled = false;
                    //playerRigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                }
            }
            else
            {
                playerInput = null;
                playerShooter = null;
                playerRigid = null;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && playerInput == null)
        {
            playerInput = collision.GetComponent<PlayerInput>();
            playerShooter = collision.GetComponent<PlayerShooter>();
            playerRigid = collision.GetComponent<Rigidbody2D>();
            playerMovement = collision.GetComponent<PlayerMovement>();

            //this.playerSpeed = playerMovement.ChangeBasicSpeed;
            this.isPlayerOutOfStore = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            this.isPlayerOutOfStore = true;
        }
    }

    #endregion

    #region === Private Methods ===

    private void SetSlotItemCount(StoreItemSlot slot, int count)
    {
        var data = this.saleItems[slot];
        data.count = count;
        this.saleItems[slot] = data;
        Store.leftItemCount[slot.name] = count;
        slot.Count.text = $"{this.countPrefix} {count}";
        Debug.Log($"<{nameof(Store)}> GameManager와 남은 아이템 개수 동기화 성공", this);
    }

    private void SetWeaponBuyUI()
    {
        // 무기 구매 버튼
        var storeWeaponSlots = saleWeapons.Keys.OrderBy(s => this.saleWeapons[s].price);
        int slotIndex = 0;
        foreach (var slot in storeWeaponSlots)
        {
            slot.BuyButton.onClick.AddListener(() => OnWeaponBuyBtnClicked(slot));
            var data = saleWeapons[slot];
            data.count = -1;
            saleWeapons[slot] = data;
            slot.Price.text = $"{saleWeapons[slot].price}";
            //slot.BuyButton.onClick.AddListener(() => SetSlotsInteractable(saleWeapons));
            //Debug.Log($"<{nameof(Store)}> 형제 인덱스: {slot.transform.GetSiblingIndex()}", slot);
            slot.transform.SetSiblingIndex(slotIndex);
            slotIndex++;
        }

        // 무기 선택 버튼
        for (byte i = 0; i < weaponSlotSelectionBtns.Length; i++)
        {
            byte idx = i;
            weaponSlotSelectionBtns[i].onClick.AddListener(() => OnSlotSelectionBtnClicked(idx));
        }

        var itemStoreBtnHandler = weaponStoreUI.OtherStoreBtn.onClick;
        itemStoreBtnHandler.AddListener(() => weaponStoreUI.gameObject.SetActive(false));
        itemStoreBtnHandler.AddListener(() => StoreUIOn(itemStoreUI, this.saleItems));
    }

    private void SetItemBuyUI()
    {
        // 아이템 구매 버튼
        var storeItemSlots = saleItems.Keys.OrderBy(s => this.saleItems[s].price);
        int slotIndex = 0;
        foreach (var slot in storeItemSlots)
        {
            slot.BuyButton.onClick.AddListener(() => OnItemBuyBtnClicked(slot));
            slot.Price.text = $"{saleItems[slot].price}";

            if (slot.Count != null)
            {
                // 남은 아이템 개수 Save용 Dictionary 동기화
                if (leftItemCount.ContainsKey(slot.name))
                {
                    var data = saleItems[slot];
                    data.count = Store.leftItemCount[slot.name];
                    saleItems[slot] = data;
                }
                else
                {
                    Store.leftItemCount[slot.name] = saleItems[slot].count;
                }

                slot.Count.text = $"{this.countPrefix} {saleItems[slot].count}";
            }
            //slot.BuyButton.onClick.AddListener(() => SetSlotsInteractable(saleItems));
            slot.transform.SetSiblingIndex(slotIndex);
            slotIndex++;
        }

        var weaponStoreBtnhandler = itemStoreUI.OtherStoreBtn.onClick;
        weaponStoreBtnhandler.AddListener(() => itemStoreUI.gameObject.SetActive(false));
        weaponStoreBtnhandler.AddListener(() => StoreUIOn(weaponStoreUI, this.saleWeapons));
    }


    private void OnWeaponBuyBtnClicked(StoreItemSlot slot)
    {
        selectedBuyBtn = slot;

        PlaceSlotSelectionBtns();
        weaponStoreUI.ItemBuyUI.gameObject.SetActive(false);
        weaponStoreUI.WeaponSlotSelectionUI.gameObject.SetActive(true);
    }

    private void OnItemBuyBtnClicked(StoreItemSlot slot)
    {
        var itemData = saleItems[slot];
        if (itemData.itemPrefab != null && playerInput.gameObject != null && GameManager.Instance.Gold >= itemData.price)
        {
            itemData.itemPrefab.GetItem(playerInput.gameObject);
            GameManager.Instance.Gold -= itemData.price;
            if (slot.Count != null)
            {
                itemData.count--;
                this.SetSlotItemCount(slot, itemData.count);
                //GameManager.Instance.GameDataLoaded.StoreData = new StoreData(Store.leftItemCount, Store.canChangeItemCount);
            }
            saleItems[slot] = itemData;
            SetSlotsInteractable(this.saleItems);
        }
    }

    private void OnSlotSelectionBtnClicked(byte slotIndex)
    {
        var weaponData = saleWeapons[selectedBuyBtn];

        if (weaponData.itemPrefab != null && playerShooter != null && GameManager.Instance.Gold >= weaponData.price)
        {
            try
            {
                // 무기 슬롯 용량이 아직 다 차있지 않은 경우 추가함.
                playerShooter.AddWeapon(weaponData.itemPrefab, slotIndex);
            }
            catch (System.InvalidOperationException)
            {
                // 무기 슬롯 용량이 다 찬경우 기존 무기와 교체함.
                playerShooter.ChangeWeapon(weaponData.itemPrefab, slotIndex);
            }
            GameManager.Instance.Gold -= weaponData.price;
            Debug.Log($"<{nameof(Store)}> {weaponData.itemPrefab.name} 구매 완료", this);
            SetSlotsInteractable(this.saleWeapons);
        }

        CloseSlotSelectionUI();
    }

    private void PlaceSlotSelectionBtns()
    {
        float current = -((playerShooter.WeaponSlotCapacity - 1) / 2f * weaponSlotSelectionBtnPlacementInterval); // 시작 배치 위치
        weaponStoreUI.WeaponSlotSelectionUI.rectTransform.sizeDelta = new Vector2((Mathf.Abs(current) + 200) * 2, weaponStoreUI.WeaponSlotSelectionUI.rectTransform.sizeDelta.y);

        byte btnCount = (byte)Mathf.Min(playerShooter.WeaponSlotCapacity, weaponSlotSelectionBtns.Length);
        for (int i = 0; i < btnCount; i++)
        {
            // 무기 슬롯 선택 버튼 배치
            weaponSlotSelectionBtns[i].gameObject.SetActive(true);
            weaponSlotSelectionBtns[i].targetGraphic.rectTransform.anchoredPosition = new Vector2(current, 0f);
            current += weaponSlotSelectionBtnPlacementInterval;
        }
    }

    private void CloseSlotSelectionUI()
    {
        // 슬롯 선택 버튼 비활성화
        for (int i = 0; i < weaponSlotSelectionBtns.Length; i++)
        {
            weaponSlotSelectionBtns[i].gameObject.SetActive(false);
        }

        weaponStoreUI.WeaponSlotSelectionUI.gameObject.SetActive(false);
        weaponStoreUI.ItemBuyUI.gameObject.SetActive(true);
    }

    private void PlayerControl(bool enabled)
    {
        //playerRigid.velocity = enabled ? playerRigid.velocity : Vector2.zero;
        playerRigid.constraints = enabled ? RigidbodyConstraints2D.FreezeRotation : RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        playerShooter.CanAim = enabled;
        playerShooter.CanFire = enabled;
        playerShooter.CanSwap = enabled;
        playerMovement.JumpControl = enabled;
        playerMovement.MoveControl = enabled;
    }

    private void StoreUIOn<T>(StoreUI store, SerializableDictionary<StoreItemSlot, StoreItemData<T>> itemSlots) where T : MonoBehaviour
    {
        SetSlotsInteractable(itemSlots);
        store.gameObject.SetActive(true);
        this.currentStoreUI = store;
    }

    private void SetSlotsInteractable<T>(SerializableDictionary<StoreItemSlot, StoreItemData<T>> itemSlots) where T : MonoBehaviour
    {
        foreach (var slot in itemSlots.Keys)
        {
            slot.BuyButton.interactable = GameManager.Instance.Gold >= itemSlots[slot].price && itemSlots[slot].count != 0;
            //Debug.Log($"<{nameof(Store)}> {itemSlots[slot].itemPrefab.name}", this);
        }
    }

    #endregion
}

#endregion
