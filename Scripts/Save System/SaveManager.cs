using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager
{
    private static readonly Dictionary<SaveKey, List<ISaveable>> saveableObjects = new Dictionary<SaveKey, List<ISaveable>>();
    
    public static void Save(string fileName, SaveKey key)
    {
        List<SaveData> saveDatas = new List<SaveData>();

        if(saveableObjects.TryGetValue(key, out List<ISaveable> saveablelist))
        {
            // getData from listener
            foreach(ISaveable saveable in saveablelist)
                saveDatas.Add(new SaveData(saveable.Save(), saveable.GetType(), saveable.ID));

            string path = Application.persistentDataPath + "/" + fileName;
            using (var fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, saveDatas);
            }
        }
        else
        {
            throw new KeyNotFoundException($"saveableObjects[key]에 해당하는 List가 없습니다.");
        }
    }

    /// <summary>
    /// 등록된 saveablelist와 match되는 saveData가 없을경우 ArgumentNullException 발생!
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="key"></param>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="Exception"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static void Load(string fileName, SaveKey key)
    {
        string path = Application.persistentDataPath + "/" + fileName;

        if (!IsFileExistence(path))
        {
            throw new FileNotFoundException($"There is no DataFile.");
        }

        if(saveableObjects.TryGetValue(key, out List<ISaveable> saveablelist))
        {
            //load
            using (var fs = new FileStream(path, FileMode.Open))
            {
                var bf = new BinaryFormatter();
                var data = bf.Deserialize(fs) as List<SaveData>;

                foreach (ISaveable saveable in saveablelist)
                {
                    SaveData d = data.Find(e => e.listenerID == saveable.ID && e.listenerType == saveable.GetType());
                   
                    if(d == null)
                        throw new ArgumentNullException($"load 실퍠 TYPE 혹은 ID 불일치(SaveData와 match되는 Saveable Object가 없음) saveable.ID : {saveable.ID}, saveable.GetType() : {saveable.GetType()} ");

                    saveable.Load(d.data);
                }
            }
        }
        else
        {
            throw new KeyNotFoundException($"saveableObjects[key]에 해당하는 List가 없습니다.");
        }
    }

    /// <summary>
    /// Add saveable objece to saveableObject Dictionary
    /// </summary>
    /// <param name="saveable"></param>
    /// <param name="key"></param>
    public static void Add(ISaveable saveable, SaveKey key)//exception handle
    {
        if (saveableObjects.TryGetValue(key, out List<ISaveable> saveablelist))
            saveablelist.Add(saveable);
        else
            saveableObjects.Add(key, new List<ISaveable> { saveable });
    }

    /// <summary>
    /// remove saveable objece from saveableObject Dictionary. if remove fale return false
    /// </summary>
    /// <param name="saveable"></param>
    /// <param name="key"></param>
    public static bool Remove(ISaveable saveable, SaveKey key)
    {
        if (saveableObjects.TryGetValue(key, out List<ISaveable> saveablelist))
        {
            saveablelist.Remove(saveable);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// path : file path
    /// If the file exists return true, not exists return false
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool IsFileExistence(string path)
    {
        if (File.Exists(path))
            return true;
        return false;
    }
    public static void DeleteFile(string fileName)
    {
        File.Delete(Application.persistentDataPath + "/" + fileName);
        Debug.Log($"<{nameof(SaveManager)}> {fileName} 파일 삭제 완료.");
    }
    ///<summary>
    /// disposable 중첩 클래스
    /// </summary>
    public sealed class TemporarySaveScope : IDisposable
    {
        private bool isDispose = false;
        string fileNameTemp;
        SaveKey keyTemp;
        public TemporarySaveScope(string fileName, SaveKey key)
        {
            fileNameTemp = fileName;
            keyTemp = key;
            SaveManager.Save(fileName, key);
        }
        public void Dispose()
        {
            if (!this.isDispose)
            {
                SaveManager.Load(fileNameTemp, keyTemp);
                this.isDispose = true;
                DeleteFile(fileNameTemp);
            }
            GC.SuppressFinalize(this);
        }
    }
}

public enum SaveKey
{
    GameData,
    Setting
}

[Serializable]
public class SaveData
{
    public readonly object data;
    public readonly Type listenerType;
    public readonly string listenerID;

    public SaveData(object data, Type type, string id)
    {
        this.data = data;
        this.listenerType = type;
        this.listenerID = id;
    }
}