using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public sealed class GameObjectGenerator : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField, MoveTool] private CreationOption creationOption = new CreationOption();
    [SerializeField] private DestroyOption destroyOption;

    public GameObject Prefab
    {
        set
        {
            if (value.scene.name != null)
                throw new ArgumentException("Prefab이 아닌 오브젝트는 할당할 수 없습니다.");

            this.prefab = value;
        }
    }

    [Serializable, MoveToolAvailable]
    private class CreationOption
    {
        public Vector2 start;
        public Vector2 directed;
        [Min(0)] public int count;
        [Min(0f)] public float timeInterval;
        [Min(0f)] public float distanceInterval;
    }

    [Serializable]
    private class DestroyOption
    {
        public bool isDestory;
        public bool startToEnd = true;
        [Min(0f)] public float destroyTime;
        [Min(0f)] public float timeInterval;
    }

    public void Run()
    {
        StartCoroutine(CreateObjects());
    }

    private IEnumerator CreateObjects()
    {
        Vector2 current = creationOption.start;
        Vector2 direction = (creationOption.directed - creationOption.start).normalized;

        if (destroyOption.startToEnd)
        {
            Queue<GameObject> queue = new Queue<GameObject>();

            for (int i = 0; i < creationOption.count; i++)
            {
                queue.Enqueue(Instantiate(prefab, current, Quaternion.identity));
                current += creationOption.distanceInterval * direction;
                if (creationOption.timeInterval > 0)
                    yield return new WaitForSeconds(creationOption.timeInterval);
            }

            if (destroyOption.isDestory)
            {
                while (queue.Count != 0)
                {
                    if (destroyOption.timeInterval > 0)
                        yield return new WaitForSeconds(destroyOption.timeInterval);

                    if (destroyOption.startToEnd)
                        Destroy(queue.Dequeue(), destroyOption.destroyTime);

                }
            }
        }
        else
        {
            Stack<GameObject> stack = new Stack<GameObject>();

            for (int i = 0; i < creationOption.count; i++)
            {
                stack.Push(Instantiate(prefab, current, Quaternion.identity));
                current += creationOption.distanceInterval * direction;
                if (creationOption.timeInterval > 0)
                    yield return new WaitForSeconds(creationOption.timeInterval);
            }

            if (destroyOption.isDestory)
            {
                while (stack.Count != 0)
                {
                    if (destroyOption.timeInterval > 0)
                        yield return new WaitForSeconds(destroyOption.timeInterval);

                    if (destroyOption.startToEnd)
                        Destroy(stack.Pop(), destroyOption.destroyTime);

                }
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (Vector2.Distance(creationOption.start, creationOption.directed) >= 0.1f)
        {
            Handles.color = Color.cyan;
            Handles.DrawLine(creationOption.start, creationOption.directed, 1f);
        }
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameObjectGenerator)), CanEditMultipleObjects]
public sealed class GameObjectGeneratorEditor : Editor
{
    private GameObjectGenerator m_target;
    private SerializedProperty start;
    private SerializedProperty directed;

    private MoveToolEditor moveToolEditor;

    private void OnEnable()
    {
        m_target = target as GameObjectGenerator;
        var creation = serializedObject.FindProperty("creationOption");
        start = creation.FindPropertyRelative("start");
        directed = creation.FindPropertyRelative("directed");
    }

    private void OnSceneGUI()
    {
        if (moveToolEditor == null)
        {
            moveToolEditor = Editor.CreateEditor(m_target, typeof(MoveToolEditor)) as MoveToolEditor;
            moveToolEditor.OnEnable();
        }
        moveToolEditor.OnSceneGUI();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();

        EditorGUILayout.FloatField("Distance - Readonly", Vector2.Distance(start.vector2Value, directed.vector2Value));

        EditorGUILayout.Space();
        if (GUILayout.Button("Run"))
        {
            if (Application.isPlaying)
                m_target.Run();
        }
    }

    private void OnDisable()
    {
        DestroyImmediate(moveToolEditor);
    }
}
#endif