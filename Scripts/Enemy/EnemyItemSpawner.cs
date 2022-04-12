using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyItemSpawner : MonoBehaviour
{
    [SerializeField, MoveTool(LocalMode = true, Label = "아이템 생성 위치")] private Vector2 creationPosition;
    [SerializeField] private List<ItemProperty> items = new List<ItemProperty>();

    [System.Serializable]
    private struct ItemProperty
    {
        public Item itemPrefab;
        [Min(0)] public int minCount;
        [Min(0)] public int maxCount;
        [Range(0f, 1f)] public float probability;
        public SceneData<Item> data;

        public int Count { get => Random.Range(minCount, maxCount + 1); }
    }

    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        enemy.OnDeath += OnEnemyDeath;
    }

    private void OnEnemyDeath()
    {
        if (!enabled)
            return;

        foreach (var item in items)
        {
            if (item.probability <= 0f)
                continue;

            if (Random.Range(0f, 1f) <= item.probability)
            {
                if (!(item.itemPrefab is null))
                {
                    for (int i = 0; i < item.Count; i++)
                    {
                        var instantiated = Instantiate(item.itemPrefab, (Vector2)transform.position + creationPosition + new Vector2(Random.Range(-0.1f, 0.1f), 0), Quaternion.identity);
                        try
                        {
                            item.data.TrySetValue(instantiated);
                        }
                        catch { }
                    }
                }
            }
        }
    }
}
