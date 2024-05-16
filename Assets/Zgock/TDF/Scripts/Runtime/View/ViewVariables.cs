using System.Collections;
using System.Collections.Generic;
using TotalDialogue.Core;
using TotalDialogue.Core.Variables;
using UnityEngine;
namespace TotalDialogue
{
    [System.Serializable]
    public class ViewVariables
    {
        public TDFScriptableVariables scriptableVariables;
        public TDFComponentVariables componentVariables;
        public IVariables Variables{
            get {
                if (scriptableVariables != null)
                {
                    return scriptableVariables;
                }
                if (componentVariables != null)
                {
                    return componentVariables;
                }
                return null;
            }
            set {
                if (value == null)
                {
                    scriptableVariables = null;
                    componentVariables = null;
                    return;
                }
                if (value is TDFScriptableVariables)
                {
                    componentVariables = null;
                    scriptableVariables = value as TDFScriptableVariables;
                }
                if (value is TDFComponentVariables)
                {
                    scriptableVariables = null;
                    componentVariables = value as TDFComponentVariables;
                }
                else
                {
                    scriptableVariables = null;
                    componentVariables = null;
                }
            }
        }
    }
}