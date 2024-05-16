using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TotalDialogue.Core.Variables
{
    [System.Serializable]
    public class QuaternionVar : TDFVar<Quaternion,QuaternionListener>
    {
        public override string TypeName => "Quaternion";
    }
}