using System;
using System.Collections;
using System.Collections.Generic;
using TotalDialogue.Core.Variables;
using UnityEngine;
namespace TotalDialogue
{
    public abstract class TDFView : MonoBehaviour
    {
        private IVariables m_variables;
        public virtual IVariables Variables { get => m_variables; set => m_variables = value; }
        readonly Dictionary<Type, Func<string, object>> getMethodMapping;
        readonly Dictionary<Type, Action<string, object>> setMethodMapping;
        public TDFView()
        {
            getMethodMapping = new()
            {
                { typeof(bool), key => Variables.GetBool(key) },
                { typeof(int), key => Variables.GetInt(key) },
                { typeof(float), key => Variables.GetFloat(key) },
                { typeof(string), key => Variables.GetString(key) },
                { typeof(Vector3), key => Variables.GetVector3(key) },
                { typeof(Quaternion), key => Variables.GetQuaternion(key) }
            };
            setMethodMapping = new()
            {
                { typeof(bool), (key, value) => Variables.SetBool(key, (bool)value) },
                { typeof(int), (key, value) => Variables.SetInt(key, (int)value) },
                { typeof(float), (key, value) => Variables.SetFloat(key, (float)value) },
                { typeof(string), (key, value) => Variables.SetString(key, (string)value) },
                { typeof(Vector3), (key, value) => Variables.SetVector3(key, (Vector3)value) },
                { typeof(Quaternion), (key, value) => Variables.SetQuaternion(key, (Quaternion)value) }
            };
        }
        public T GetValue<T>(string key)
        {
            return (T)getMethodMapping[typeof(T)](key);
        }
        public void SetValue<T>(string key, T value)
        {
            setMethodMapping[typeof(T)](key, value);
        }
    }
}
