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
    [CustomPropertyDrawer(typeof(Vector3Var), true)]
    public class Vector3VarDrawer : TDFVarDrawer
    {
        protected override bool IsDefault(object var)
        {
            Vector3Var vector3Var = (Vector3Var)var;
            return vector3Var.Value == vector3Var.DefaultValue;
        }
        protected override void DrawValue(Rect rect0, Rect rect1, Rect rect2, SerializedProperty property, ref BaseVar var)
        {
            Vector3Var vector3Var = (Vector3Var)var;
            Vector3 prevValue = vector3Var.Value;
            EditorGUI.BeginChangeCheck();
            Vector3 newValue = EditorGUI.Vector3Field(rect0, GUIContent.none, vector3Var.Value);
            if (EditorGUI.EndChangeCheck()){
                vector3Var.Value = newValue;
            }
        }
    }
}
#endif
