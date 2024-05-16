using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TotalDialogue.Core.Variables
{
    public interface IVariables
    {
        public void SetBool(string key, bool value);
        public bool GetBool(string key);
        public void SetInt(string key, int value);
        public int GetInt(string key);
        public void SetString(string key, string value);
        public string GetString(string key);
        public void SetFloat(string key, float value);
        public float GetFloat(string key);
        public void SetVector3(string key, Vector3 value);
        public Vector3 GetVector3(string key);
        public void SetQuaternion(string key, Quaternion value);
        public Quaternion GetQuaternion(string key);
        public void AddListener(BoolListener listener);
        public void RemoveListener(BoolListener listener);
        public void AddListener(IntListener listener);
        public void RemoveListener(IntListener listener);
        public void AddListener(FloatListener listener);
        public void RemoveListener(FloatListener listener);
        public void AddListener(StringListener listener);
        public void RemoveListener(StringListener listener);
        public void AddListener(Vector3Listener listener);
        public void RemoveListener(Vector3Listener listener);
        public void AddListener(QuaternionListener listener);
        public void RemoveListener(QuaternionListener listener);
        public void Reset();
        public int MaxChoice { get; }
        public int MaxDialogue { get;}
        public void InitSystemValues();
        public GameObject GetGameObject(string key);
        public void SetGameObject(string key, GameObject value);
        public void SetSprite(string key, Sprite value);
        public void SetAudioClip(string key, AudioClip value);
        public void SetTexture(string key, Texture2D value);
    }
}
