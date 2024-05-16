using System.Collections;
using System.Collections.Generic;
using TotalDialogue.Core;
using TotalDialogue.Core.Variables;
using UnityEngine;
namespace TotalDialogue
{
    public class QuaternionWatcher : Watcher<Quaternion,QuaternionListener>
    {
        [SerializeField]
        private TDFScriptableVariables _variables;
        [SerializeField]
        private bool _fireOnEnable = true;
        protected override bool FireOnEnable { get { return _fireOnEnable; } set { _fireOnEnable = value; } }

        public override IVariables Variables
        {
            get { return _variables != null ? _variables : base.Variables; }
            set {
                if (value is TDFScriptableVariables)
                {
                    _variables = value as TDFScriptableVariables;
                }
                base.Variables = value;
            }
        }

        protected override void OnChanged(Quaternion value)
        {
            Vector3 eulerAngles = value.eulerAngles;
            text.text = eulerAngles.ToString();
        }
    }
}
