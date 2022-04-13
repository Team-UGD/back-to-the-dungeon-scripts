using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SettingButtonUI : MonoBehaviour, IUiActiveCheck
{
    #region 변수초기화
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private SettingButton setting;
    [SerializeField] private Text hardRecord;
    [SerializeField] private Text easyRecord;
    [SerializeField]private Hero hero;
    public const string CancelButtonName = "Cancel";
    private bool isSettingButtonActive;
    private bool isHardMode = false;
    //private bool isHard = false;
    //private bool isEasy = false;
    private GameLevel clickedLevelBtn;

    private bool isStart = false;
    private GameLevel gameLevel;
    #endregion

    #region 프로퍼티
    public bool IsUIActive { get => this.isSettingButtonActive; }
    ///<summery>
    ///창을 열때, 초기화를 위해 창을 열었다는 정보를 SettingButton으로부터 받아온다.
    ///</summery>
    public bool IsStart
    {
        set { isStart = value; }
    }
    ///<summery>
    ///setting 후 레벨을 return합니다.
    ///</summery>
    public GameLevel SettingLevel
    {
        get { return gameLevel; }
    }
    #endregion

    #region 이벤트함수
    private void Start()
    {
        //Debug.Log(Application.persistentDataPath);
        //if (SettingDataController.Data.lastSelectedLevel == GameLevel.Hard)
        //{
        //    isHardMode = true;
        //}
        //else
        //{
        //    isHardMode = false;
        //}
        clickedLevelBtn = SettingDataController.Data?.lastSelectedLevel ?? GameLevel.Easy;
        gameLevel = clickedLevelBtn;
        this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }
    private void Update()
    {
        bool cancel = Input.GetButtonDown(CancelButtonName);
        if (isSettingButtonActive && cancel)
        {
            CloseSettingUI();
            isSettingButtonActive = false;
        }
        //Setting 창을 열었다면 초기화를 한번만 시켜준다.
        if (isStart)
        {
            RecordActive();
            ResetFunction();
            isStart = false;
            isSettingButtonActive = true;
            //UIManager.Instance.IsOtherUIActive = true; SettingButtonUI에는 필요없음!
        }
    }
    #endregion

    #region UI이벤트

    private void CloseSettingUI()
    {
        setting.IsSettingActive = false;
        this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    //easy가 true가 된다면,
    public void Function_Toggle_Easy(bool b)
    {
        //isHard = false;
        //isEasy = true;
        clickedLevelBtn = GameLevel.Easy;
    }
    //Hard가 true가 된다면,
    public void Function_Toggle_Hard(bool b)
    {
        //isHard = true;
        //isEasy = false;
        clickedLevelBtn = GameLevel.Hard;
    }
    //Back 버튼을 눌렀다면,
    public void OnBackButtonClicked()
    {
        CloseSettingUI();
    }
    //OK버튼을 눌렀을 때, 정보를 저장해준다.
    public void OnOkButtonClicked()
    {
        //if (isHard && !isEasy) 
        //{
        //    isHardMode = true;
        //    gameLevel = GameLevel.Hard;
        //}
        //else 
        //{
        //    isHardMode = false;
        //    gameLevel = GameLevel.Easy;
        //}
        gameLevel = clickedLevelBtn;

        CloseSettingUI();
    }

    public void OnDeleteBtnClick()
    {
        GameManager.DeleteGameData(clickedLevelBtn);

        using (var _ = new SaveManager.TemporarySaveScope("gamedata_temp", SaveKey.GameData))
        {
            switch (clickedLevelBtn)
            {
                case GameLevel.Easy:
                    Level_Information(easyRecord, clickedLevelBtn);
                    break;
                case GameLevel.Hard:
                    Level_Information(hardRecord, clickedLevelBtn);
                    break;
                default:
                    break;
            }
        }
    }

    #endregion

    #region 초기화부분
    private void ResetFunction()
    {
        var toggles = toggleGroup.GetComponentsInChildren<Toggle>();
        //Debug.Log($"IsHardMode = {isHardMode}");
        //if (isHardMode)
        //{
        //    //Debug.Log($"Hard");
        //    toggles[0].isOn = true;
        //}
        if (gameLevel == GameLevel.Hard)
        {
            toggles[0].isOn = true;
        }
        else
        {
            //Debug.Log($"Easy");
            toggles[1].isOn = true;
        }
    }
    private void RecordActive()
    {
        if (!hardRecord || !easyRecord)
        {
            return;
        }

        hardRecord.text = "";
        easyRecord.text = "";

        using (var temp = new SaveManager.TemporarySaveScope("gamedata_temp", SaveKey.GameData))
        {
            Level_Information(hardRecord, GameLevel.Hard);
            Level_Information(easyRecord, GameLevel.Easy);
        }
    }
    public void Level_Information(Text levelText, GameLevel level)
    {
        levelText.text = "\n";
        GameManager.Instance.Level = level;
        if (GameManager.Instance.LoadGameData())
        {
            levelText.text += $"[Your Information]\n\n";
            levelText.text += $"Gold : {GameManager.Instance.Gold.ToString()}\n";
            levelText.text += $"Health: {hero.Health.ToString()}\n";
            levelText.text += $"Stamina: {hero.MaxStamina.ToString()}\n";
            levelText.text += $"Death: {GameManager.Instance.DeathCount}\n";
            var record = GameManager.Instance.ClearRecords;
            int cnt = 0;
            //최고 기록 출력
            foreach (var item in record)
            {
                cnt++;
            }
            levelText.text += "\n";
            levelText.text += $"[Your Best Record]\n";
            if (cnt > 0)
            {
                levelText.text += $"Date : {record.GetEnumerator().Current.Date.Year} years {record.GetEnumerator().Current.Date.Month} months {record.GetEnumerator().Current.Date.Day} days\n";
                levelText.text += $"         {record.GetEnumerator().Current.Date.Hour} hours {record.GetEnumerator().Current.Date.Minute} minutes {record.GetEnumerator().Current.Date.Second} seconds\n";
                var time = TimeSpan.FromSeconds(record.GetEnumerator().Current.ClearTime);
                levelText.text += $"Clear Time : {(time.Hours == 0 ? string.Empty : $"{time.Hours} hours ")}{(time.Minutes == 0 ? string.Empty : $"{time.Minutes} minutes ")}{$"{time.Seconds} seconds "}\n";
            }
            else
            {
                levelText.text += $"\n There's no best record.";
            }
        }
        else
        {
            levelText.text += "\n\n\n";
            levelText.text += $"There's no {level.ToString().ToLower()} level data. \n\n";
        }
    }
    #endregion
}
