using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TotalDialogue.Core.Variables;
using UnityEditor;
using UnityEngine;

namespace TotalDialogue.Editor
{
    [CustomPropertyDrawer(typeof(FloatVar), true)]
    public class FloatVarDrawer : TDFVarDrawer
    {
        protected override bool IsDefault(object var)
        {
            FloatVar floatVar = (FloatVar)var;
            return floatVar.Value == floatVar.DefaultValue;
        }
        protected override void DrawValue(Rect rect0, Rect rect1, Rect rect2, SerializedProperty property, ref BaseVar var)
        {
            FloatVar floatVar = (FloatVar)var;
            float prevValue = floatVar.Value;
            EditorGUI.BeginChangeCheck();
            float newValue = EditorGUI.FloatField(rect0, floatVar.Value);
            if (EditorGUI.EndChangeCheck()){
                floatVar.Value = newValue;
            }
        }
    }
}