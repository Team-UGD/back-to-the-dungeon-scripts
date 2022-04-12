using System;
using UnityEngine;

public sealed class SettingDataController : ISaveable
{
    private static event Action<SettingData> onSettingDataLoaded;
    private static SettingData data;
    private static readonly SettingDataController instance = new SettingDataController();
    public const string SettingFileName = "setting";

    /// <summary>
    /// Setting Data가 정상적으로 로드 될 시 발동되는 이벤트
    /// </summary>
    public static event Action<SettingData> OnSettingDataLoaded { add => onSettingDataLoaded += value; remove => onSettingDataLoaded -= value; }

    public static void SaveSettingData()
    {
        SaveManager.Save(SettingFileName, SaveKey.Setting);
    }

    public static bool LoadSettingData()
    {
        try
        {
            SaveManager.Load(SettingFileName, SaveKey.Setting);
            return true;
        }
        catch (System.IO.FileNotFoundException)
        {
            Debug.Log($"<{nameof(SettingDataController)}> Setting File이 존재하지 않습니다.");
            return false;
        }
    }

    /// <summary>
    /// 마지막으로 로드된 Setting Data
    /// </summary>
    public static SettingData Data { get => data; }

    string ISaveable.ID => null;

    void ISaveable.Load(object loaded)
    {
        if (loaded is SettingData data)
        {
            SettingDataController.data = data;
            onSettingDataLoaded?.Invoke(data);

        }
    }

    object ISaveable.Save() => new SettingData();

    private SettingDataController()
    {
        // SaveManager에 이 listener를 등록하는 처리
        data = new SettingData();
        SaveManager.Add(this, SaveKey.Setting);
    }
}
