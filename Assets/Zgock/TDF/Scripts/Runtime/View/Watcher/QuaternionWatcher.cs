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
        private ViewVariables m_variables = new();
        protected override ViewVariables ViewVariables { get => m_variables; set => m_variables = value; }
        [SerializeField]
        private bool _fireOnEnable = true;
        protected override bool FireOnEnable { get { return _fireOnEnable; } set { _fireOnEnable = value; } }
        protected override void OnChanged(Quaternion value)
        {
            Vector3 eulerAngles = value.eulerAngles;
            text.text = eulerAngles.ToString();
        }
    }
}
