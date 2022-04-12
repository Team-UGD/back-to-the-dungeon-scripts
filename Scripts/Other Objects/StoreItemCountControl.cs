using UnityEngine;
using System.Collections.Generic;

public class StoreItemCountControl : MonoBehaviour
{
    [SerializeField] private Store store;
    [SerializeField] private SerializableDictionary<StoreItemSlot, int> itemCountChangeSetting = new SerializableDictionary<StoreItemSlot, int>();
    [SerializeField] private bool addMode = true;

    // Start is called before the first frame update
    private void Start()
    {
        try
        {
            if (addMode)
            {
                var temp = new Dictionary<StoreItemSlot, int>();
                foreach (var pair in itemCountChangeSetting)
                {
                    temp[pair.Key] = pair.Value + store[pair.Key];
                }
                store.SetLeftItemsCount(temp);
            }
            else
            {
                store.SetLeftItemsCount(itemCountChangeSetting);
            }
        }
        catch { }   
    }
}