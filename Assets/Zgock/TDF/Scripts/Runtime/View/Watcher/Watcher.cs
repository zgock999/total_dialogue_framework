using System;
using System.Collections;
using System.Collections.Generic;
using TotalDialogue.Core.Variables;
using TMPro;
using UnityEngine;
using Codice.Client.BaseCommands;
namespace TotalDialogue
{
    public abstract class Watcher<TValue,TListener> : TDFView where TListener : VariableListener<TValue> 
    {
        public TMP_Text text;

        TListener listener;

        protected virtual bool FireOnEnable{get; set;} 
        public string key;
        protected abstract void OnChanged(TValue value);
 
        protected virtual void OnEnable()
        {
            listener = Activator.CreateInstance<TListener>();
            listener.variables = Variables;
            listener.OnChanged.AddListener(OnChanged);
            listener.key = key;
            listener.fireOnBecome = false;;
            switch (listener)
            {
                case BoolListener boolListener:
                    Variables.AddListener(boolListener);
                    boolListener.targetValue = true;
                    if (FireOnEnable) boolListener.OnChanged.Invoke(Variables.GetBool(key));
                    break;
                case IntListener intListener:
                    intListener.targetValue = 0;
                    Variables.AddListener(intListener);
                    if (FireOnEnable) intListener.OnChanged.Invoke(Variables.GetInt(key));
                    break;
                case FloatListener floatListener:
                    floatListener.targetValue = 0;
                    Variables.AddListener(floatListener);
                    if (FireOnEnable) floatListener.OnChanged.Invoke(Variables.GetFloat(key));
                    break;
                case StringListener stringListener:
                    stringListener.targetValue = "";
                    Variables.AddListener(stringListener);
                    if (FireOnEnable) stringListener.OnChanged.Invoke(Variables.GetString(key));
                    break;
                case Vector3Listener vector3Listener:
                    vector3Listener.targetValue = Vector3.zero;
                    Variables.AddListener(vector3Listener);
                    if (FireOnEnable) vector3Listener.OnChanged.Invoke(Variables.GetVector3(key));
                    break;
                case QuaternionListener quaternionListener:
                    quaternionListener.targetValue = Quaternion.identity;
                    Variables.AddListener(quaternionListener);
                    if (FireOnEnable) quaternionListener.OnChanged.Invoke(Variables.GetQuaternion(key));
                    break;
                default:
                    break;
            }
        }
        protected virtual void OnDisable()
        {
            switch (listener)
            {
                case BoolListener boolListener:
                    Variables.RemoveListener(boolListener);
                    break;
                case IntListener intListener:
                    Variables.RemoveListener(intListener);
                    break;
                case FloatListener floatListener:
                    Variables.RemoveListener(floatListener);
                    break;
                case StringListener stringListener:
                    Variables.RemoveListener(stringListener);
                    break;
                case Vector3Listener vector3Listener:
                    Variables.RemoveListener(vector3Listener);
                    break;
                case QuaternionListener quaternionListener:
                    Variables.RemoveListener(quaternionListener);
                    break;
                default:
                    break;
            }
            listener.OnChanged.RemoveListener(OnChanged);
            listener = null;
        }
    }
}
