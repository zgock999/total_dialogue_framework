#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using TotalDialogue.Core.Collections;

namespace TotalDialogue.Editor
{
    [CustomPropertyDrawer(typeof(TDFKeyValuePair<,>))]
    public class TDFKeyValuePairDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var keyProperty = property.FindPropertyRelative("key");
            var valueProperty = property.FindPropertyRelative("value");
            float keyHeight = EditorGUI.GetPropertyHeight(keyProperty, label);
            float valueHeight = EditorGUI.GetPropertyHeight(valueProperty, label);

            float splitRatio = property.FindPropertyRelative("splitRatio").floatValue; // splitRatioをSerializedPropertyから取得
            bool isDragging = property.FindPropertyRelative("isDragging").boolValue; // isDraggingをSerializedPropertyから取得

            // splitRatioが0の場合、0.5にリセット
            if (splitRatio == 0)
            {
                splitRatio = 0.5f;
                property.FindPropertyRelative("splitRatio").floatValue = splitRatio;
            }

            var keyRect = new Rect(position.x, position.y, position.width * splitRatio - 4, keyHeight);
            var valueRect = new Rect(position.x + position.width * splitRatio + 4, position.y, position.width * (1 - splitRatio) - 4, valueHeight);

            EditorGUI.PropertyField(keyRect, keyProperty, GUIContent.none);
            EditorGUI.PropertyField(valueRect, valueProperty, GUIContent.none);

            // ドラッグによるリサイズをサポートするためのコード
            var dragArea = new Rect(position.x + position.width * splitRatio - 4, position.y, 8, position.height);
            EditorGUIUtility.AddCursorRect(dragArea, MouseCursor.ResizeHorizontal);
            if (Event.current.type == EventType.MouseDown && dragArea.Contains(Event.current.mousePosition))
            {
                isDragging = true;
                property.FindPropertyRelative("isDragging").boolValue = isDragging; // isDraggingをSerializedPropertyに保存
            }
            if (isDragging && Event.current.type == EventType.MouseDrag)
            {
                splitRatio = (Event.current.mousePosition.x - position.x) / position.width;
                splitRatio = Mathf.Clamp(splitRatio, 0.1f, 0.9f); // 分割比率を10%から90%の範囲に制限
                property.FindPropertyRelative("splitRatio").floatValue = splitRatio; // splitRatioをSerializedPropertyに保存
                EditorUtility.SetDirty(property.serializedObject.targetObject); // ドラッグ中にエディタを再描画
            }
            if (Event.current.type == EventType.MouseUp)
            {
                isDragging = false;
                property.FindPropertyRelative("isDragging").boolValue = isDragging; // isDraggingをSerializedPropertyに保存
            }

            EditorGUI.EndProperty();
        }
    }
}
#endif