using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TotalDialogue.Core.Variables
{
    [System.Serializable]
    public class TDFGameObject
    {
        public enum ObjectType {
            Prefab,
            Sprite,
            Texture,
            AudioClip
        }
        public string key;
        public ObjectType objectType;
        public GameObject prefab = null;
        public Sprite sprite = null;
        public Texture2D texture = null;
        public AudioClip audioClip = null;
        public GameObject GetGameObject()
        {
            switch(objectType)
            {
                case ObjectType.Prefab:
                    return prefab;
            }
            return null;
        }
        public void SetGameObject(GameObject value)
        {
            objectType = ObjectType.Prefab;
            prefab = value;
        }
        public void SetSprite(Sprite value)
        {
            objectType = ObjectType.Sprite;
            sprite = value;
        }
        public void SetTexture(Texture2D value)
        {
            objectType = ObjectType.Texture;
            texture = value;
        }
        public void SetAudioClip(AudioClip value)
        {
            objectType = ObjectType.AudioClip;
            audioClip = value;
        }
        public Sprite GetSprite()
        {
            return sprite;
        }
        public Texture GetTexture()
        {
            return texture;
        }
        public AudioClip GetAudioClip()
        {
            return audioClip;
        }
    }
}
