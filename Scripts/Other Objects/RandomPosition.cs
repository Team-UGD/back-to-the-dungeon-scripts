using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RandomPosition : MonoBehaviour
{
    [SerializeField, MoveTool] private Vector2 start;
    [SerializeField, MoveTool] private Vector2 end;

    [SerializeField, Min(0f)] private float updateRate;
    [SerializeField] private Mode mode = Mode.Immediate;

    private float lastUpdateTime;
    private float t;

    private Vector2 last;
    private Vector2 goal;

    private bool moveEnabled = true;

    public bool IsLooping { get; set; } = true;
    public Vector2 Start { get => start; set => start = value; }
    public Vector2 End { get => end; set => end = value; }
    public Mode MoveMode { get => mode; set => mode = value; }

    public enum Mode
    {
        Immediate,
        Linear
    }

    public void MoveOneShot()
    {
        SetGoalPosition();
    }

    // Update is called once per frame
    public void Update()
    {
        if (IsLooping && Time.time >= lastUpdateTime + updateRate)
        {
            SetGoalPosition();
            lastUpdateTime = Time.time;
        }

        if (moveEnabled && mode == Mode.Linear)
        {
            t = Mathf.Clamp01(t + Time.deltaTime / updateRate);
            transform.position = Vector2.Lerp(last, goal, t);
            moveEnabled = Vector2.Distance(transform.position, goal) > 0.1f;
        }
    }

    private void SetGoalPosition()
    {
        moveEnabled = true;
        last = transform.position;
        goal = new Vector2(Random.Range(start.x, end.x), Random.Range(start.y, end.y));

        switch (mode)
        {
            case Mode.Immediate:
                transform.position = goal;
                break;
            case Mode.Linear:
                t = 0f;
                break;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // p0   p1
        // p2   p3
        Vector2 p0 = (Vector2)this.start;
        Vector2 p3 = (Vector2)this.end;
        Vector2 p1 = new Vector2(p3.x, p0.y);
        Vector2 p2 = new Vector2(p0.x, p3.y);

        Handles.color = Color.cyan;
        Handles.DrawLine(p0, p1);
        Handles.DrawLine(p1, p3);
        Handles.DrawLine(p3, p2);
        Handles.DrawLine(p2, p0);

        Handles.color = Color.green;
        Handles.DrawLine(last, goal);
        Handles.color = Color.white;
        Handles.Label(last, "Last");
        Handles.DrawSolidDisc(last, Vector3.forward, 0.1f);
        Handles.color = Color.red;
        Handles.Label(goal, "Goal");
        Handles.DrawSolidDisc(goal, Vector3.forward, 0.1f);
    }
#endif
}


//[CustomEditor(typeof(RandomPosition), true), CanEditMultipleObjects]
//public class RandomPositionEditor : Editor
//{
//    private MoveToolEditor moveToolEditor;
//    private RandomPosition m_target;

//    private FieldInfo start;
//    private FieldInfo end;
//    private FieldInfo last;
//    private FieldInfo goal;
//    private FieldInfo mode;

//    private void OnEnable()
//    {
//        m_target = target as RandomPosition;

//        var type = m_target.GetType();
//        var flag = BindingFlags.Instance | BindingFlags.NonPublic;
//        start = type.GetField(nameof(start), flag);
//        end = type.GetField(nameof(end), flag);
//        last = type.GetField(nameof(last), flag);
//        goal = type.GetField(nameof(goal), flag);
//        mode = type.GetField(nameof(mode), flag);
//    }

//    private void OnSceneGUI()
//    {

//    }

//    private void OnDisable()
//    {
//        DestroyImmediate(moveToolEditor);
//    }
//}