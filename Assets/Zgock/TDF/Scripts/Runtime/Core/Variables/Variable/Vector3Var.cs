using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TotalDialogue.Core.Variables
{
    [System.Serializable]
    public class Vector3Var : TDFVar<Vector3,Vector3Listener>
    {
        public override string TypeName => "Vector3";
    }
}