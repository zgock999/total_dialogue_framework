using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TotalDialogue.Core.Variables
{
    public class StringListener : VariableListener<string>
    {
        public override void SetValue(string key, string value)
        {
            variables.SetString(key, value);
        }
        public override string GetValue(string key)
        {
            return variables.GetString(key);
        }
    }
}