using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System;

#if UNITY_EDITOR
[CustomEditor(typeof(BezierMoveTool)), CanEditMultipleObjects]
public class BezierMoveToolEditor : Editor
{
    private BezierMoveTool m_target;
    private MoveToolEditor moveToolEditor;

    private SerializedProperty segmentCount;
    private SerializedProperty bezierPath;
    private SerializedProperty mode;
    private SerializedProperty runProperty;
    private SerializedProperty shift;
    private SerializedProperty shiftedPathIndex;

    private GUIContent shiftedPathIndexGUI;

    private void OnEnable()
    {
        m_target = target as BezierMoveTool;

        segmentCount = serializedObject.FindProperty(nameof(segmentCount));
        bezierPath = serializedObject.FindProperty(nameof(bezierPath));
        mode = serializedObject.FindProperty(nameof(mode));
        runProperty = serializedObject.FindProperty(nameof(runProperty));
        shift = serializedObject.FindProperty(nameof(shift));
        shiftedPathIndex = serializedObject.FindProperty(nameof(shiftedPathIndex));

        shiftedPathIndexGUI = new GUIContent("Index");
    }

    private void OnSceneGUI()
    {
        if (moveToolEditor == null)
            moveToolEditor = Editor.CreateEditor(target, typeof(MoveToolEditor)) as MoveToolEditor;

        moveToolEditor.OnEnable();
        moveToolEditor.OnSceneGUI();
        //moveToolEditor.SetMoveTool();

        for (int i = 0; i < m_target.Count; i++)
            SetLines(m_target[i].point, m_target.SegmentCount);
    }
    private void SetLines(List<Vector2> path, int segmentCount)
    {
        if (path.Count < 2)
            return;

        Handles.color = Color.white;
        for (int i = 0; i < path.Count - 1; i++)
        {
            Handles.DrawLine(path[i], path[i + 1]);
        }
        Handles.color = Color.cyan;
        var f = PhysicsUtility.BezierInterpolation(path);
        for (float i = 0; i < segmentCount; i++)
        {
            float value_before = i / segmentCount;
            Vector2 before = f(value_before);

            float value_after = (i + 1) / segmentCount;
            Vector2 after = f(value_after);

            Handles.DrawLine(before, after);
        }
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        // Path Settings
        EditorGUILayout.PropertyField(segmentCount);
        EditorGUILayout.PropertyField(bezierPath, true);

        EditorGUILayout.Space(10f);
        this.DrawLine();

        // Path Shift
        EditorGUILayout.PropertyField(shift);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(shiftedPathIndex, shiftedPathIndexGUI, GUILayout.MinWidth(220f));

        EditorGUILayout.Space(10f);
        if (GUILayout.Button("Shift Path", GUILayout.MinWidth(100f)))
        {
            int idx = shiftedPathIndex.intValue;
            if (idx < bezierPath.arraySize)
            {
                var path = bezierPath.GetArrayElementAtIndex(idx);
                var points = path.FindPropertyRelative("point");
                Vector2 shifted = this.shift.vector2Value;
                for (int i = 0; i < points.arraySize; i++)
                {
                    var p = points.GetArrayElementAtIndex(i);
                    p.vector2Value += shifted;
                }
            }
            else
            {
                Debug.LogError("Path Shift Settings의 Index 항목의 값이 Path의 인덱스 범위를 벗어나 이동 할 수 없습니다.", m_target);
            }
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("You can shift the path matching the index.\nPress \"Shift Path\" button.", MessageType.Info);

        EditorGUILayout.Space(10f);
        this.DrawLine();

        // Run Settings
        EditorGUILayout.PropertyField(mode);
        EditorGUILayout.PropertyField(runProperty, true);

        EditorGUILayout.Space();
        if (GUILayout.Button("Run") && Application.isPlaying)
        {
            m_target.GetType().GetMethod("Run", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(m_target, new object[] { true });
        }

        EditorGUILayout.HelpBox("Note that \"Run\" button only works during game mode.", MessageType.Warning);
        serializedObject.ApplyModifiedProperties();
    }

    private void OnDisable()
    {
        DestroyImmediate(moveToolEditor);
    }

    private void DrawLine()
    {
        var rect = EditorGUILayout.BeginHorizontal();
        Handles.color = Color.gray;
        Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
        EditorGUILayout.EndHorizontal();
    }
}
#endif