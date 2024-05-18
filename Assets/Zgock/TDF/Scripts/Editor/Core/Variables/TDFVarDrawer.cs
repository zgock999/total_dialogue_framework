#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TotalDialogue.Core.Variables;
using UnityEditor;
using UnityEngine;

namespace TotalDialogue.Editor
{
    public class TDFVarDrawer : TDFDrawer
    {
        private const int m_labelWidth = 20;
        private const int m_buttonWidth = 20;
        protected virtual int LabelWidth { get => m_labelWidth; }
        protected virtual int ButtonWidth { get => m_buttonWidth; }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
            var rect = new Rect(position.x, position.y, LabelWidth, EditorGUIUtility.singleLineHeight);
            var rect0 = new Rect(position.x + LabelWidth, position.y, position.width - LabelWidth - ButtonWidth * 2, EditorGUIUtility.singleLineHeight);
            var rect1 = new Rect(position.x + position.width - ButtonWidth * 2, position.y, ButtonWidth, EditorGUIUtility.singleLineHeight);
            var rect2 = new Rect(position.x + position.width - ButtonWidth, position.y, ButtonWidth, EditorGUIUtility.singleLineHeight);

            BaseVar var = (BaseVar)GetValueFromSerializedProperty(property);
            Color prevColor = GUI.color;
            if (!IsDefault(var)){
                GUI.color = Color.yellow;
            }

            EditorGUI.LabelField(rect, var.TypeName[..1]);
            DrawValue(rect0, rect1, rect2, property,ref var);
            GUI.color = prevColor;
            if (GUI.Button(rect1, "R")){
                var.RestoreDefault();
            }
            if (GUI.Button(rect2, "S")){
                var.StoreDefault();
            }
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
        protected virtual bool IsDefault(object var){
            return true;
        }

        protected virtual void DrawValue(Rect rect0, Rect rect1, Rect rect2, SerializedProperty property, ref BaseVar var){
            EditorGUI.LabelField(rect0, "Not Supported");
        }
    }
}
#endif