using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TotalDialogue.Core.Collections;
using UnityEngine;

namespace TotalDialogue.Core.Variables
{
    [System.Serializable]
    public class TDFVariables : IVariables
    {
        [SerializeField]
        private int m_maxChoice;
        public int MaxChoice { get => m_maxChoice;}
        [SerializeField]
        private int m_maxDialogue;
        public int MaxDialogue { get => m_maxDialogue; }
        [SerializeField]
        protected VariableList<bool,BoolVar,BoolListener> m_boolVars = new();
        [SerializeField]
        protected VariableList<int,IntVar,IntListener> m_intVars = new();
        [SerializeField]
        protected VariableList<float,FloatVar,FloatListener> m_floatVars = new();
        [SerializeField]
        protected VariableList<string,StringVar,StringListener> m_stringVars = new();
        [SerializeField]
        protected VariableList<Vector3,Vector3Var,Vector3Listener> m_vector3Vars = new();
        [SerializeField]
        protected VariableList<Quaternion,QuaternionVar,QuaternionListener> m_quaternionVars = new();
        [SerializeField]
        protected TDFGameObjectList m_objects = new();

        public bool GetBool(string key)
        {
            return m_boolVars[key].Value;
        }

        public float GetFloat(string key)
        {
            return m_floatVars[key].Value;
        }

        public int GetInt(string key)
        {
            return m_intVars[key].Value;
        }

        public Quaternion GetQuaternion(string key)
        {
            return m_quaternionVars[key].Value;
        }

        public string GetString(string key)
        {
            return m_stringVars[key].Value;
        }

        public Vector3 GetVector3(string key)
        {
            return m_vector3Vars[key].Value;
        }

        public void Reset()
        {
            m_boolVars.RestoreDefault();
            m_intVars.RestoreDefault();
            m_floatVars.RestoreDefault();
            m_stringVars.RestoreDefault();
            m_vector3Vars.RestoreDefault();
            m_quaternionVars.RestoreDefault();
        }

        public void SetBool(string key, bool value)
        {
            m_boolVars[key].Value = value;
        }

        public void SetFloat(string key, float value)
        {
            m_floatVars[key].Value = value;
        }

        public void SetInt(string key, int value)
        {
            m_intVars[key].Value = value;
        }

        public void SetQuaternion(string key, Quaternion value)
        {
            m_quaternionVars[key].Value = value;
        }

        public void SetString(string key, string value)
        {
            m_stringVars[key].Value = value;
        }

        public void SetVector3(string key, Vector3 value)
        {
            m_vector3Vars[key].Value = value;
        }

        public void AddListener(BoolListener listener)
        {
            if (m_boolVars.ContainsKey(listener.key))
            {
                m_boolVars[listener.key].AddListener(listener);
            }
        }

        public void RemoveListener(BoolListener listener)
        {
            if (m_boolVars.ContainsKey(listener.key))
            {
                m_boolVars[listener.key].RemoveListener(listener);
            }
        }

        public void AddListener(IntListener listener)
        {
            if (m_intVars.ContainsKey(listener.key))
            {
                m_intVars[listener.key].AddListener(listener);
            }
        }

        public void RemoveListener(IntListener listener)
        {
            if (m_intVars.ContainsKey(listener.key))
            {
                m_intVars[listener.key].RemoveListener(listener);
            }
        }

        public void AddListener(FloatListener listener)
        {
            if (m_floatVars.ContainsKey(listener.key))
            {
                m_floatVars[listener.key].AddListener(listener);
            }
        }

        public void RemoveListener(FloatListener listener)
        {
            if (m_floatVars.ContainsKey(listener.key))
            {
                m_floatVars[listener.key].RemoveListener(listener);
            }
        }

        public void AddListener(StringListener listener)
        {
            if (m_stringVars.ContainsKey(listener.key))
            {
                m_stringVars[listener.key].AddListener(listener);
            }
        }

        public void RemoveListener(StringListener listener)
        {
            if (m_stringVars.ContainsKey(listener.key))
            {
                m_stringVars[listener.key].RemoveListener(listener);
            }
        }

        public void AddListener(Vector3Listener listener)
        {
            if (m_vector3Vars.ContainsKey(listener.key))
            {
                m_vector3Vars[listener.key].AddListener(listener);
            }
        }

        public void RemoveListener(Vector3Listener listener)
        {
            if (m_vector3Vars.ContainsKey(listener.key))
            {
                m_vector3Vars[listener.key].RemoveListener(listener);
            }
        }

        public void AddListener(QuaternionListener listener)
        {
            if (m_quaternionVars.ContainsKey(listener.key))
            {
                m_quaternionVars[listener.key].AddListener(listener);
            }
        }

        public void RemoveListener(QuaternionListener listener)
        {
            if (m_quaternionVars.ContainsKey(listener.key))
            {
                m_quaternionVars[listener.key].RemoveListener(listener);
            }
        }

        public void InitSystemValues()
        {
            m_boolVars.Keys.ToList().Where(key => key.StartsWith("_")).ToList().ForEach(key => m_boolVars.Remove(key));
            m_intVars.Keys.ToList().Where(key => key.StartsWith("_")).ToList().ForEach(key => m_intVars.Remove(key));
            m_floatVars.Keys.ToList().Where(key => key.StartsWith("_")).ToList().ForEach(key => m_floatVars.Remove(key));
            m_stringVars.Keys.ToList().Where(key => key.StartsWith("_")).ToList().ForEach(key => m_stringVars.Remove(key));
            m_vector3Vars.Keys.ToList().Where(key => key.StartsWith("_")).ToList().ForEach(key => m_vector3Vars.Remove(key));
            m_quaternionVars.Keys.ToList().Where(key => key.StartsWith("_")).ToList().ForEach(key => m_quaternionVars.Remove(key));

            m_boolVars.Add(TDFConst.next,new BoolVar());
            m_boolVars.Add(TDFConst.cancel,new BoolVar());
            m_boolVars.Add(TDFConst.skip,new BoolVar());
            
            for (int i = 0;i < MaxDialogue;i++){
                m_boolVars.Add(TDFConst.writingKey + i,new BoolVar());
                m_intVars.Add(TDFConst.windowKey + i,new IntVar());
                m_boolVars.Add(TDFConst.nextableKey + i,new BoolVar());
                m_boolVars.Add(TDFConst.cancelableKey + i,new BoolVar());
                m_boolVars.Add(TDFConst.skippableKey + i,new BoolVar());
                m_boolVars.Add(TDFConst.acceptKey + i,new BoolVar());
                m_boolVars.Add(TDFConst.asyncKey + i,new BoolVar());
                m_boolVars.Add(TDFConst.clearKey + i,new BoolVar());
                m_stringVars.Add(TDFConst.nameKey + i,new StringVar());
                m_stringVars.Add(TDFConst.textKey + i,new StringVar());
            }
            for (int i = 0;i < MaxChoice;i++){
                m_intVars.Add(TDFConst.choosingKey + i,new IntVar());
                m_stringVars.Add(TDFConst.chooserKey + i,new StringVar());
                m_intVars.Add(TDFConst.choiceKey + i,new IntVar());
                m_boolVars.Add(TDFConst.blockChoiceKey + i,new BoolVar());
            }
        }

        public GameObject GetGameObject(string key)
        {
            return m_objects[key].GetGameObject();
        }

        public void SetGameObject(string key, GameObject value)
        {
            m_objects[key].SetGameObject(value);
        }

        public void SetSprite(string key, Sprite value)
        {
            m_objects[key].SetSprite(value);
        }

        public void SetAudioClip(string key, AudioClip value)
        {
            m_objects[key].SetAudioClip(value);
        }

        public void SetTexture(string key, Texture2D value)
        {
            m_objects[key].SetTexture(value);
        }
    }
}