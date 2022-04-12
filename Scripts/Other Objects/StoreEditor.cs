using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
[CustomEditor(typeof(Store))]
public class StoreEditor : Editor
{
    private Store m_target;

    private SerializedProperty saleWeapons;
    private SerializedProperty weaponKeys;
    private SerializedProperty weaponValues;

    private SerializedProperty saleItems;
    private SerializedProperty itemKeys;
    private SerializedProperty itemValues;

    private bool showWeapons = true;
    private bool showItems = true;
    private int selectedWeaponIndex;
    private int selectedItemIndex;

    private void OnEnable()
    {
        m_target = target as Store;

        saleWeapons = serializedObject.FindProperty(nameof(saleWeapons));
        weaponKeys = saleWeapons.FindPropertyRelative("keys");
        weaponValues = saleWeapons.FindPropertyRelative("values");

        saleItems = serializedObject.FindProperty(nameof(saleItems));
        itemKeys = saleItems.FindPropertyRelative("keys");
        itemValues = saleItems.FindPropertyRelative("values");
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();

        DrawLine();

        EditorGUILayout.Space();

        var style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
        EditorGUILayout.LabelField("Sale Settings", style);

        // 무기 판매 설정
        EditorGUILayout.Space();

        this.showWeapons = SetDictionaryTitle(this.showWeapons, "Weapons", weaponKeys, weaponValues);
        if (showWeapons)
        {
            EditorGUILayout.Space();

            using (var h = new EditorGUILayout.HorizontalScope())
            {
                var w = EditorGUIUtility.currentViewWidth;
                EditorGUILayout.LabelField("Slot", GUILayout.Width(w * 0.35f));
                EditorGUILayout.LabelField("Weapon Prefab", GUILayout.Width(w * 0.35f));
                EditorGUILayout.LabelField("Price", GUILayout.Width(w * 0.2f));
            }

            SetStoreItems(weaponKeys, weaponValues, typeof(Weapon));

            EditorGUILayout.Space();

            this.selectedWeaponIndex = SetItemChangeButton(weaponKeys, weaponValues, this.selectedWeaponIndex, "Weapon");
        }

        EditorGUILayout.Space();

        // 아이템 판매 설정

        this.showItems = SetDictionaryTitle(this.showItems, "Items", itemKeys, itemValues);
        if (showItems)
        {
            EditorGUILayout.Space();

            using (var h = new EditorGUILayout.HorizontalScope())
            {
                var w = EditorGUIUtility.currentViewWidth;
                EditorGUILayout.LabelField("Slot", GUILayout.Width(w * 0.3f));
                EditorGUILayout.LabelField("Item Prefab", GUILayout.Width(w * 0.3f));
                EditorGUILayout.LabelField("Price", GUILayout.Width(w * 0.15f));
                EditorGUILayout.LabelField("Count", GUILayout.Width(w * 0.15f));
            }

            SetStoreItems(itemKeys, itemValues, typeof(Item), true);

            EditorGUILayout.Space();

            this.selectedItemIndex = SetItemChangeButton(itemKeys, itemValues, this.selectedItemIndex, "Item");
        }

        serializedObject.ApplyModifiedProperties();
    }

    private int SetItemChangeButton(SerializedProperty keys, SerializedProperty values, int selectedIndex, string text)
    {
        EditorGUILayout.LabelField($"{text} Change");
        using (var h = new EditorGUILayout.HorizontalScope())
        {
            var w = EditorGUIUtility.currentViewWidth;
            if (GUILayout.Button($"Add {text}", GUILayout.Width(w * 0.35f)))
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

            if (GUILayout.Button($"Remove {text}", GUILayout.Width(w * 0.35f)))
            {
                for (int i = selectedIndex; i < keys.arraySize; i++)
                {
                    keys.MoveArrayElement(i, i + 1);
                }

                keys.arraySize = Mathf.Max(keys.arraySize - 1, 0);
                values.arraySize = Mathf.Max(values.arraySize - 1, 0);
            }

            selectedIndex = Mathf.Max(EditorGUILayout.IntField(selectedIndex, GUILayout.Width(w * 0.2f)), 0);
        }

        return selectedIndex;
    }

    private void SetStoreItems(SerializedProperty keys, SerializedProperty values, Type itemType, bool countDisplay = false)
    {
        for (int i = 0; i < keys.arraySize; i++)
        {
            var key = keys.GetArrayElementAtIndex(i);
            var value = values.GetArrayElementAtIndex(i);
            var item = value.FindPropertyRelative("itemPrefab");
            var price = value.FindPropertyRelative("price");
            var count = value.FindPropertyRelative("count");

            using (var h = new EditorGUILayout.HorizontalScope())
            {
                if (countDisplay)
                {
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        var w = EditorGUIUtility.currentViewWidth;
                        var key_obj = EditorGUILayout.ObjectField(key.objectReferenceValue, typeof(StoreItemSlot), true, GUILayout.Width(w * 0.3f));
                        var item_obj = EditorGUILayout.ObjectField(item.objectReferenceValue, itemType, true, GUILayout.Width(w * 0.3f));
                        var price_value = EditorGUILayout.IntField(price.intValue, GUILayout.Width(w * 0.15f));
                        var count_value = EditorGUILayout.IntField(count.intValue, GUILayout.Width(w * 0.15f));
                        if (check.changed)
                        {
                            key.objectReferenceValue = key_obj;
                            item.objectReferenceValue = item_obj;
                            price.intValue = price_value;
                            count.intValue = count_value;
                        }
                    }
                }
                else
                {
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        var w = EditorGUIUtility.currentViewWidth;
                        var key_obj = EditorGUILayout.ObjectField(key.objectReferenceValue, typeof(StoreItemSlot), true, GUILayout.Width(w * 0.35f));
                        var item_obj = EditorGUILayout.ObjectField(item.objectReferenceValue, itemType, true, GUILayout.Width(w * 0.35f));
                        var price_value = EditorGUILayout.IntField(price.intValue, GUILayout.Width(w * 0.2f));
                        if (check.changed)
                        {
                            key.objectReferenceValue = key_obj;
                            item.objectReferenceValue = item_obj;
                            price.intValue = price_value;
                        }
                    }
                }               
            }          
        }
    }

    private bool SetDictionaryTitle(bool foldout, string titleTxt, SerializedProperty keys, SerializedProperty values)
    {
        using (var h = new EditorGUILayout.HorizontalScope())
        {
            var style = new GUIStyle(EditorStyles.foldout);
            //ColorUtility.TryParseHtmlString("#ACACAC", out var c);
            //EditorStyles.foldout.normal.textColor = c;
            ColorUtility.TryParseHtmlString("#7ba8eb", out var color);
            style.onNormal.textColor = color;
            foldout = EditorGUILayout.Foldout(foldout, titleTxt, true, style);
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                int count = EditorGUILayout.DelayedIntField(keys.arraySize, GUILayout.Width(50f));
                count = Mathf.Max(count, 0);
                if (check.changed)
                {
                    keys.arraySize = count;
                    values.arraySize = count;
                }
            }
        }
        return foldout;
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