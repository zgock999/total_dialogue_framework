using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TotalDialogue.Core.Variables
{
    [System.Serializable]
    public class StringVar : TDFVar<string,StringListener>
    {
        public override string TypeName => "String";
    }
}