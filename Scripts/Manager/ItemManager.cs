using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    //[SerializeField] private SerializableDictionary<Item, float> items = new SerializableDictionary<Item, float>();
    [SerializeField] private List<ItemProperty> items = new List<ItemProperty>();

#if LEGACY
    [SerializeField] float coinPercent;
    [SerializeField] float potionPercent;
    [SerializeField] float InvinciblePercent;
    [SerializeField] float nonePercent;
#endif
    private float sum;

    [System.Serializable]
    private struct ItemProperty
    {
        public Item itemPrefab;
        public float weight;
        public SceneData<Item> data;
    }

    private void Awake()
    {
#if LEGACY
        float sum = coinPercent + potionPercent + nonePercent + InvinciblePercent;
        coinPercent = coinPercent / sum * 100;
        potionPercent = potionPercent / sum * 100;
        InvinciblePercent = InvinciblePercent / sum * 100;
        nonePercent = nonePercent / sum * 100;
#endif
        this.sum = items.Sum(i => i.weight);

        var enemies = FindObjectsOfType<Enemy>();
        foreach (var enemy in enemies)
        {
            enemy.OnDeath += () => OnEnemyDeath(enemy);
        }
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        float prob = UnityEngine.Random.Range(0f, 1f);
        float current_prob = 0f;
        foreach (var item in items)
        {
            current_prob += item.weight / this.sum;
            if (prob <= current_prob)
            {
                if (!(item.itemPrefab is null))
                {
                    var instantiated = Instantiate(item.itemPrefab, enemy.transform.position, Quaternion.identity);
                    try
                    {
                        item.data.TrySetValue(instantiated);
                    }
                    catch { }
                }
                return;
            }
        }
    }


#if LEGACY
    void Start()
    {
        GameObject[] temp = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] prefebs = Resources.LoadAll<GameObject>("Item/");


        foreach (var i in temp)
        {
            if (i.GetComponent<Entity>() != null)
            {
                i.GetComponent<Entity>().OnDeath +=
                    delegate ()
                    {
                        int n = Random.Range(0, (int)(coinPercent + potionPercent + InvinciblePercent + nonePercent));

                        if (n < coinPercent)
                            Instantiate(prefebs[0], i.transform.position, Quaternion.identity);
                        else if (n < (coinPercent + potionPercent))
                            Instantiate(prefebs[2], i.transform.position , Quaternion.identity);
                        else if (n < (coinPercent + potionPercent + InvinciblePercent))
                            Instantiate(prefebs[1], i.transform.position, Quaternion.identity);
                    };
            }
        }
    }
#endif
}
