#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomPropertyDrawer(typeof(ScenePopupAttribute))]
public class ScenePopupDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = attribute as ScenePopupAttribute;
        if (attr == null)
            return;

        var scenes = EditorBuildSettings.scenes.Where(s => attr.onlyEnabled ? s.enabled : true);
        var sceneNames = scenes.Select(s => Path.GetFileNameWithoutExtension(s.path)).ToList();
        var sceneIndexes = scenes.Select(s => SceneUtility.GetBuildIndexByScenePath(s.path)).ToList();

        int selected = 0;
        switch (property.propertyType)
        {
            case SerializedPropertyType.String:
                selected = sceneNames.IndexOf(property.stringValue);
                break;
            case SerializedPropertyType.Integer:
                selected = sceneIndexes.IndexOf(property.intValue);
                break;
            default:
                EditorGUI.PropertyField(position, property, label, true);
                return;
        }

        if (selected < 0)
            selected = 0;

        GUIContent[] popupLabels = new GUIContent[sceneNames.Count];
        for (int i = 0; i < popupLabels.Length; i++)
        {
            string text = sceneNames[i];
            if (attr.index)
            {
                text += $"{(string.IsNullOrEmpty(text) ? string.Empty : " - ")}[{sceneIndexes[i]}]";
            }
            popupLabels[i] = new GUIContent(text);
        }

        selected = EditorGUI.Popup(position, label, selected, popupLabels);
        switch (property.propertyType)
        {
            case SerializedPropertyType.String:
                property.stringValue = sceneNames[selected];
                break;
            case SerializedPropertyType.Integer:
                property.intValue = sceneIndexes[selected];
                break;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label);
    }
}
#endif