using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TotalDialogue.Core.Variables
{
    [System.Serializable]
    public class IntVar : TDFVar<int,IntListener>
    {
        public override string TypeName => "Int";
    }
}