using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Unity Inspector에서 Editing 가능한 Dictionary
/// </summary>
[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys;
    [SerializeField] private List<TValue> values;

    public SerializableDictionary()
    {
        keys = new List<TKey>();
        values = new List<TValue>();
        SyncInspectorFromDictionary();
    }

    /// <summary>
    /// 새로운 KeyValuePair을 추가하며, 인스펙터도 업데이트
    /// </summary>
    public new void Add(TKey key, TValue value)
    {
        base.Add(key, value);
        SyncInspectorFromDictionary();
    }

    /// <summary>
    /// KeyValuePair을 삭제하며, 인스펙터도 업데이트
    /// </summary>
    public new void Remove(TKey key)
    {
        base.Remove(key);
        SyncInspectorFromDictionary();
    }

    public void OnBeforeSerialize()
    {
    }

    /// <summary>
    /// 인스펙터를 딕셔너리로 초기화
    /// </summary>
    public void SyncInspectorFromDictionary()
    {
        //인스펙터 키 밸류 리스트 초기화
        keys.Clear();
        values.Clear();

        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    /// <summary>
    /// 딕셔너리를 인스펙터로 초기화
    /// </summary>
    public void SyncDictionaryFromInspector()
    {
        //딕셔너리 키 밸류 리스트 초기화
        foreach (var key in Keys.ToList())
        {
            base.Remove(key);
        }

        for (int i = 0; i < keys.Count; i++)
        {
            try
            {
                //중복된 키가 있다면 에러 출력
                if (this.ContainsKey(keys[i]))
                {
                    Debug.LogError("중복된 키가 있습니다.");
                    break;
                }
                base.Add(keys[i], values[i]);
            }
            catch (System.ArgumentNullException)
            {
                Debug.LogError($"{nameof(System.ArgumentNullException)}: Key of SerializableDictionary cannot be null.");
            }
        }
    }

    public void OnAfterDeserialize()
    {
        //Debug.Log(this + string.Format("인스펙터 키 수 : {0} 값 수 : {1}", keys.Count, values.Count));

        //인스펙터의 Key Value가 KeyValuePair 형태를 띌 경우
        if (keys.Count == values.Count)
        {
            SyncDictionaryFromInspector();
        }
    }
}