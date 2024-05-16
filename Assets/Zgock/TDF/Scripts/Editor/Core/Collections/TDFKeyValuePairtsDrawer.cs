#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Net.WebSockets;
using System;
using System.Reflection;
using TotalDialogue.Core.Collections;
using TotalDialogue.Core.Variables;
using Codice.CM.SEIDInfo;
using System.Collections.Generic;

namespace TotalDialogue.Editor
{
    [CustomPropertyDrawer(typeof(TDFKeyValuePairs<,>), true)]
    public class TDFKeyValuePairsDrawer : TDFDrawer
    {
        protected ReorderableList reorderableList;
        protected UnityEngine.Object targetObject;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (reorderableList == null)
            {
                var list = property.FindPropertyRelative("list");
                DoCreateReorderableList(property,list);

                reorderableList.drawHeaderCallback = (Rect rect) => DoDrawHeader(rect,property);
                reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => DoDrawElement(rect, index, isActive, isFocused, property);
                reorderableList.onChangedCallback = (ReorderableList list) => DoChanged(list, property);
                reorderableList.onAddCallback = (ReorderableList list) => DoAdd(list, property);
                reorderableList.onRemoveCallback = (ReorderableList list) => DoRemove(list, property);
                reorderableList.elementHeightCallback = (int index) => DoElementHeight(index, property);

                reorderableList.onCanAddCallback = (ReorderableList list) => CanAdd(list, property);
                reorderableList.onCanRemoveCallback = (ReorderableList list) => CanRemove(list, property);

                //var target = fieldInfo.GetValue(property.serializedObject.targetObject);
                var target = GetNestedField(property.serializedObject.targetObject, property.propertyPath);
                targetObject = property.serializedObject.targetObject;
                MethodInfo method;
                method = this.GetType().GetMethod("SubscribeOnChangeNonGeneric");
                method.Invoke(this, new object[] { target });
            }
            PreDoList(reorderableList, property);
            DoBeginLock(property);
            try
            {
                reorderableList.DoList(position);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                DoEndLock(property);
            }
            PostDoList(reorderableList, property);
        }
        public static object GetNestedField(object target, string propertyPath)
        {
            var pathParts = propertyPath.Split('.');
            foreach (var part in pathParts)
            {
                var type = target.GetType();
                var field = type.GetField(part, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (field == null)
                {
                    return null;
                }
                target = field.GetValue(target);
            }
            return target;
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return reorderableList != null ? reorderableList.GetHeight() : EditorGUI.GetPropertyHeight(property, label);
        }

        public void SubscribeOnChange<T>(object target)
        {
            var targetList = target as ITDFList;
            if (targetList == null)
            {
                return;
            }

            targetList.OnChange += OnListChanged;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SubscribeOnChangeNonGeneric(object target)
        {
            if (target is ITDFList targetList)
            {
                targetList.OnChange += OnListChanged;
            }
        }
        private void OnListChanged()
        {
            EditorUtility.SetDirty(targetObject);
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected void DoUpdateDuplicates(SerializedProperty property)
        {
            InvokeMethod(property, "UpdateDuplicates");
        }
        protected void DoInvokeOnChange(SerializedProperty property)
        {
            InvokeMethod(property, "InvokeOnChange");
        }
        protected void DoBeginLock(SerializedProperty property)
        {
            InvokeMethod(property, "BeginLock");
        }

        protected void DoEndLock(SerializedProperty property)
        {
            InvokeMethod(property, "EndLock");
        }
        protected bool IsDuplicateIndex(SerializedProperty property, int index)
        {
            return (bool)InvokeMethodAndReturn(property, "IsDuplicateIndex", new object[] { index });
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Customizing(Overridable) Methods
 
        protected virtual void DoCreateReorderableList(SerializedProperty property,SerializedProperty list)
        {
            reorderableList = new ReorderableList(property.serializedObject, list, true, true, true, true);
        }
        protected virtual void DoDrawHeader(Rect rect, SerializedProperty property)
        {
            rect.x += 10; // Indent the header to match the foldout
            bool isExpanded = !property.FindPropertyRelative("folded").boolValue;
            isExpanded = EditorGUI.Foldout(rect, isExpanded, property.displayName, true);
            property.FindPropertyRelative("folded").boolValue = !isExpanded;
            //EditorGUI.LabelField(rect, property.displayName);
            reorderableList.displayAdd = isExpanded;
            reorderableList.displayRemove = isExpanded;
            reorderableList.draggable = isExpanded;
        }
        protected virtual void DoDrawElement(Rect rect, int index, bool isActive, bool isFocused, SerializedProperty property)
        {
            if (property.FindPropertyRelative("folded").boolValue) return;
            var list = property.FindPropertyRelative("list");
            var element = list.GetArrayElementAtIndex(index);
            rect.y += 2;
            var duplicate = IsDuplicateIndex(property, index);
            var color = duplicate ? Color.red : GUI.color;

            GUI.color = color;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(rect, element);
            if (EditorGUI.EndChangeCheck())
            {
                EditorApplication.delayCall += () => DoUpdateDuplicates(property);
                EditorApplication.delayCall += () => DoInvokeOnChange(property);
            }
            GUI.color = Color.white;
        }
        protected virtual void DoChanged(ReorderableList list, SerializedProperty property)
        {
            //Debug.Log("Changed");
            EditorApplication.delayCall += () => DoUpdateDuplicates(property);
            EditorApplication.delayCall += () => DoInvokeOnChange(property);
        }
        protected virtual void DoAdd(ReorderableList list, SerializedProperty property)
        {
            var listProperty = property.FindPropertyRelative("list");
            listProperty.arraySize++;
            property.serializedObject.ApplyModifiedProperties();
            EditorApplication.delayCall += () => DoUpdateDuplicates(property);
            EditorApplication.delayCall += () => DoInvokeOnChange(property);
        }
        protected virtual void DoRemove(ReorderableList list, SerializedProperty property)
        {
            var listProperty = property.FindPropertyRelative("list");
            listProperty.DeleteArrayElementAtIndex(list.index);
            property.serializedObject.ApplyModifiedProperties();
            EditorApplication.delayCall += () => DoUpdateDuplicates(property);
            EditorApplication.delayCall += () => DoInvokeOnChange(property);
        }
        protected virtual float DoElementHeight(int index, SerializedProperty property)
        {
            if (property.FindPropertyRelative("folded").boolValue) return 0;
            var list = property.FindPropertyRelative("list");
            var element = list.GetArrayElementAtIndex(index);
            var key = element.FindPropertyRelative("key");
            var value = element.FindPropertyRelative("value");
            float keyHeight = EditorGUI.GetPropertyHeight(key);
            float valueHeight = EditorGUI.GetPropertyHeight(value);
            return keyHeight > valueHeight ? keyHeight : valueHeight;
        }
        protected virtual bool CanAdd(ReorderableList list, SerializedProperty property)
        {
            // for override
            return true;
        }
        protected virtual bool CanRemove(ReorderableList list, SerializedProperty property)
        {
            // for override
            return true;
        }
        protected virtual void PreDoList(ReorderableList list, SerializedProperty property)
        {
            // for override
        }
        protected virtual void PostDoList(ReorderableList list, SerializedProperty property)
        {
            // for override
        }
    }
}
#endif