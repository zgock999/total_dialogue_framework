using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TotalDialogue.Core.Variables
{
    public class QuaternionListener : VariableListener<Quaternion>
    {
        public override Quaternion GetValue(string key)
        {
            return variables.GetQuaternion(key);
        }

        public override void SetValue(string key, Quaternion value)
        {
            variables.SetQuaternion(key, value);
        }
    }
}