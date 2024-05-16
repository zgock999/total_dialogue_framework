using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TotalDialogue.Core.Variables
{
    [Serializable]
    public class FloatVar : TDFVar<float,FloatListener>
    {
        public override string TypeName => "Float";
    }
}
 