using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TotalDialogue.Core.Variables
{
    public interface IEmptyVar
    {
        public string Key { get; }
        public void StoreDefault();
        public void RestoreDefault();
        public string StringValue { get; }
    }
}