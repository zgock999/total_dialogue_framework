using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TotalDialogue{
    public partial class TDFDriver : Skipper
    {
        public void DoSetBool(string name, bool value){
            Variables.SetBool(name,value);
        }
        public void DoGetBool(string destination,string source){
            Variables.SetBool(destination,Variables.GetBool(source));
        }
        public void DoSetInt(string name, int value){
            Variables.SetInt(name,value);
        }
        public void DoGetInt(string destination,string source){
            Variables.SetInt(destination,Variables.GetInt(source));
        }
        public void DoSetString(string name, string value){
            Variables.SetString(name,value);
        }
        public void DoGetString(string destination,string source){
            Variables.SetString(destination,Variables.GetString(source));
        }
        public void DoSetFloat(string name, float value){
            Variables.SetFloat(name,value);
        }
        public void DoGetFloat(string destination,string source){
            Variables.SetFloat(destination,Variables.GetFloat(source));
        }
    }
}
