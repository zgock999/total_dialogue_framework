using UnityEditor;
using UnityEngine;
using TotalDialogue.Core.Variables;

namespace TotalDialogue.Editor
{
    [CustomPropertyDrawer(typeof(TDFGameObject))]
    public class TDFGameObjectDrawer : PropertyDrawer
    {
        private const int objectTypeWidth = 90;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            var typeRect = new Rect(position.x, position.y, objectTypeWidth, position.height);
            var valueRect = new Rect(position.x + objectTypeWidth + 5, position.y, position.width - objectTypeWidth - 10, position.height);

            // Draw fields - pass GUIContent.none to each so they are drawn without labels
            //EditorGUI.PropertyField(keyRect, property.FindPropertyRelative("key"), GUIContent.none);
            EditorGUI.PropertyField(typeRect, property.FindPropertyRelative("objectType"), GUIContent.none);

            var objectType = (TDFGameObject.ObjectType)property.FindPropertyRelative("objectType").enumValueIndex;
            switch (objectType)
            {
                case TDFGameObject.ObjectType.Prefab:
                EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("prefab"), GUIContent.none);
                break;
                case TDFGameObject.ObjectType.Sprite:
                EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("sprite"), GUIContent.none);
                break;
                case TDFGameObject.ObjectType.Texture:
                EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("texture"), GUIContent.none);
                break;
                case TDFGameObject.ObjectType.AudioClip:
                EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("audioClip"), GUIContent.none);
                break;
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}
