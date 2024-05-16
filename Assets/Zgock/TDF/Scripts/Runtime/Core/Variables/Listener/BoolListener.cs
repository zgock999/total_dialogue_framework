using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TotalDialogue.Core.Variables
{
    public class BoolListener : VariableListener<bool>
    {
        public override void SetValue(string key, bool value)
        {
            variables.SetBool(key, value);
        }
        public override bool GetValue(string key)
        {
            return variables.GetBool(key);
        }
    }
}
