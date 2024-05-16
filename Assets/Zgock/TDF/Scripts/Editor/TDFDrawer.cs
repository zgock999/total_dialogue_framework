using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TotalDialogue.Core.Variables;
using UnityEditor;
using UnityEngine;

namespace TotalDialogue.Editor
{
    public class TDFDrawer : PropertyDrawer
    {
        /// <summary>
        /// リフレクションを使って、指定したメソッドを実行します
        /// </summary>
        /// <param name="property">実行するメソッドがあるプロパティ</param>
        /// <param name="methodName">実行するメソッド名</param>
        /// <param name="args">メソッドに渡す引数</param>
        /// <remarks>戻り値がないメソッドのみ使用可能です</remarks>
        protected void InvokeMethod(SerializedProperty property, string methodName, object[] args = null)
        {
            object propertyObject = GetValueFromSerializedProperty(property);
            var method = propertyObject.GetType().GetMethod(methodName);
            method.Invoke(propertyObject, args);
        }
        /// <summary>
        /// リフレクションを使って、指定したメソッドを実行し、戻り値を返します
        /// </summary>
        /// <param name="property">実行するメソッドがあるプロパティ</param>
        /// <param name="methodName">実行するメソッド名</param>
        /// <param name="args">メソッドに渡す引数</param>
        /// <returns>メソッドの戻り値</returns>
        /// <remarks>戻り値があるメソッドのみ使用可能です</remarks>
        protected object InvokeMethodAndReturn(SerializedProperty property, string methodName, object[] args = null)
        {
            object propertyObject = GetValueFromSerializedProperty(property);
            var method = propertyObject.GetType().GetMethod(methodName);
            return method.Invoke(propertyObject, args);
        }
        /// <summary>
        /// プロパティからオブジェクトを取得します
        /// </summary>
        /// <param name="property">取得するオブジェクトがあるプロパティ</param>
        /// <returns>取得したオブジェクト</returns>
        /// <remarks>プロパティの値を取得するために使用します</remarks>
        protected object GetValueFromSerializedProperty(SerializedProperty property)
        {
            object obj = property.serializedObject.targetObject;
            string path = property.propertyPath.Replace(".Array.data[", "[");
            string[] elements = path.Split('.');
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    string elementName = element.Substring(0, element.IndexOf("["));
                    int index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }
            return obj;
        }

        private object GetValue(object source, string name)
        {
            if (source == null)
                return null;
            Type type = source.GetType();
            FieldInfo f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f == null)
            {
                PropertyInfo p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p == null)
                    return null;
                return p.GetValue(source, null);
            }
            return f.GetValue(source);
        }

        private object GetValue(object source, string name, int index)
        {
            var enumerable = GetValue(source, name) as IEnumerable;
            var enm = enumerable.GetEnumerator();
            while (index-- >= 0)
                enm.MoveNext();
            return enm.Current;
        }
    }
}