using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TotalDialogue.Core.Variables;
using UnityEditor;
using UnityEngine;

namespace TotalDialogue.Editor
{
    [CustomPropertyDrawer(typeof(IntVar), true)]
    public class IntVarDrawer : TDFVarDrawer
    {
        protected override bool IsDefault(object var)
        {
            IntVar intVar = (IntVar)var;
            return intVar.Value == intVar.DefaultValue;
        }
        protected override void DrawValue(Rect rect0, Rect rect1, Rect rect2, SerializedProperty property, ref BaseVar var)
        {
            IntVar intVar = (IntVar)var;
            EditorGUI.BeginChangeCheck();
            int newValue = EditorGUI.IntField(rect0, intVar.Value);
            if (EditorGUI.EndChangeCheck()){
                intVar.Value = newValue;
            }
        }
    }
}