using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RandomSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField, MoveTool] private Vector2 start;
    [SerializeField, MoveTool] private Vector2 end;
    [SerializeField, Min(0)] private int count;
    [SerializeField] private float coolDown;

    public Rect Area { get => new Rect(start, end - start); }

    private float time;

    private void Update()
    {
        if (Time.time < time + coolDown)
            return;

        for (int i = 0; i < count; i++)
        {
            Instantiate(prefab, new Vector3(Random.Range(start.x, end.x), Random.Range(start.y, end.y)), Quaternion.identity);
        }
        time = Time.time;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.DrawSolidRectangleWithOutline(this.Area, Color.clear, Color.cyan);
    }
#endif
}