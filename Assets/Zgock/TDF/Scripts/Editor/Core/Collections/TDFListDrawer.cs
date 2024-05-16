#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using TotalDialogue.Core.Collections;
using System;

[CustomPropertyDrawer(typeof(TDFList<>))]
public class TDFListDrawer : PropertyDrawer
{
    private UnityEngine.Object targetObject;
    private bool isSubscribed = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var target = fieldInfo.GetValue(property.serializedObject.targetObject);
        targetObject = property.serializedObject.targetObject;

        if (!isSubscribed)
        {
            var genericMethod = this.GetType().GetMethod("SubscribeOnChange").MakeGenericMethod(fieldInfo.FieldType.GetGenericArguments()[0]);
            genericMethod.Invoke(this, new object[] { target });
            isSubscribed = true;
        }

        bool wasModified = property.serializedObject.hasModifiedProperties;
    
        // BeginLockを呼び出す
        var beginLockMethod = target.GetType().GetMethod("BeginLock");
        beginLockMethod?.Invoke(target, null);
        try
        {
            // プロパティを描画
            EditorGUI.PropertyField(position, property, label, true);
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            // EndLockを呼び出す
            var endLockMethod = target.GetType().GetMethod("EndLock");
            endLockMethod?.Invoke(target, null);
        }
        if (!wasModified && property.serializedObject.hasModifiedProperties)
        {
            // InvokeOnChange method should be called when the target is updated from the inspector
            var invokeOnChangeMethod = target.GetType().GetMethod("InvokeOnChange");
            invokeOnChangeMethod?.Invoke(target, null);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
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

    private void OnListChanged()
    {
        // リストが更新されたときに実行するコード
        //Debug.Log("List has been updated.");
        EditorUtility.SetDirty(targetObject);
    }
}
#endif