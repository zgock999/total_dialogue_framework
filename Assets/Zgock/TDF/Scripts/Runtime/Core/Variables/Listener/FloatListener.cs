using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TotalDialogue.Core.Variables
{
    public class FloatListener : VariableListener<float>
    {
        public override void SetValue(string key, float value)
        {
            variables.SetFloat(key, value);
        }
        public override float GetValue(string key)
        {
            return variables.GetFloat(key);
        }
    }
}