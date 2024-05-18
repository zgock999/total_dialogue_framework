using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using Cysharp.Threading.Tasks;
using TotalDialogue.Core.Variables;
using UnityEngine;
using UnityEngine.Events;

namespace TotalDialogue
{
    public abstract class Skipper : MonoBehaviour
    {
        public enum BatchType{
            All,
            Any,
            Sequence
        }
        public class Batch{
            public SkipSource cts;
            public List<UniTask> tasks = new();
            public async UniTask RunAll(){
                await UniTask.WhenAll(tasks).AttachExternalCancellation(cts.Token).SuppressCancellationThrow();
            }
            public async UniTask RunAny(){
                await UniTask.WhenAny(tasks).AttachExternalCancellation(cts.Token).SuppressCancellationThrow();
            }
            public async UniTask RunSequence(){
                foreach(UniTask task in tasks){
                    await task.AttachExternalCancellation(cts.Token).SuppressCancellationThrow();
                }
            }
        }
        protected Stack<Batch> batchStack = new();
        protected abstract ViewVariables ViewVariables { get; set; }
        public virtual IVariables Variables
        {
            get {
                if (ViewVariables != null && ViewVariables.Variables != null)
                {
                    return ViewVariables.Variables;
                }
                return null;
            }
            set {
                ViewVariables.Variables = null;
            }
        }        protected string nextBool = TDFConst.next;
        protected string cancelBool = TDFConst.cancel;
        protected string skipBool = TDFConst.skip;

        [SerializeField]
        private int m_id;

        public int Id => m_id;

        public void AddTask(UniTask task){
            batchStack.Peek().tasks.Add(task);
        }
        public List<UniTask> GetTasks(){
            return batchStack.Peek().tasks;
        }
        public void CancelBatch(){
            batchStack.Peek().cts.Cancel();
        }
        public void CancelAllBatch(){
            foreach(Batch batch in batchStack){
                batch.cts.Cancel();
            }
        }
        public void AppendBatch(Batch batch,BatchType type){
            batchStack.Peek().cts = batch.cts;
            switch(type){
                case BatchType.All:
                    batchStack.Peek().tasks.Add(batch.RunAll());
                    break;
                case BatchType.Any:
                    batchStack.Peek().tasks.Add(batch.RunAny());
                    break;
                case BatchType.Sequence:
                    batchStack.Peek().tasks.Add(batch.RunSequence());
                    break;
            }
        }
        public void BeginBatch(){
            Batch batch = new Batch();
            batchStack.Push(batch);
        }
        public async UniTask RunBatch(BatchType type,bool next,bool cancel,bool skip){
            SkipSource cts = GetSkipSource(next,cancel,skip);
            await RunBatch(type,cts);
        }
        public async UniTask RunBatch(BatchType type,SkipSource cts){
            Batch batch = batchStack.Pop();
            if (batch.tasks.Count == 0){
                return;
            }
            batch.cts = cts;
            if (batchStack.Count > 0){
                AppendBatch(batch,type);
            } else {
                switch(type){
                    case BatchType.All:
                        await batch.RunAll();
                        break;
                    case BatchType.Any:
                        await batch.RunAny();
                        break;
                    case BatchType.Sequence:
                        await batch.RunSequence();
                        break;
                }
            }
        }
        public async UniTask RunAll(bool next,bool cancel,bool skip){
            SkipSource cts = GetSkipSource(next,cancel,skip);
            await RunBatch(BatchType.All,cts);
        }
        public async UniTask RunAll(bool next,bool cancel,bool skip,UniTask task){
            SkipSource cts = GetSkipSource(next,cancel,skip);
            await RunBatch(BatchType.All,cts,task);
        }
        public async UniTask RunAll(SkipSource cts){
            await RunBatch(BatchType.All,cts);
        }
        public async UniTask RunAll(SkipSource cts,UniTask task){
            await RunBatch(BatchType.All,cts,task);
        }
        public async UniTask RunAny(bool next,bool cancel,bool skip){
            SkipSource cts = GetSkipSource(next,cancel,skip);
            await RunBatch(BatchType.Any,cts);
        }
        public async UniTask RunAny(bool next,bool cancel,bool skip,UniTask task){
            SkipSource cts = GetSkipSource(next,cancel,skip);
            await RunBatch(BatchType.Any,cts,task);
        }
        public async UniTask RunAny(SkipSource cts){
            await RunBatch(BatchType.Any,cts);
        }
        public async UniTask RunAny(SkipSource cts,UniTask task){
            await RunBatch(BatchType.Any,cts,task);
        }
        public async UniTask RunSequence(bool next,bool cancel,bool skip){
            SkipSource cts = GetSkipSource(next,cancel,skip);
            await RunBatch(BatchType.Sequence,cts);
        }
        public async UniTask RunSequence(bool next,bool cancel,bool skip,UniTask task){
            SkipSource cts = GetSkipSource(next,cancel,skip);
            await RunBatch(BatchType.Sequence,cts,task);
        }
        public async UniTask RunSequence(SkipSource cts){
            await RunBatch(BatchType.Sequence,cts);
        }
        public async UniTask RunSequence(SkipSource cts,UniTask task){
            await RunBatch(BatchType.Sequence,cts,task);
        }

        public async UniTask RunBatch(BatchType type,bool next,bool cancel,bool skip,UniTask task){
            SkipSource cts = GetSkipSource(next,cancel,skip);
            await RunBatch(type,cts,task);
        }

        public async UniTask RunBatch(BatchType type,SkipSource cts,UniTask task){
            AddTask(task);
            await RunBatch(type,cts);
        }
        public class SkipSource:CancellationTokenSource
        {
            public string guid;
            public bool next;
            public bool cancel;
            public bool skip;
        }

        private readonly ConcurrentDictionary<string,SkipSource> sources = new();

        private bool isAccepting(){
            for (int i = 0; i< Variables.MaxDialogue; i++){
                if (Variables.GetBool(TDFConst.acceptKey + i)){
                    return true;
                }
            }
            return false;
        }

        private async UniTaskVoid CheckSkip(CancellationToken token){
            try{
                for(;;){
                    List<SkipSource> toRemove = new();
                    foreach(SkipSource source in sources.Values){
                        if(source.next && Variables.GetBool(nextBool) && isAccepting()){
                            source.Cancel();
                            toRemove.Add(source);
                            ///Debug.Log(source.guid + " Nexted");
                            continue;
                        }
                        if(source.cancel && Variables.GetBool(cancelBool)){
                            source.Cancel();
                            toRemove.Add(source);
                            //Debug.Log(source.guid + " Canceled");
                            continue;
                        }
                        if(source.skip && Variables.GetBool(skipBool)){
                            source.Cancel();
                            toRemove.Add(source);
                            //Debug.Log(source.guid + " Skiped");
                            continue;
                        }
                    }
                    foreach(SkipSource source in toRemove){
                        sources.TryRemove(source.guid, out _);
                        //Debug.Log("Auto Removed. Sources = " + sources.Values.Count);
                    }
                    await UniTask.Yield(token);
                }
            } finally {
                foreach(SkipSource source in sources.Values){
                    source.Cancel();
                }
                sources.Clear();
            }
        }
        protected SkipSource GetSkipSource(bool canNext,bool canCancel,bool canSkip){
            SkipSource source = new()
            {
                guid = Guid.NewGuid().ToString("N"),
                next = canNext,
                cancel = canCancel,
                skip = canSkip
            };
            sources.TryAdd(source.guid,source);
            //Debug.Log(source.guid + "Added. Sources = " + sources.Count);
            return source;
        }
        protected void RemoveSource(SkipSource source){
            sources.TryRemove(source.guid, out _);
            //Debug.Log(source.guid + "Manual Removed. Sources = " + sources.Count);
        }
        protected virtual void Awake()
        {
            CheckSkip(this.GetCancellationTokenOnDestroy()).Forget();
        }
        protected void SetupListener(BoolListener listener,string key,UnityAction action){
            listener.variables = Variables;
            listener.key = key;
            listener.targetValue = true;
            listener.fireOnBecome = true;
            listener.OnBecome.AddListener(action);
            Variables.AddListener(listener);
        }
        protected void SetupListener(BoolListener listener,string key,UnityAction<bool> action){
            listener.variables = Variables;
            listener.key = key;
            listener.fireOnBecome = false;
            listener.OnChanged.AddListener(action);
            Variables.AddListener(listener);
        }
        protected void SetupListener(IntListener listener,string key,UnityAction<int> action){
            listener.variables = Variables;
            listener.key = key;
            listener.fireOnBecome = false;
            listener.OnChanged.AddListener(action);
            Variables.AddListener(listener);
        }
        protected void ShutDownListener(BoolListener listener,UnityAction action){
            Variables.RemoveListener(listener);
            listener.OnBecome.RemoveListener(action);
        }
        protected void ShutDownListener(BoolListener listener,UnityAction<bool> action){
            Variables.RemoveListener(listener);
            listener.OnChanged.RemoveListener(action);
        }
        protected void ShutDownListener(IntListener listener,UnityAction<int> action){
            Variables.RemoveListener(listener);
            listener.OnChanged.RemoveListener(action);
        }
    }
}
