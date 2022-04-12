using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Scene과 연동되는 아이템 Value 데이터 Dictionary
/// </summary>
/// <typeparam name="TObject">오브젝트 타입</typeparam>
/// <typeparam name="TValue">아이템의 Value 타입</typeparam>
public abstract class SceneData<TObject, TValue> : SceneData<TObject>, IDictionary<int, TValue>, ISerializationCallbackReceiver where TObject : MonoBehaviour
{
    #region === Private Fields ===

    private Dictionary<int, TValue> indexDict = new Dictionary<int, TValue>();

    #endregion

    #region === Serialized Fields ===

    [Header("Item Scene Settings")]
    [SerializeField, Tooltip("IObjectValue<T> interface의 상속을 받은 컴포넌트 Instance만 할당할 수 있습니다.")] private TObject prefabObject;
    [SerializeField, ScenePopup] private List<int> scenes = new List<int>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    #endregion

    #region === Non-Private Members ===

    /// <summary>
    /// IObjectValue&#60;<typeparamref name="TValue"/>&#62;의 상속을 받은 Prefab 오브젝트
    /// </summary>
    public IObjectValue<TValue> PrefabObject
    {
        get => prefabObject as IObjectValue<TValue>;
    }

    public override bool TrySetValue(TObject instance, int buildIndex)
    {
        try
        {
            if (instance.GetType() == this.prefabObject.GetType())
            {
                (instance as IObjectValue<TValue>).Value = this[buildIndex];
                return true;
            }
        }
        catch
        {
            return false;
        }

        return false;
    }

    /// <summary>
    /// 직렬화되는 Values 항목의 각 값에 대한 속성을 설정한다.
    /// </summary>
    /// <param name="value">Values 항목의 현재 값</param>
    /// <param name="index">Values 항목의 현재 인덱스</param>
    protected virtual TValue SetDataProperty(TValue value, int index)
    {
        return value;
    }

    #endregion

    #region === Dictionary ===

    public ICollection<int> Keys => ((IDictionary<int, TValue>)indexDict).Keys;

    public ICollection<TValue> Values => ((IDictionary<int, TValue>)indexDict).Values;

    public int Count => ((ICollection<KeyValuePair<int, TValue>>)indexDict).Count;

    public bool IsReadOnly => ((ICollection<KeyValuePair<int, TValue>>)indexDict).IsReadOnly;

    public TValue this[int key]
    {
        get => ((IDictionary<int, TValue>)indexDict)[key];
        set
        {
            ((IDictionary<int, TValue>)indexDict)[key] = value;
            SyncFromDictionary();
        }
    }


    public void Add(int key, TValue value)
    {
        ((IDictionary<int, TValue>)indexDict).Add(key, value);
        SyncFromDictionary();
    }

    public bool ContainsKey(int key)
    {
        return ((IDictionary<int, TValue>)indexDict).ContainsKey(key);
    }

    public bool Remove(int key)
    {
        var result = ((IDictionary<int, TValue>)indexDict).Remove(key);
        SyncFromDictionary();
        return result;
    }

    public bool TryGetValue(int key, out TValue value)
    {
        return ((IDictionary<int, TValue>)indexDict).TryGetValue(key, out value);
    }

    public void Add(KeyValuePair<int, TValue> item)
    {
        ((ICollection<KeyValuePair<int, TValue>>)indexDict).Add(item);
        SyncFromDictionary();
    }

    public void Clear()
    {
        ((ICollection<KeyValuePair<int, TValue>>)indexDict).Clear();
        SyncFromDictionary();
    }

    public bool Contains(KeyValuePair<int, TValue> item)
    {
        return ((ICollection<KeyValuePair<int, TValue>>)indexDict).Contains(item);
    }

    public void CopyTo(KeyValuePair<int, TValue>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<int, TValue>>)indexDict).CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<int, TValue> item)
    {
        var result = ((ICollection<KeyValuePair<int, TValue>>)indexDict).Remove(item);
        SyncFromDictionary();
        return result;
    }

    public IEnumerator<KeyValuePair<int, TValue>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<int, TValue>>)indexDict).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)indexDict).GetEnumerator();
    }

    #endregion

    #region === Serialize ===

    public void OnBeforeSerialize()
    {
        //SyncFromInsepctor();
    }

    public void OnAfterDeserialize()
    {
        SyncFromInsepctor();

        if (!(prefabObject is IObjectValue<TValue>))
            prefabObject = null;
    }

    private void SyncFromInsepctor()
    {
        // Scenes와 Values의 항목 수를 동일하게 함
        if (scenes.Count > values.Count)
        {
            while (values.Count != scenes.Count)
            {
                values.Add(default(TValue));
            }
        }
        else if (scenes.Count < values.Count)
        {
            while (values.Count != scenes.Count)
            {
                values.RemoveAt(values.Count - 1);
            }
        }

        // Values의 속성을 적용함.
        for (int i = 0; i < values.Count; i++)
        {
            values[i] = this.SetDataProperty(values[i], i);
        }

        // Dictionary와 동기화
        indexDict.Clear();
        for (int i = 0; i < scenes.Count; i++)
        {
            indexDict[scenes[i]] = values[i];
        }
    }

    private void SyncFromDictionary()
    {
        scenes.Clear();
        values.Clear();
        var keys = indexDict.Keys.ToArray();

        foreach (var key in keys)
        {
            scenes.Add(key);
            values.Add(indexDict[key]);
        }
    }

    #endregion
}

/// <summary>
/// Object에 대한 Value 프로퍼티를 정의한 인터페이스
/// </summary>
/// <typeparam name="T">Item Value Type</typeparam>
public interface IObjectValue<T>
{
    public T Value { get; set; }
}

/// <summary>
/// Abstract class about each object data in each scene 
/// </summary>
/// <typeparam name="T">Component Type</typeparam>
public abstract class SceneData<T> : ScriptableObject where T : MonoBehaviour
{
    /// <summary>
    /// 현재 scene에 대응하는 컴포넌트의 value를 설정한다.
    /// </summary>
    /// <param name="instance">인스턴스화된 컴포넌트</param>
    /// <returns>성공적으로 값을 설정했는지 여부</returns>
    public bool TrySetValue(T instance)
    {
        try
        {
            return this.TrySetValue(instance, instance.gameObject.scene.buildIndex);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// build index에 해당하는 scene에 대응하는 컴포넌트의 value를 설정한다.
    /// </summary>
    /// <param name="instance">인스턴스화된 컴포넌트</param>
    /// <param name="buildIndex">Scene의 빌드인덱스</param>
    /// <returns>성공적으로 값을 설정했는지 여부</returns>
    public abstract bool TrySetValue(T instance, int buildIndex);
}