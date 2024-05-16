using System.Collections;
using System.Collections.Generic;
using TotalDialogue.Core.Variables;
using UnityEditor;
using UnityEngine;
namespace TotalDialogue.Editor
{
    [CustomPropertyDrawer(typeof(BoolVar), true)]
    public class BoolVarDrawer : TDFVarDrawer
    {
　      protected override bool IsDefault(object var)
        {
            BoolVar boolVar = (BoolVar)var;
            return boolVar.Value == boolVar.DefaultValue;
        }
        protected override void DrawValue(Rect rect0, Rect rect1, Rect rect2, SerializedProperty property, ref BaseVar var)
        {
            BoolVar boolVar = (BoolVar)var;
            EditorGUI.BeginChangeCheck();
            bool newValue = EditorGUI.Toggle(rect0, boolVar.Value);
            if (EditorGUI.EndChangeCheck()){
                boolVar.Value = newValue;
            }
        }
    }
}
