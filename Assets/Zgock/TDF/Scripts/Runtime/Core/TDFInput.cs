using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands;
using UnityEngine;


namespace TotalDialogue
{
    public class TDFInput : MonoBehaviour
    {
        private struct Child{
            public TDFInput input;
            public bool enabled;
        }
        List<Child> childs = new(); 
        [System.Serializable]
        public class InputResult{
            public enum Type{Bool,Int,Float,String}
            public Type type;
            public string Key;
            public string target;
        }
        [System.Serializable]
        public class InputEntry{
            public enum Type{ Key, Input, Mouse }
            public Type type;
            public KeyCode code;
            public string name;

        }
        [System.Serializable]
        public class Entry{
            public List<InputEntry> inputs;

            public bool IsDown(){
                foreach (var input in inputs)
                {
                    switch (input.type)
                    {
                        case InputEntry.Type.Key:
                            if (Input.GetKeyDown(input.code))
                            {
                                return true;
                            }
                            break;
                        case InputEntry.Type.Input:
                            if (Input.GetButtonDown(input.name))
                            {
                                return true;
                            }
                            break;
                        case InputEntry.Type.Mouse:
                            if (Input.GetMouseButtonDown(int.Parse(input.name)))
                            {
                                return true;
                            }
                            break;
                    }
                }
                return false;
            }
            public bool IsUp(){
                foreach (var input in inputs)
                {
                    switch (input.type)
                    {
                        case InputEntry.Type.Key:
                            if (Input.GetKeyUp(input.code))
                            {
                                return true;
                            }
                            break;
                        case InputEntry.Type.Input:
                            if (Input.GetButtonUp(input.name))
                            {
                                return true;
                            }
                            break;
                        case InputEntry.Type.Mouse:
                            if (Input.GetMouseButtonUp(int.Parse(input.name)))
                            {
                                return true;
                            }
                            break;
                    }
                }
                return false;
            }
            public bool IsPressed(){
                foreach (var input in inputs)
                {
                    switch (input.type)
                    {
                        case InputEntry.Type.Key:
                            if (Input.GetKey(input.code))
                            {
                                return true;
                            }
                            break;
                        case InputEntry.Type.Input:
                            if (Input.GetButton(input.name))
                            {
                                return true;
                            }
                            break;
                        case InputEntry.Type.Mouse:
                            if (Input.GetMouseButton(int.Parse(input.name)))
                            {
                                return true;
                            }
                            break;
                    }
                }
                return false;
            }
            public InputResult onDown;
            public InputResult onUp;
            public InputResult onPressed;
            public string toggle;
        }
        [SerializeField]
        private ViewVariables variables = new();

        public int priority;
        [SerializeField]
        private List<Entry> entries;

        public virtual Entry[] Entries { get => entries.ToArray();}

        protected virtual void SetEntry(InputResult result,bool flag){
            if (result.Key == string.Empty) return;
            switch(result.type){
                case InputResult.Type.Bool:
                    variables.Variables.SetBool(result.Key,flag);
                    break;
                case InputResult.Type.Int:
                    if (flag) variables.Variables.SetInt(result.Key, int.Parse(result.target));
                    break;
                case InputResult.Type.Float:
                    if (flag) variables.Variables.SetFloat(result.Key, float.Parse(result.target));
                    break;
                case InputResult.Type.String:
                    if (flag) variables.Variables.SetFloat(result.Key, float.Parse(result.target));
                    break;
            }
        }
        // Update is called once per frame
        protected virtual void Update()
        {
            foreach(Entry entry in Entries){
                bool down = entry.IsDown();
                bool up = entry.IsUp();
                bool pressed = entry.IsPressed();
                if (entry.onDown.Key != string.Empty){
                    SetEntry(entry.onDown,down);
                }
                if (entry.onUp.Key != string.Empty){
                    SetEntry(entry.onUp,up);
                }
                if (entry.onPressed.Key != string.Empty){
                    SetEntry(entry.onPressed,pressed);
                }
                if (entry.toggle != string.Empty){
                    if (down){
                        variables.Variables.SetBool(entry.toggle,!variables.Variables.GetBool(entry.toggle));
                    }
                }
            }
        }
        void OnEnable(){
            childs.Clear();
            TDFInput[] inputs = FindObjectsByType<TDFInput>(FindObjectsSortMode.None);
            foreach(var input in inputs){
                if (input.priority < priority){
                    childs.Add(new Child(){input = input,enabled = input.enabled});
                }
            }
            foreach(var child in childs){
                child.input.enabled = false;
            }
        }
        void OnDisable(){
            foreach(var child in childs){
                child.input.enabled = child.enabled;
            }
        }
    }
}
