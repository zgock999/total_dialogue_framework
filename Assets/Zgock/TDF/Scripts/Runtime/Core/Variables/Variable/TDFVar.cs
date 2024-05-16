using System;
using System.Collections;
using System.Collections.Generic;
using TotalDialogue.Core.Collections;
using UnityEngine;

namespace TotalDialogue.Core.Variables
{
    [System.Serializable]
    public class TDFVar<T,TListener> : BaseVar, ITDFVar<T, TListener> where TListener : VariableListener<T>
    {
        [NonSerialized]
        protected List<TListener> m_listeners = new();
        [SerializeField]
        protected T m_value = default(T);
        [SerializeField]
        protected T m_defaultValue = default(T);
        public TDFVar(string key = "", T defaultValue = default(T)){
            m_key = key;
            m_defaultValue = defaultValue;
            m_value = defaultValue;
        }
        public T Value
        {
            get
            {
                return GetValue();
            }
            set
            {
                SetValue(value);
            }
        }

        protected T GetValue()
        {
            return m_value;
        }

        protected void SetValue(T value)
        {
            T oldValue = m_value;
            m_value = value;
            OnValueChanged.Invoke();
            bool changed = IsChanged(oldValue, value);
            foreach (TListener listener in m_listeners)
            {
                Fire(listener, oldValue, value, changed);
            }
        }
        public void FireOnChanged(T oldValue, T newValue){
            foreach (TListener listener in m_listeners)
            {
                Fire(listener, oldValue, newValue, true);
            }
        }
        protected virtual void Fire(TListener listener,T oldValue, T newValue,bool changed){
            if (changed){
                listener.OnChanged.Invoke(newValue);
                if (listener.fireOnBecome)
                {
                    if (IsBecome(oldValue, newValue, listener.targetValue))
                    {
                        listener.OnBecome.Invoke();
                    }
                    if (IsCease(oldValue, newValue, listener.targetValue))
                    {
                        listener.OnCease.Invoke();
                    }
                }
            }
            listener.OnSet.Invoke(newValue);
        }
        protected virtual bool IsChanged(T oldValue, T newValue){
            return !oldValue.Equals(newValue);
        }
        protected virtual bool IsBecome(T oldValue, T newValue, T targetValue){
            if (targetValue == null){
                return false;
            }
            return targetValue.Equals(newValue);
        }
        protected virtual bool IsCease(T oldValue, T newValue, T targetValue){
            if (targetValue == null){
                return false;
            }
            return !targetValue.Equals(newValue);
        }
        public T DefaultValue { get => m_defaultValue; set => m_defaultValue = value; }
        public override void StoreDefault(){
            m_defaultValue = m_value;
        }
        public override void RestoreDefault(){
            m_value = m_defaultValue;
        }
        public override string StringValue { get => m_value.ToString(); }
        public ITDFVar<T, TListener> GenericInterFace { get => this; }
        T ITDFVar<T, TListener>.Value { get => m_value; set => SetValue(value); }

        public ITDFVar<T, TListener> GenericInterface => throw new NotImplementedException();

        public void AddListener(TListener listener){
            m_listeners.Add(listener);
        }
        public void RemoveListener(TListener listener){
            m_listeners.Remove(listener);
        }

        public bool GetBoolValue()
        {
            return (bool)(object)m_value;
        }

        public int GetIntValue()
        {
            return (int)(object)m_value;
        }

        public float GetFloatValue()
        {
            return (float)(object)m_value;
        }

        public string GetStringValue()
        {
            return (string)(object)m_value;
        }

        public Vector3 GetVector3Value()
        {
            return (Vector3)(object)m_value;
        }

        public Quaternion GetQuaternionValue()
        {
            return (Quaternion)(object)m_value;
        }

        public void SetBoolValue(bool value)
        {
            SetValue((T)(object)value);
        }

        public void SetIntValue(int value)
        {
            SetValue((T)(object)value);
        }

        public void SetFloatValue(float value)
        {
            SetValue((T)(object)value);
        }

        public void SetStringValue(string value)
        {
            SetValue((T)(object)value);
        }

        public void SetVector3Value(Vector3 value)
        {
            SetValue((T)(object)value);
        }
        public override void SetSelf(BaseVar self)
        {
            SetValue(((TDFVar<T,TListener>)self).m_value);
        }
        public virtual TDFVar<T,TListener> GetSekf(){
            return this;
        }
    }
}