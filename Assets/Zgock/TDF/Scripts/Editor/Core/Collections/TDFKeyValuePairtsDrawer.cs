#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Net.WebSockets;
using System;
using TotalDialogue.Core.Collections;

namespace TotalDialogue.Editor
{
    [CustomPropertyDrawer(typeof(TDFDictionary<,>))]
    [CustomPropertyDrawer(typeof(TDFKeyValuePairs<,>))]
    public class TDFKeyValuePairsDrawer : PropertyDrawer
    {
        private ReorderableList reorderableList;
        private UnityEngine.Object targetObject;

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

                var target = fieldInfo.GetValue(property.serializedObject.targetObject);
                targetObject = property.serializedObject.targetObject;
                var genericMethod = this.GetType().GetMethod("SubscribeOnChange").MakeGenericMethod(fieldInfo.FieldType.GetGenericArguments()[0]);
                genericMethod.Invoke(this, new object[] { target });
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
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return reorderableList != null ? reorderableList.GetHeight() : EditorGUI.GetPropertyHeight(property, label);
        }

        public void SubscribeOnChange<T>(object target)
        {
            var targetList = target as ITDFList<T>;
            if (targetList == null)
            {
                return;
            }

            targetList.OnChange += OnListChanged;
        }

        private void OnListChanged()
        {
            // リストが更新されたときに実行するコード
            //Debug.Log("List has been updated.");
            EditorUtility.SetDirty(targetObject);
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected void InvokeMethod(SerializedProperty property, string methodName)
        {
            var listObject = fieldInfo.GetValue(property.serializedObject.targetObject);
            var method = fieldInfo.FieldType.GetMethod(methodName);
            method.Invoke(listObject, null);
        }
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
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Customizing(Overridable) Methods
 
        protected virtual void DoCreateReorderableList(SerializedProperty property,SerializedProperty list)
        {
            reorderableList = new ReorderableList(property.serializedObject, list, true, true, true, true);
        }
        protected virtual void DoDrawHeader(Rect rect, SerializedProperty property)
        {
            EditorGUI.LabelField(rect, property.displayName);
        }
        protected virtual void DoDrawElement(Rect rect, int index, bool isActive, bool isFocused, SerializedProperty property)
        {
            var list = property.FindPropertyRelative("list");
            var element = list.GetArrayElementAtIndex(index);
            rect.y += 2;
            var listObject = fieldInfo.GetValue(reorderableList.serializedProperty.serializedObject.targetObject);
            var isDuplicateIndexMethod = fieldInfo.FieldType.GetMethod("IsDuplicateIndex");
            var duplicate = (bool)isDuplicateIndexMethod.Invoke(listObject, new object[] { index });
            var color = duplicate ? Color.red : GUI.color;

            GUI.color = color;
            EditorGUI.BeginChangeCheck();
            //EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element);
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