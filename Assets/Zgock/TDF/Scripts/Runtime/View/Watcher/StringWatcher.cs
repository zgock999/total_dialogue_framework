using System.Collections;
using System.Collections.Generic;
using TotalDialogue.Core;
using TotalDialogue.Core.Variables;
using UnityEngine;

namespace TotalDialogue
{
    public class StringWatcher : Watcher<string,StringListener>
    {
        [SerializeField]
        private ViewVariables m_variables = new();
        protected override ViewVariables ViewVariables { get => m_variables; set => m_variables = value; }
        [SerializeField]
        private bool _fireOnEnable = true;
        protected override bool FireOnEnable { get { return _fireOnEnable; } set { _fireOnEnable = value; } }
        protected override void OnChanged(string value)
        {
            text.text = value;
        }
    }
}