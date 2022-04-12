using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 세이브 시스템
/// </summary>
public static class SaveSystem
{
    private static string playerDataPath = "/player_data";
    private static string playerWeaponPath = "/player_weapon";
    private static string gameManagerDataPath = "/gamemanager_data";
    private static string gameDataPath = "/gamedata";

    /// <summary>
    /// 플레이어 데이터를 저장한다.
    /// </summary>
    [Obsolete]
    public static void SavePlayerData(PlayerData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + playerDataPath;
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    /// <summary>
    /// 플레이어 데이터를 로드한다. 반드시 예외처리를 하시기 바랍니다.
    /// </summary>
    /// <returns>플레이어 데이터</returns>
    /// <exception cref="FileNotFoundException">플레이어 데이터 파일이 존재하지 않을 경우</exception>
    [Obsolete]
    public static PlayerData LoadPlayerData()
    {
        string path = Application.persistentDataPath + playerDataPath;
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"플레이어 데이터 파일이 존재하지 않습니다.", path);
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);

        PlayerData data = formatter.Deserialize(stream) as PlayerData;
        stream.Close();

        return data;
    }

    /// <summary>
    /// 게임 매니저 데이터를 저장한다.
    /// </summary>
    [Obsolete]
    public static void SaveGameManagerData(GameManagerData data)
    {
        string path = Application.persistentDataPath + gameManagerDataPath;        
        using (var fs = new FileStream(path, FileMode.Create))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, data);
        }
    }

    /// <summary>
    /// 게임 매니저 데이터를 로드한다.
    /// </summary>
    /// <exception cref="FileNotFoundException">게임 매니저 데이터 파일이 존재하지 않을 경우</exception>
    [Obsolete]
    public static GameManagerData LoadGameManagerData()
    {
        string path = Application.persistentDataPath + gameManagerDataPath;
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"게임 매니저 데이터 파일이 존재하지 않습니다.", path);
        }

        using (var fs = new FileStream(path, FileMode.Open))
        {
            var bf = new BinaryFormatter();
            var data = bf.Deserialize(fs) as GameManagerData;
            return data;
        }
    }

    /// <summary>
    /// 플레이어 무기 데이터를 저장한다.
    /// </summary>
    [Obsolete]
    public static void SavePlayerWeapon(PlayerWeaponData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + playerWeaponPath;
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    /// <summary>
    /// 플레이어 무기 정보를 반환한다. 반드시 예외처리를 하시기 바랍니다.
    /// </summary>
    /// <returns>플레이어 무기 데이터</returns>
    /// <exception cref="FileNotFoundException">플레이어 무기 데이터 파일이 존재하지 않을 경우</exception>
    [Obsolete]
    public static PlayerWeaponData LoadPlayerWeapon()
    {
        string path = Application.persistentDataPath + playerWeaponPath;
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"플레이어 무기 파일이 존재하지 않습니다.", path);
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);

        PlayerWeaponData data = formatter.Deserialize(stream) as PlayerWeaponData;
        stream.Close();

        return data;
    }

    /// <summary>
    /// 게임 데이터를 저장.
    /// </summary>
    [Obsolete]
    public static void SaveGameData(GameData data)
    {
        string path = Application.persistentDataPath + gameDataPath;
        using (var fs = new FileStream(path, FileMode.Create))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, data);
        }
    }

    /// <summary>
    /// 게임 정보를 반환함.
    /// </summary>
    /// <returns>게임 데이터</returns>
    /// <exception cref="FileNotFoundException">게임 데이터 파일이 존재하지 않을 경우</exception>
    [Obsolete]
    public static GameData LoadGameData()
    {
        string path = Application.persistentDataPath + gameDataPath;
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"게임 데이터 파일이 존재하지 않습니다.", path);
        }

        using (var fs = new FileStream(path, FileMode.Open))
        {
            var bf = new BinaryFormatter();
            var data = bf.Deserialize(fs) as GameData;
            return data;
        }
    }
}

[Serializable]
public class GameData
{
    private PlayerData playerData;
    private GameManagerData gameManagerData;
    private PlayerWeaponData playerWeaponData;
    private StoreData storeData;

    public PlayerData Player { get => playerData; set => playerData = value; }
    public GameManagerData GameManager { get => gameManagerData; set => gameManagerData = value; }

    public PlayerWeaponData PlayerWeapon { get => playerWeaponData; set => playerWeaponData = value; }

    public StoreData StoreData { get => storeData; set => storeData = value; }
}

