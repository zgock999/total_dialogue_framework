using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TotalDialogue.Core.Variables
{
    [Serializable]
    public class VariableListener<T>
    {
        public bool fireOnEnable = true;
        public bool fireOnBecome = true;
        public string key;
        [NonSerialized]
        public IVariables variables;
        [NonSerialized]
        public UnityEvent<T> OnSet = new();
        [NonSerialized]
        public UnityEvent<T> OnChanged = new();
        [NonSerialized]
        public UnityEvent OnBecome = new();
        [NonSerialized]
        public UnityEvent OnCease = new();
        public T targetValue = default(T);
        public virtual void SetValue(string key,T value){

        }
        public virtual T GetValue(string key){
            return default(T);
        }
    }
}