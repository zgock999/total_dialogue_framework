using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TotalDialogue
{
    public partial class TDFDriver : Skipper
    {
        [SerializeField]
        private ViewVariables m_variables = new();
        protected override ViewVariables ViewVariables { get => m_variables; set => m_variables = value; }
        protected override void Awake()
        {
            base.Awake();
            Variables.Reset();
        }

        protected void SetCancellation(int id,bool next, bool cancel, bool skip,bool async){
            Variables.SetBool(TDFConst.nextableKey + id, next);
            Variables.SetBool(TDFConst.cancelableKey + id, cancel);
            Variables.SetBool(TDFConst.skippableKey + id, skip);
            Variables.SetBool(TDFConst.asyncKey + id, async);
        }
        public async UniTask WriteDialogue(string name, string text, bool next, bool cancel, bool skip, bool async,bool clear = true,int id = 0,bool setclear = true)
        {
            SetCancellation(id,next, cancel, skip, async);
            if (setclear) Variables.SetBool(TDFConst.clearKey + id,clear);
            if (name != null) Variables.SetString(TDFConst.nameKey + id,name);
            if (text != null) Variables.SetString(TDFConst.textKey + id,text);
            Variables.SetBool(TDFConst.writingKey + id, true);
            if (!async){
                await UniTask.WaitUntil(() => !Variables.GetBool(TDFConst.writingKey + id));
            }
        }
        public virtual async UniTask OpenWindow(bool clear = true, bool next = false, bool cancel = true, bool skip = true, bool async = false,int id = 0)
        {
            SetCancellation(id,next, cancel, skip, async);
            Variables.SetBool(TDFConst.clearKey + id, clear);
            Variables.SetInt(TDFConst.windowKey + id, 1);
            if (!async){
                await UniTask.WaitUntil(() => Variables.GetInt(TDFConst.windowKey + id) == 2);
            }
        }
        public virtual async UniTask CloseWindow(bool clear = true, bool next = false, bool cancel = true, bool skip = true, bool async = false,int id = 0,bool setclear = true)
        {
            SetCancellation(id,next, cancel, skip, async);
            if (setclear) Variables.SetBool(TDFConst.clearKey + id, clear);
            Variables.SetInt(TDFConst.windowKey + id, 3);
            if (!async){
                await UniTask.WaitUntil(() => Variables.GetInt(TDFConst.windowKey + id) == 0);
            }
        }
        public virtual async UniTask Choice(string chooser, bool next = false, bool cancel = false, bool skip = false, bool async = false,int id = 0,int depth = 0,bool cancelable = false){
            SetCancellation(id, false, cancel, skip, async);
            Variables.SetString(TDFConst.chooserKey + id, chooser);
            Variables.SetInt(TDFConst.currentChoiceKey, -1);
            for (int i = 0; i <= depth; i++)
            {
                Variables.SetInt(TDFConst.choiceKey + (id + i), -1);
            }
            Variables.SetInt(TDFConst.choosingKey + id, 1);
            Variables.SetInt(TDFConst.choiceStartKey, id);
            Variables.SetInt(TDFConst.choiceDepthKey, depth);
            Variables.SetBool(TDFConst.choiceCancelableKey + id, cancelable);
            if (!async){
                await UniTask.WaitUntil(() => Variables.GetInt(TDFConst.choosingKey + id) == 0);
            }
        }
    }
}
