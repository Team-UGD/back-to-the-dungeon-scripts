#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// EnemyPathfinder 컴포넌트의 커스텀 인스펙터
/// </summary>
[CustomEditor(typeof(EnemyPathfinder)), CanEditMultipleObjects]
public class EnemyPathfinderEditor : Editor
{
    private EnemyPathfinder enemyPathfinder;

    private void OnEnable()
    {
        enemyPathfinder = target as EnemyPathfinder;
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        //enemyPathfinder.target = EditorGUILayout.ObjectField(ConvertToInspectorLabelName(nameof(enemyPathfinder.target)), enemyPathfinder.target, typeof(Transform), true) as Transform;

        #region Pathfinding

        //EditorGUILayout.Space();
        // Header
        EditorGUILayout.LabelField("Pathfinding", EditorStyles.boldLabel);

        // Contents
        //enemyPathfinder.canFindPath = EditorGUILayout.Toggle(ConvertToInspectorLabelName(nameof(enemyPathfinder.canFindPath)), enemyPathfinder.canFindPath);
        MakeInspectorField(ref enemyPathfinder.canFindPath, EditorGUILayout.Toggle, nameof(enemyPathfinder.canFindPath).InspectorLabel());
        if (enemyPathfinder.canFindPath)
        {
            EditorGUI.indentLevel++;
            MakeInspectorField(ref enemyPathfinder.repathRate, EditorGUILayout.FloatField, nameof(enemyPathfinder.repathRate).InspectorLabel());
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EnumPopup("Current Pathfinding State", enemyPathfinder.PathfindingState);

        #endregion

        #region Movement
        EditorGUILayout.Space();
        // Header
        EditorGUILayout.LabelField("Movement", EditorStyles.boldLabel);

        // Contents
        MakeInspectorField(ref enemyPathfinder.maxSpeed, EditorGUILayout.FloatField, nameof(enemyPathfinder.maxSpeed).InspectorLabel());
        //enemyPathfinder.maxSpeed = EditorGUILayout.FloatField(ConvertToInspectorLabelName(nameof(enemyPathfinder.maxSpeed)), enemyPathfinder.maxSpeed);
        MakeInspectorField(ref enemyPathfinder.canFly, EditorGUILayout.Toggle, nameof(enemyPathfinder.canFly).InspectorLabel());
        if (!enemyPathfinder.canFly)
        {
            MakeInspectorField(ref enemyPathfinder.canJump, EditorGUILayout.Toggle, nameof(enemyPathfinder.canJump).InspectorLabel());
            if (enemyPathfinder.canJump)
            {
                EditorGUI.indentLevel++;
                MakeInspectorField(ref enemyPathfinder.maxJumpWidth, EditorGUILayout.FloatField, nameof(enemyPathfinder.maxJumpWidth).InspectorLabel());
                MakeInspectorField(ref enemyPathfinder.jumpHeight, EditorGUILayout.FloatField, nameof(enemyPathfinder.jumpHeight).InspectorLabel());
                EditorGUI.indentLevel--;
            }
        }

        MakeInspectorField(ref enemyPathfinder.timeToReachMaxSpeed, EditorGUILayout.FloatField, nameof(enemyPathfinder.timeToReachMaxSpeed).InspectorLabel());
        MakeInspectorField(ref enemyPathfinder.pickNextWaypointDistance, EditorGUILayout.FloatField, nameof(enemyPathfinder.pickNextWaypointDistance).InspectorLabel());
        MakeInspectorField(ref enemyPathfinder.slowDownDistance, EditorGUILayout.FloatField, nameof(enemyPathfinder.slowDownDistance).InspectorLabel());
        //enemyPathfinder.slowDownDistance = EditorGUILayout.Slider(ConvertToInspectorLabelName(nameof(enemyPathfinder.slowDownDistance)), enemyPathfinder.slowDownDistance, 0.1f, 10f);
        MakeInspectorField(ref enemyPathfinder.endReachedDistance, EditorGUILayout.FloatField, nameof(enemyPathfinder.endReachedDistance).InspectorLabel());
        MakeInspectorField(ref enemyPathfinder.gravitySetting, (l, g, o) => (EnemyPathfinder.GravityType)EditorGUILayout.EnumPopup(l, g, o), nameof(enemyPathfinder.gravitySetting).InspectorLabel());

        #endregion

        #region Settings
        EditorGUILayout.Space();
        // Header
        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);

        // Contents
        MakeInspectorField(ref enemyPathfinder.alwaysDrawGizmos, EditorGUILayout.Toggle, nameof(enemyPathfinder.alwaysDrawGizmos).InspectorLabel());

        EditorGUI.BeginChangeCheck();
        LayerMask tempMask = EditorGUILayout.MaskField(nameof(enemyPathfinder.obstacleLayer).InspectorLabel(), InternalEditorUtility.LayerMaskToConcatenatedLayersMask(enemyPathfinder.obstacleLayer), InternalEditorUtility.layers);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(enemyPathfinder, $"Change {nameof(enemyPathfinder.obstacleLayer).InspectorLabel()}");
            enemyPathfinder.obstacleLayer = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);
            EditorUtility.SetDirty(enemyPathfinder);
        }

        #endregion
    }

    private void MakeInspectorField<T>(ref T value, Func<string, T, GUILayoutOption[], T> inspectorFieldCreator, string label, params GUILayoutOption[] options)
    {
        EditorGUI.BeginChangeCheck();
        T temp = inspectorFieldCreator(label, value, options);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(enemyPathfinder, $"{enemyPathfinder.GetInstanceID()}_{label}_{temp}");
            value = temp;
            //EditorUtility.SetDirty(enemyPathfinder);
            PrefabUtility.RecordPrefabInstancePropertyModifications(enemyPathfinder);
        }
    }
}
#endif