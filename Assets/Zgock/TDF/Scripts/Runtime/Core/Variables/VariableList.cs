using System;
using System.Collections;
using System.Collections.Generic;
using TotalDialogue.Core.Collections;
using UnityEngine;

namespace TotalDialogue.Core.Variables
{
    [Serializable]
    public class VariableList<T, TVar, TListener> : TDFDictionary<string, TVar>
        where TListener : VariableListener<T>
        where TVar : TDFVar<T,TListener>
    {
        public VariableList()
        {
            OnAdd += (Index, pair) => {
                pair.Value.Key = pair.Key;
            };
            OnChangeItem += (Index, pair) => {
                pair.Value.Key = pair.Key;
            };
        }
        public void StoreDefault()
        {
            foreach (var value in Values)
            {
                value.StoreDefault();
            }
        }
        public void RestoreDefault()
        {
            foreach (var value in Values)
            {
                value.RestoreDefault();
            }
        }
    }
}
