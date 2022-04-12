using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{

    [Header("Text UI")]
    [SerializeField] Text UIStageText;
    [SerializeField] Text UICoinText;
    [SerializeField] Text UIBulletText;
    [SerializeField] Text UIHpText;
    [SerializeField] Text UIRemainEnemyText;
    [SerializeField] Text UIRemainLife;

    [Header("Image UI")]
    [SerializeField] GameObject resurrectionImage;

    [Header("Other GameObject UI")]
    [SerializeField] Store UIStore;
    [SerializeField] RecordBoard UIRecordBoard;
    [SerializeField] GameObject UIGameOver;
    [SerializeField] SettingButtonUI settingButton;

    [Header("Object UI")]
    [SerializeField] GameObject UIPause;
    [SerializeField] GameObject UISetting;
    [SerializeField] Slider UIHpBar;
    [SerializeField] Slider UIStaminaBar;
    [SerializeField] Slider UIBossHpBar;
    [SerializeField] Graphic weaponPanel;
    [SerializeField] Image weaponImagePrefab;
    [SerializeField] float weaponImagePlacementInterval = 200;
    [SerializeField] Slider soundSlider;
    [SerializeField] GameObject StartSceneUI;

    private bool isEventActive;
    private bool isSetting = false;
    private bool isOtherUIActive = false;

    private List<Type> weaponTypes = new List<Type>();
    private List<Image> images = new List<Image>();
    private byte currentWeaponIdx = byte.MaxValue;
    
    protected UIManager() { }

    //restart 버튼 클릭시 실행되는 이벤트
    public event Action OnClickRestartButton;

    //일시정지 활성화시 실행되는 이벤트
    public event Action ActivatePause;

    //일시정지 비활성화시 실행되는 이벤트
    public event Action InactivatePause;

    //setting 버튼 클릭시 다른 버튼 제어 프로퍼티
    public bool IsSettingActive { get { return isSetting; } set { isSetting = value; } }

    public bool IsOtherUIActive { get { return isOtherUIActive; } set { isOtherUIActive = value; } }

    private void Awake()
    {
        if (!CheckSingletonInstance(true))
            return;

        UIPause.SetActive(false);
        UIGameOver.SetActive(false);
        UIBossHpBar.gameObject.SetActive(false);

        GameManager.Instance.OnSceneLoaded += OnSceneLoaded;
        //SettingDataController.OnSettingDataLoaded += SetSound;

        GetUIGameObject();
    }

    // start Scene가 아닌 다른 곳에서 자동 설정
    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            StartSceneUI.SetActive(false);

            SetPlayerUI(true);
            //this.transform.Find("Player UI").gameObject.SetActive(true);
        }
        isEventActive = false;

        SetSound(SettingDataController.Data);
    }

    private void OnSceneLoaded(Scene? previous, Scene? loaded, SceneLoadingTiming when)
    {
        switch (when)
        {
            case SceneLoadingTiming.BeforeLoading:
                StartSceneUI.SetActive(false);
                break;
            case SceneLoadingTiming.AfterLoading:
                if (loaded.Value.buildIndex == GameManager.Instance.InGameStartSceneBuildIndex)
                {
                    //this.transform.Find("Player UI").gameObject.SetActive(true);
                    SetPlayerUI(true);
                    GetUIGameObject();
                }
                UIStageText.text = SceneManager.GetActiveScene().name;
                break;

        }
    }

    Stack<GameObject> stack = new Stack<GameObject>();

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0 || isOtherUIActive)
            return;

        if (stack.Count == 0 && Input.GetButtonDown("pause button"))
        {
            UIPause.SetActive(true);
            stack.Push(UIPause);
        }
        else if (Input.GetButtonDown("pause button"))
        {
            stack.Pop().SetActive(false);
            try
            {
                stack.Peek().SetActive(true);
            }
            catch (Exception ex){ }
            finally
            {
                SettingDataController.SaveSettingData();
            }
        }

        #region 기존 Pause logic
        //if (stack.Count == 0 && Input.GetButtonDown("pause button"))
        //{
        //    if (UIStore != null && UIRecordBoard != null)
        //    {
        //        if (UIStore.IsUIActive || UIRecordBoard.IsUIActive)
        //        {
        //            return;
        //        }
        //    }
        //    if (UIGameOver.activeSelf || isEventActive || UISetting.activeSelf)
        //        return;

        //    if (ActivatePause != null)
        //        ActivatePause();

        //    stack.Push(UIPause);
        //    UIPause.SetActive(true);

        //}
        //else if (stack.Count == 1 && Input.GetButtonDown("pause button"))
        //{
        //    if (stack.Peek().GetComponent<IUiActiveCheck>() != null)
        //    {
        //        stack.Pop();
        //        return;
        //    }

        //    stack.Pop().SetActive(false);

        //    if (InactivatePause != null)
        //        InactivatePause();
        //}
        //else if (stack.Count >= 2 && Input.GetButtonDown("pause button"))
        //{
        //    if (stack.Peek().GetComponent<IUiActiveCheck>() != null)
        //    {
        //        stack.Pop();
        //        return;
        //    }
        //    stack.Pop().SetActive(false);
        //    stack.Peek().SetActive(true);
        //    AudioListener.volume = soundSlider.value;
        //}
        #endregion

        //set timeScale
        if (UIPause.activeSelf)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    void SetSound(SettingData data)
    {
        // 현재 setting file이 없더라도 Setting에서 Data를 가져와 1로 초기화 시킴
        if (data == null)   //사용 x
        {
            AudioListener.volume = 0.3f;
            soundSlider.value = 0.3f;
        }
        else
        {
            AudioListener.volume = data.audioVolume;
            soundSlider.value = data.audioVolume;
        }
    }

    private void GetUIGameObject()
    {
        UIStore = FindObjectOfType<Store>();
        UIRecordBoard = FindObjectOfType<RecordBoard>();

    }

    public void SetPlayerUI(bool value)
    {
        transform.Find("Player UI").gameObject.SetActive(value);
    }
    
    /// <summary>
    /// StageText UI를 설정해주는 메서드
    /// </summary>
    /// <param name="text"></param>
    public void SetStageText(string text)
    {
        UIStageText.text = text;
    }

    /// <summary>
    /// 게임 재시작 버튼 활성화 메서드
    /// </summary>
    public void SetActiveRestartButton()
    {
        UIGameOver.SetActive(true);
    }

    /// <summary>
    /// 게임 재시작 메서드
    /// </summary>
    public void Restart()
    {
        OnClickRestartButton();

        UIGameOver.SetActive(false);
        UIPause.SetActive(false);
        Time.timeScale = 1;
    }

    /// <summary>
    /// Left Enemy Text UI를 설정해주는 메서드
    /// </summary>
    /// <param name="count"></param>
    public void SetRemainEnemyUI(int count)
    {
        if (!UIRemainEnemyText.enabled)
            return;

        UIRemainEnemyText.text = "Enemy : " + count.ToString();
    }

    /// <summary>
    /// 남은 목숨 표시 UI
    /// </summary>
    /// <param name="count"></param>
    public void SetRemainLife(int count)
    {
        if (!UIRemainLife.enabled)
            return;

        UIRemainLife.text = "Life  		: " + count.ToString();
    }

    /// <summary>
    /// Left Enemy Text UI의 enabled를 설정할수있는 메서드 b = true or false
    /// </summary>
    /// <param name="b"></param>
    public void EnabledRemainEnemyUI(bool b)
    {
        UIRemainEnemyText.enabled = b;
    }

    public void EnabledBossHealthUI(bool b)
    {
        UIBossHpBar.gameObject.SetActive(b);
    }


    /// <summary>
    /// coinText UI를 설정해주는 메서드
    /// </summary>
    /// <param name="gold"></param>
    public void SetGoldUI(int gold)
    {
        UICoinText.text = GameManager.Instance.Gold.ToString();
    }

    /// <summary>
    /// BulletText UI를 설정해주는 메서드
    /// </summary>
    /// <param name="curBullet"></param>
    /// <param name="maxBullet"></param>
    public void SetBulletUI(int curBullet, int maxBullet)
    {
        UIBulletText.text = curBullet.ToString() + "/" + maxBullet.ToString();
    }

    /// <summary>
    /// HText UI와 HpBar UI를 설정해주는 메서드
    /// </summary>
    /// <param name="health"></param>
    /// <param name="maxHealth"></param>
    public void SetHealthUI(float health, float maxHealth)
    {
        UIHpText.text = (Mathf.Ceil(health*10)/10).ToString() + "/" + maxHealth.ToString();
        UIHpBar.value = health / maxHealth;
    }

    public void SetStaminaUI(float Stamina, float maxStamina)
    {
        UIStaminaBar.value = Stamina / maxStamina;
    }

    /// <summary>
    /// Boss의 HpBar를 설정해주는 메서드
    /// </summary>
    /// <param name="health"></param>
    /// <param name="maxHealth"></param>
    public void SetBossHealthUI(float health, float maxHealth)
    {
        UIBossHpBar.value = health / maxHealth;
    }

    /// <summary>
    /// ResurrectionImage 설정 메서드
    /// </summary>
    /// <param name="val"></param>
    public void SetResurrectionImage(bool b)
    {
        
        resurrectionImage.SetActive(b);
    }

    public void ActiveteSetting()
    {
        stack.Peek().SetActive(false);
        stack.Push(UISetting);
        UISetting.SetActive(true);
    }

    public void ChangeSound()
    {
        AudioListener.volume = soundSlider.value;
    }

    /// <summary>
    /// 무기 슬롯 UI를 설정해주는 메서드
    /// </summary>
    public void SetWeaponSlotUI(PlayerShooter playerShooter, IEnumerable<Weapon> weaponSlot)
    {
        weaponSlot = weaponSlot.Where(w => w != null); // null 제외
        
        bool isTransformed = false;

        // 무기 이미지 생성 및 기본 세팅
        int weaponIdx = 0;
        foreach (var weapon in weaponSlot)
        {
            if (weaponIdx < images.Count)
            {
                if (weaponTypes[weaponIdx] == weapon.GetType())
                {
                    weaponIdx++;
                    continue;
                }

                // 기존 무기 슬롯 UI의 무기 이미지를 파괴 후 새롭게 생성 후 할당
                Destroy(images[weaponIdx].gameObject);
                images[weaponIdx] = Instantiate(weaponImagePrefab);
                weaponTypes[weaponIdx] = weapon.GetType();
            }
            else
            {
                // 무기 슬롯 UI에 무기 이미지를 새롭게 추가
                images.Add(Instantiate(weaponImagePrefab));
                weaponTypes.Add(weapon.GetType());
            }

            images[weaponIdx].sprite = weapon.GetComponent<SpriteRenderer>().sprite; // 실제 이미지 할당
            images[weaponIdx].transform.SetParent(weaponPanel.transform); // Weapon Panel UI Object의 자식 오브젝트로 할당
            images[weaponIdx].SetNativeSize();
            images[weaponIdx].rectTransform.localScale = Vector3.one;
            images[weaponIdx].GetComponent<Shadow>().effectDistance = new Vector2(10, -10); // 그림자 효과

            // 투명도 조정
            Color temp = images[weaponIdx].color;
            temp.a = 0.5f;
            images[weaponIdx].color = temp;

            // 무기 이미지의 앵커 프리셋을 중앙으로 조정
            images[weaponIdx].rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            images[weaponIdx].rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

            isTransformed = true;
            weaponIdx++;
        }

        // 현재 무기 슬롯의 무기 개수보다 초과된 이미지는 제거
        if (images.Count > weaponIdx)
        {
            images.GetRange(weaponIdx, images.Count - weaponIdx).ForEach(i => Destroy(i.gameObject));
            images.RemoveRange(weaponIdx, images.Count - weaponIdx);
            weaponTypes.RemoveRange(weaponIdx, images.Count - weaponIdx);
            isTransformed = true;
        }

        try
        {
            // 현재 사용 중인 무기 강조 효과
            if (currentWeaponIdx != playerShooter.CurrentWeaponSlotNumber || weaponTypes[currentWeaponIdx] != playerShooter.CurrentWeapon)
            {
                if (currentWeaponIdx < images.Count && images[currentWeaponIdx] != null)
                {
                    Color temp = images[currentWeaponIdx].color;
                    temp.a = 0.5f;
                    images[currentWeaponIdx].color = temp;
                    images[currentWeaponIdx].rectTransform.localScale = Vector3.one;
                }

                currentWeaponIdx = playerShooter.CurrentWeaponSlotNumber;
                Color temp1 = images[currentWeaponIdx].color;
                temp1.a = 1f;
                images[currentWeaponIdx].color = temp1;
                images[currentWeaponIdx].rectTransform.localScale = new Vector3(1.2f, 1.2f, 1f);
            }
        }
        catch (ArgumentOutOfRangeException)
        {

        }
        catch (NullReferenceException)
        {

        }
        

        // 새롭게 생성된 이미지가 없다면 즉시 종료
        if (!isTransformed)
            return;

        // 무기 이미지 배치
        float current = -((weaponIdx - 1) / 2f * weaponImagePlacementInterval); // 시작 배치 위치
        weaponPanel.rectTransform.sizeDelta = new Vector2((Mathf.Abs(current) + 100) * 2, weaponPanel.rectTransform.sizeDelta.y); // Weapon Panel의 크기 조정
        for (int i = 0; i < images.Count; i++)
        {
            // 무기 이미지 위치 조정
            images[i].rectTransform.anchoredPosition = new Vector2(current, 0f);
            current += weaponImagePlacementInterval;
        }

    }

    //for start scene Ui
    public void OnClickGameStartButton()
    {
        if (!isSetting)
        {
            //StartGame 구현 완료 후 수정: GameManager.Instance.StartGame(settingButton.Level);
            GameManager.Instance.LoadScene(GameManager.Instance.InGameStartSceneBuildIndex);
            GameObject.Find("Start Button").GetComponent<Button>().interactable = false;
        }
    }

    public void OnClickGameQuitButton()
    {
        if (!isSetting)
            Application.Quit(); // 어플리케이션 종료
    }
    public void OnClickGameSettingButton()
    {
        isSetting = true;
        GameObject.Find("Setting Canvas").transform.GetChild(0).gameObject.SetActive(true);
    }
}
