using System;
using System.Collections;
using System.Collections.Generic;
using TotalDialogue.Core.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace TotalDialogue.Core.Variables
{
    [System.Serializable]
    public class BaseVar : ITDFVar
    {
        public virtual string TypeName { get => "?"; }
        [SerializeField]
        [HideInInspector]
        protected string m_key;
        public virtual string Key { get => m_key; set => m_key = value; }
        public virtual string StringValue {get;}

        public ITDFVar Interface => this;

        public virtual void RestoreDefault()
        {
        }

        public virtual void StoreDefault()
        {
        }
        public UnityEvent OnValueChanged = new();
        public virtual void SetSelf(BaseVar self){
        }
    }
}
