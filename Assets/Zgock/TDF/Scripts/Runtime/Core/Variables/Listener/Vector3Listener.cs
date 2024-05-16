using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TotalDialogue.Core.Variables
{
    public class Vector3Listener : VariableListener<Vector3>
    {
        public override void SetValue(string key, Vector3 value)
        {
            variables.SetVector3(key, value);
        }
        public override Vector3 GetValue(string key)
        {
            return variables.GetVector3(key);
        }
    }
}