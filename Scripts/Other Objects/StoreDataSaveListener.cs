using System.Collections.Generic;
using UnityEngine;

internal class StoreDataSaveListener : ISaveable
{
    //public StoreData Data { get; private set; }

    private Dictionary<string, int> leftItemCount;
    private Dictionary<int, bool> canChangeItemCount;

    string ISaveable.ID => null;

    object ISaveable.Save() => new StoreData(leftItemCount, canChangeItemCount);

    void ISaveable.Load(object loaded)
    {
        if (loaded is StoreData data)
        {
            leftItemCount.Clear();
            foreach (var pair in data.LeftItemCounts)
            {
                leftItemCount[pair.Key] = pair.Value;
            }

            canChangeItemCount.Clear();
            foreach (var pair in data.CanChangeItemCount)
            {
                canChangeItemCount[pair.Key] = pair.Value;
                //Debug.Log($"<{nameof(Store)}> {pair.key}의 Store 아이템 개수 변경 가능: {pair.Value}");
            }

            Debug.Log($"<{nameof(StoreDataSaveListener)}> Store data load 성공.");
        }
        else
        {
            Debug.LogError($"<{nameof(StoreDataSaveListener)}> 잘못된 데이터가 입력되었습니다. 필요한 데이터: {typeof(StoreData)}. 입력된 데이터: {loaded?.GetType() ?? null}");
        }
    }

    /// <summary>
    /// StoreDataAgent 객체에 leftItemCount와 canChangeItemCount dictionary instance를 동기화 시킬 수 있도록 함.
    /// </summary>
    /// <param name="leftItemCount">동기화 될 남은 아이템 개수</param>
    /// <param name="canChangeItemCount">동기화 될 아이템 변경 가능 여부</param>
    public StoreDataSaveListener(Dictionary<string, int> leftItemCount, Dictionary<int, bool> canChangeItemCount)
    {
        this.leftItemCount = leftItemCount;
        this.canChangeItemCount = canChangeItemCount;

        // SaveManager에 자기 자신을 등록하는 처리
        SaveManager.Add(this, SaveKey.GameData);
    }

    public void CleanUp()
    {
        SaveManager.Remove(this, SaveKey.GameData);
    }
}

[System.Serializable]
public class StoreData
{
    private readonly Dictionary<string, int> leftItemCount;
    private readonly Dictionary<int, bool> canChangeItemCount;

    public IEnumerable<KeyValuePair<string, int>> LeftItemCounts
    {
        get
        {
            foreach (var pair in leftItemCount)
                yield return pair;
        }
    }

    public IEnumerable<KeyValuePair<int, bool>> CanChangeItemCount
    {
        get
        {
            foreach (var pair in canChangeItemCount)
                yield return pair;
        }
    }

    public StoreData(Dictionary<string, int> leftItemCount, Dictionary<int, bool> canChangeItemCount)
    {
        this.leftItemCount = leftItemCount;
        this.canChangeItemCount = canChangeItemCount;
    }
}