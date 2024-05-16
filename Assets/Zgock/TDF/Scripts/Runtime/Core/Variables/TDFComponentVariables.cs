using System.Collections;
using System.Collections.Generic;
using TotalDialogue.Core.Variables;
using UnityEngine;

namespace TotalDialogue.Core
{
    public class TDFComponentVariables : MonoBehaviour,IVariables
    {
        public TDFVariables variables = new();

        public int MaxChoice => variables.MaxChoice;

        public int MaxDialogue => variables.MaxDialogue;

        public void AddListener(BoolListener listener)
        {
            variables.AddListener(listener);
        }

        public void AddListener(IntListener listener)
        {
            variables.AddListener(listener);
        }

        public void AddListener(FloatListener listener)
        {
            variables.AddListener(listener);
        }

        public void AddListener(StringListener listener)
        {
            variables.AddListener(listener);
        }

        public void AddListener(Vector3Listener listener)
        {
            variables.AddListener(listener);
        }

        public void AddListener(QuaternionListener listener)
        {
            variables.AddListener(listener);
        }

        public bool GetBool(string key)
        {
            return variables.GetBool(key);
        }

        public float GetFloat(string key)
        {
            return variables.GetFloat(key);
        }

        public int GetInt(string key)
        {
            return variables.GetInt(key);
        }

        public Quaternion GetQuaternion(string key)
        {
            return variables.GetQuaternion(key);
        }

        public string GetString(string key)
        {
            return variables.GetString(key);
        }

        public Vector3 GetVector3(string key)
        {
            return variables.GetVector3(key);
        }

        public void RemoveListener(BoolListener listener)
        {
            variables.RemoveListener(listener);
        }

        public void RemoveListener(IntListener listener)
        {
            variables.RemoveListener(listener);
        }

        public void RemoveListener(FloatListener listener)
        {
            variables.RemoveListener(listener);
        }

        public void RemoveListener(StringListener listener)
        {
            variables.RemoveListener(listener);
        }

        public void RemoveListener(Vector3Listener listener)
        {
            variables.RemoveListener(listener);
        }

        public void RemoveListener(QuaternionListener listener)
        {
            variables.RemoveListener(listener);
        }

        public void Reset()
        {
            variables.Reset();
        }

        public void SetBool(string key, bool value)
        {
            variables.SetBool(key, value);
        }

        public void SetFloat(string key, float value)
        {
            variables.SetFloat(key, value);
        }

        public void SetInt(string key, int value)
        {
            variables.SetInt(key, value);
        }

        public void SetQuaternion(string key, Quaternion value)
        {
            variables.SetQuaternion(key, value);
        }

        public void SetString(string key, string value)
        {
            variables.SetString(key, value);
        }

        public void SetVector3(string key, Vector3 value)
        {
            variables.SetVector3(key, value);
        }
    }
}