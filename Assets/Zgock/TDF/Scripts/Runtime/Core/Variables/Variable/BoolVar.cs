using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TotalDialogue.Core.Variables
{
    [Serializable]
    public class BoolVar : TDFVar<bool,BoolListener>
    {
        public override string TypeName => "Bool";
    }
}