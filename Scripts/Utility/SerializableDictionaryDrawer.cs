using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if LEGACY
[CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
public class SerializableDictionaryDrawer : PropertyDrawer
{
    private const float fieldHeight = 20f;
    private const float intervalHeight = 2f;
    private const float keyValuePairHeight = 30f;
    private const float widthWeight = 0.48f;
    private const float heightWeight = 0.6f;

    private GUIStyle currentStyle = null;
    private int selectedItemIndex;

    private Color defaultBoxColor = new Color32(65, 65, 65, 255);
    private Color selectedBoxColor = new Color32(44, 93, 135, 255);
    private Color selectedTextFieldColor = new Color32(68, 68, 68, 255);

    private float keyValuePairRectHeight;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {     
        var keys = property.FindPropertyRelative("keys");
        var values = property.FindPropertyRelative("values");

        position.height = fieldHeight;

        // Dictionary Title
        float countFieldWidth = 48f;
        EditorGUI.PropertyField(new Rect(position.x, position.y, position.width - countFieldWidth, position.height), property, label, false);

        // Dictionary 크기 설정
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            int count = EditorGUI.DelayedIntField(new Rect(position.x + position.width - countFieldWidth, position.y, countFieldWidth, position.height), keys.arraySize);
            if (check.changed)
            {
                count = Mathf.Max(0, count);
                keys.arraySize = count;
                values.arraySize = count;
            }
        }

        // Dictionary Element 설정
        if (property.isExpanded)
        {
            var keysRect = new Rect(position.x, position.y + fieldHeight, position.width * widthWeight, fieldHeight);
            var valuesRect = new Rect(position.x + position.width * widthWeight, position.y + fieldHeight, position.width * widthWeight, fieldHeight);

            using (var indent = new EditorGUI.IndentLevelScope(2))
            {
                EditorGUI.LabelField(keysRect, "Keys");
                EditorGUI.LabelField(valuesRect, "Values");
            }
            

            Rect keyValuePairRect = new Rect(keysRect.x, keysRect.y + fieldHeight + intervalHeight, position.width, keyValuePairHeight * keys.arraySize);
            KeyValuePairField(keys, values, keyValuePairRect);

            //current.width = position.width;
            Rect itemChangeRect = keyValuePairRect;
            itemChangeRect.y += keyValuePairRect.height;
            itemChangeRect.height = fieldHeight;
            this.selectedItemIndex = SetItemChangeButton(keys, values, this.selectedItemIndex, itemChangeRect);
            this.keyValuePairRectHeight = keyValuePairRect.height;

        }
    }

    private void KeyValuePairField(SerializedProperty keys, SerializedProperty values, Rect rect)
    {
        if (keys.arraySize == 0)
            return;

        Color color_default = GUI.backgroundColor;
        //GUIStyle style = new GUIStyle(GUI.skin.button);
        GUIStyle style = GetBtnStyle();
        style.active.background = style.normal.background;

        Rect current = rect;
        var height = rect.height / keys.arraySize;
        current.height = height;

        float width = rect.width;

        for (int i = 0; i < keys.arraySize; i++)
        {
            //GUI.backgroundColor = (selectedItemIndex == i) ? selectedBoxColor : defaultBoxColor;
            //if (GUI.Button(new Rect(current.x, current.y, current.width, current.height), GUIContent.none, style))
            //{
            //    this.selectedItemIndex = i;
            //}
            //GUI.backgroundColor = color_default;


            var key = keys.GetArrayElementAtIndex(i);
            var value = values.GetArrayElementAtIndex(i);

            using (var indent = new EditorGUI.IndentLevelScope(2))
            {
                Rect keyRect = new Rect(current.x, current.y + height * (1f - heightWeight) / 2f, current.width * widthWeight, height * heightWeight);
                var valueRect = new Rect(current.x + current.width * widthWeight, current.y + height * (1f - heightWeight) / 2f, current.width * widthWeight, height * heightWeight);

                EditorGUI.PropertyField(new Rect(current.x, current.y + height * (1f - heightWeight) / 2f, current.width * widthWeight, height * heightWeight), key, GUIContent.none, true);
                EditorGUI.PropertyField(new Rect(current.x + current.width * widthWeight, current.y + height * (1f - heightWeight) / 2f, current.width * widthWeight, height * heightWeight), value, GUIContent.none, true);
                using (var scroll = new GUI.ScrollViewScope(new Rect(valueRect.x, valueRect.y + height - 5f, valueRect.width, 5f), valueRect.position, valueRect, true, false))
                {
                    
                }
            }

            current.y += height;
        }

    }

    private GUIStyle GetBtnStyle()
    {
        var s = new GUIStyle(GUI.skin.label);
        Color[] pix = new Color[] { Color.white };
        Texture2D result = new Texture2D(1, 1);
        result.SetPixels(pix);
        result.Apply();
        s.normal.background = result;
        s.fontSize = 20;
        s.alignment = TextAnchor.MiddleLeft;
        s.margin = new RectOffset(0, 0, 0, 0);
        return s;
    }

    private int SetItemChangeButton(SerializedProperty keys, SerializedProperty values, int selectedIndex, Rect rect)
    {
        using (var indent = new EditorGUI.IndentLevelScope(1))
        {
            EditorGUI.LabelField(rect, "Item Change");

            Rect indentedRect = EditorGUI.IndentedRect(rect);
            indentedRect.y += fieldHeight;

            Rect current = indentedRect;

            current.width = indentedRect.width / 4f - 2f;
            Rect addRect = current;

            current.x += current.width + 2f;
            Rect removeRect = current;

            current.x += current.width;
            Rect indexLabelRect = current;

            current.x += current.width;
            current.width += 4f;
            Rect indexValueRect = current;

            // 삽입 버튼
            if (GUI.Button(addRect, "Add"))
            {
                if (selectedIndex <= keys.arraySize)
                {
                    keys.InsertArrayElementAtIndex(selectedIndex);
                    values.InsertArrayElementAtIndex(selectedIndex);
                }
                else
                {
                    var last = keys.arraySize;
                    keys.InsertArrayElementAtIndex(last);
                    values.InsertArrayElementAtIndex(last);
                }

            }

            // 제거 버튼
            if (GUI.Button(removeRect, "Remove"))
            {
                for (int i = selectedIndex; i < keys.arraySize; i++)
                {
                    keys.MoveArrayElement(i, i + 1);
                }

                keys.arraySize = Mathf.Max(keys.arraySize - 1, 0);
                values.arraySize = Mathf.Max(values.arraySize - 1, 0);
            }

            // 인덱스 필드
            EditorGUI.LabelField(indexLabelRect, "Index");
            selectedIndex = Mathf.Max(EditorGUI.IntField(indexValueRect, selectedIndex), 0);
        }

        

        return selectedIndex;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUI.GetPropertyHeight(property, false);
        if (property.isExpanded)
        {
            var keys = property.FindPropertyRelative("keys");
            height = (fieldHeight + intervalHeight) * 4 + keyValuePairRectHeight;
        }

        return height;
    }


#if LEGACY
    private Rect ArrayPropertyField(SerializedProperty array, Rect rect, string title)
    {
        using (var indent = new EditorGUI.IndentLevelScope(1))
        {
            EditorGUI.LabelField(rect, title);
            rect.y += fieldHeight + intervalHeight;

            for (int i = 0; i < array.arraySize; i++)
            {
                GUIStyle style = new GUIStyle(GUI.skin.button);
                //style.normal.background = MakeTex(2, 2, selectedBoxColor);
                //GUI.Button(rect, GUIContent.none, style);
                var element = array.GetArrayElementAtIndex(i);
                EditorGUI.PropertyField(rect, element, GUIContent.none);
                rect.y += fieldHeight + intervalHeight;
            }

        }

        return rect;
    }
#endif
}
#endif