using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TotalDialogue.Core.Variables
{
    public class IntListener : VariableListener<int>
    {
        public override void SetValue(string key, int value)
        {
            variables.SetInt(key, value);
        }
        public override int GetValue(string key)
        {
            return variables.GetInt(key);
        }
    }
}