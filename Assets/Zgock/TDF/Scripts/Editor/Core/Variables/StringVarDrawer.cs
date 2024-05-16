using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TotalDialogue.Core.Variables;
using UnityEditor;
using UnityEngine;

namespace TotalDialogue.Editor
{
    [CustomPropertyDrawer(typeof(StringVar), true)]
    public class StringVarDrawer : TDFVarDrawer
    {
        protected override bool IsDefault(object var)
        {
            StringVar stringVar = (StringVar)var;
            return stringVar.Value == stringVar.DefaultValue;
        }
        protected override void DrawValue(Rect rect0, Rect rect1, Rect rect2, SerializedProperty property, ref BaseVar var)
        {
            StringVar stringVar = (StringVar)var;
            EditorGUI.BeginChangeCheck();
            string newValue = EditorGUI.TextField(rect0, stringVar.Value);
            if (EditorGUI.EndChangeCheck()){
                stringVar.Value = newValue;
            }
        }
    }
}