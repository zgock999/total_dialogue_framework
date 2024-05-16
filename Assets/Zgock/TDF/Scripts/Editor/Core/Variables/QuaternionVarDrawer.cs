using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TotalDialogue.Core.Variables;
using UnityEditor;
using UnityEngine;

namespace TotalDialogue.Editor
{
    [CustomPropertyDrawer(typeof(QuaternionVar), true)]
    public class QuaternionVarDrawer : TDFVarDrawer
    {
        protected override bool IsDefault(object var)
        {
            QuaternionVar quaternionVar = (QuaternionVar)var;
            Vector3 defaultValue = quaternionVar.DefaultValue.eulerAngles;
            Vector3 value = quaternionVar.Value.eulerAngles;
            return value == defaultValue;
        }
        protected override void DrawValue(Rect rect0, Rect rect1, Rect rect2, SerializedProperty property, ref BaseVar var)
        {
            QuaternionVar quaternionVar = (QuaternionVar)var;
            EditorGUI.BeginChangeCheck();
            Vector3 newValue = EditorGUI.Vector3Field(rect0, GUIContent.none, quaternionVar.Value.eulerAngles);
            if (EditorGUI.EndChangeCheck()){
                quaternionVar.Value = Quaternion.Euler(newValue);
            }
        }
    }
 }