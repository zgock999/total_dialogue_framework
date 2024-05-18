using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Linq;
using System.Threading;
using TMPro;
using System;
using System.Runtime.CompilerServices;
using UnityEngine.Events;
using TotalDialogue.Core.Variables;

namespace TotalDialogue.View
{
    public class BasicDialogue : Skipper
    {
        [SerializeField]
        private ViewVariables m_variables = new();
        protected override ViewVariables ViewVariables { get => m_variables; set => m_variables = value; }
        public TextMeshProUGUI characterNameText;
        public TextMeshProUGUI dialogueLineText;
        public GameObject nextSymbol;

        public Transform window;

        protected string m_windowKey = TDFConst.windowKey;
        protected string m_writingKey = TDFConst.writingKey;
        protected string m_clearKey = TDFConst.clearKey;
        protected string m_nextableKey = TDFConst.nextableKey;
        protected string m_cancelableKey = TDFConst.cancelableKey;
        protected string m_skipableKey = TDFConst.skippableKey;
        protected string m_nameKey = TDFConst.nameKey;
        protected string m_textKey = TDFConst.textKey;
        protected string m_acceptKey = TDFConst.acceptKey;


        protected string WritingKey => m_writingKey + Id;
        protected string WindowKey => m_windowKey + Id;
        protected string ClearKey => m_clearKey + Id;
        protected string NameKey => m_nameKey + Id;
        protected string TextKey => m_textKey + Id;
        protected string AcceptKey => m_acceptKey + Id;
        protected string NextableKey => m_nextableKey + Id;
        protected string CancelableKey => m_cancelableKey + Id;
        protected string SkipableKey => m_skipableKey + Id;
        protected string NextKey => TDFConst.next;
        protected string CancelKey => TDFConst.cancel;
        protected string SkipKey => TDFConst.skip;
        public float windowOpenDuration = 0.3f;
        public float characterDuration = 0.3f;
        public float characterInterval = 0.05f;

        protected BoolListener writingListener = new();
        protected IntListener windowListener = new();
        protected virtual async UniTask OpenDialogue(bool clear = true, bool next = false, bool cancel = false, bool skip = true)
        {
            SkipSource source = GetSkipSource(next, cancel, skip);
            if (clear)
            {
                characterNameText.text = string.Empty;
                dialogueLineText.text = string.Empty;
            }
            window.localScale = Vector3.zero;
            nextSymbol.SetActive(false);
            window.gameObject.SetActive(true);
        
            try
            {
                float elapsedTime = 0f;
                while (elapsedTime < windowOpenDuration)
                {
                    if (source.Token.IsCancellationRequested)
                    {
                        source.Token.ThrowIfCancellationRequested();
                    }
        
                    elapsedTime += Time.deltaTime;
                    float scale = elapsedTime / windowOpenDuration;
                    window.localScale = new Vector3(scale, scale, scale);
        
                    await UniTask.Yield(PlayerLoopTiming.Update, source.Token);
                }
            }
            finally
            {
                window.localScale = Vector3.one;
                RemoveSource(source);
            }
        }    
        protected virtual async UniTask CloseDialogue(bool clear = true, bool next = false, bool cancel = false, bool skip = true)
        {
            SkipSource source = GetSkipSource(next, cancel, skip);
            if (clear)
            {
                characterNameText.text = string.Empty;
                dialogueLineText.text = string.Empty;
            }
            nextSymbol.SetActive(false);

            try
            {
                float elapsedTime = 0f;
                while (elapsedTime < windowOpenDuration)
                {
                    if (source.Token.IsCancellationRequested)
                    {
                        source.Token.ThrowIfCancellationRequested();
                    }

                    elapsedTime += Time.deltaTime;
                    float scale = 1f - (elapsedTime / windowOpenDuration);
                    window.localScale = new Vector3(scale, scale, scale);

                    await UniTask.Yield(PlayerLoopTiming.Update, source.Token);
                }
            }
            finally
            {
                window.gameObject.SetActive(false);
                window.localScale = Vector3.zero;
                RemoveSource(source);
            }
        }
        protected virtual async UniTask  WriteDialogue(string name,string line,bool next = false,bool cancel = true,bool skip = true,bool clear = true){
            //Debug.Log("Writing Start.");
            SkipSource source = GetSkipSource(false, cancel, skip);
            SkipSource source2 = GetSkipSource(next, false, skip);
            characterNameText.text = name;
            int startPos = 0;
            if (clear){
                dialogueLineText.text = line;
            } else {
                startPos = dialogueLineText.textInfo.characterCount;
                dialogueLineText.text += line;
            }
            nextSymbol.SetActive(false);
            TMP_TextInfo info = dialogueLineText.textInfo;
            TMP_CharacterInfo[] characterInfo = info.characterInfo;
            dialogueLineText.ForceMeshUpdate();
            byte [] originalAlpha = new byte[info.characterCount];
            for (int i = 0;i < info.characterCount;i++){
                TMP_CharacterInfo ci = info.characterInfo[i];
                int materialIndex = ci.materialReferenceIndex;
                int vertexIndex = ci.vertexIndex;
                originalAlpha[i] = info.meshInfo[materialIndex].colors32[vertexIndex].a;
                if (i >= startPos && ci.character != ' ' && ci.character != '\n'){
                    info.meshInfo[materialIndex].colors32[vertexIndex].a = 0;
                    info.meshInfo[materialIndex].colors32[vertexIndex + 1].a = 0;
                    info.meshInfo[materialIndex].colors32[vertexIndex + 2].a = 0;
                    info.meshInfo[materialIndex].colors32[vertexIndex + 3].a = 0;
                }
            }
            try
            {
                for (int i = startPos;i < info.characterCount;i++){
                    //dialogueLineText.maxVisibleCharacters = i + 1;
                    TMP_CharacterInfo ci = info.characterInfo[i];
                    if (ci.character == ' ' || ci.character == '\n'){
                        continue;
                    }
                    FadeCharacter(info,i,originalAlpha[i],source.Token).Forget();
                    if (await UniTask.Delay(TimeSpan.FromSeconds(characterInterval), cancellationToken: source.Token).SuppressCancellationThrow())
                    {
                        for (int j = i + 1;j < info.characterCount;j++){
                            TMP_CharacterInfo cj = info.characterInfo[j];
                            int materialIndex = cj.materialReferenceIndex;
                            int vertexIndex = cj.vertexIndex;
                            if (cj.character != ' ' && cj.character != '\n'){
                                info.meshInfo[materialIndex].colors32[vertexIndex].a = originalAlpha[j];
                                info.meshInfo[materialIndex].colors32[vertexIndex + 1].a = originalAlpha[j];
                                info.meshInfo[materialIndex].colors32[vertexIndex + 2].a = originalAlpha[j];
                                info.meshInfo[materialIndex].colors32[vertexIndex + 3].a = originalAlpha[j];
                            }
                        }
                        dialogueLineText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                        break;

                    }
                }
                //dialogueLineText.maxVisibleCharacters = info.characterCount + 1;
                if (next){
                    nextSymbol.SetActive(true);
                    Variables.SetBool(NextKey, false);
                    Variables.SetBool(AcceptKey, true);
                    await UniTask.WaitWhile(() => Variables.GetBool(WritingKey), cancellationToken: source2.Token).SuppressCancellationThrow();
                    Variables.SetBool(AcceptKey, false);
                    nextSymbol.SetActive(false);
                }
            }
            finally
            {
                nextSymbol.SetActive(false);
                Variables.SetBool(WritingKey, false);
                RemoveSource(source);
                RemoveSource(source2);
            }
        }
        protected virtual async UniTask FadeCharacter(TMP_TextInfo info, int index, byte original, CancellationToken token)
        {
            TMP_CharacterInfo ci = info.characterInfo[index];
            int materialIndex = ci.materialReferenceIndex;
            int vertexIndex = ci.vertexIndex;
            var colors32 = info.meshInfo[materialIndex].colors32;
            Color32 originalColor = colors32[vertexIndex];
            Color color = originalColor;
            float targetAlpha = original / 255.0f;
            color.a = 0;

            try
            {
                float elapsedTime = 0f;
                while (elapsedTime < characterDuration)
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    elapsedTime += Time.deltaTime;
                    float alpha = Mathf.Lerp(0, targetAlpha, elapsedTime / characterDuration);
                    Color32 newColor = new Color32(originalColor.r, originalColor.g, originalColor.b, (byte)(alpha * 255));
                    colors32[vertexIndex] = newColor;
                    colors32[vertexIndex + 1] = newColor;
                    colors32[vertexIndex + 2] = newColor;
                    colors32[vertexIndex + 3] = newColor;
                    dialogueLineText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }
            }
            finally
            {
                colors32[vertexIndex].a = original;
                colors32[vertexIndex + 1].a = original;
                colors32[vertexIndex + 2].a = original;
                colors32[vertexIndex + 3].a = original;
                dialogueLineText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            }
        }
        protected virtual async UniTask Wait(bool next = true,bool cancel = false,bool skip = true){
            SkipSource source = GetSkipSource(next, cancel, skip);
            await UniTask.WaitUntilCanceled(source.Token).SuppressCancellationThrow();
            RemoveSource(source);
        }

        protected virtual async UniTask Wait(CancellationToken token){
            await UniTask.WaitUntilCanceled(token).SuppressCancellationThrow();
        }
        public void ChangeState(int state){
            ChangeStateAsync(state).Forget();
        }

        protected async UniTask ChangeStateAsync(int state){
            switch(state){
                case 1:
                    await OpenDialogue(Variables.GetBool(ClearKey));
                    Variables.SetInt(WindowKey,2);
                    break;
                case 3:
                    await CloseDialogue(Variables.GetBool(ClearKey));
                    Variables.SetInt(WindowKey,0);
                    break;
                default:
                    break;
            }
        }
        public void StartWrite(){
            StartWriteAsync().Forget();
        }

        protected async UniTask StartWriteAsync(){
            bool next = Variables.GetBool(NextableKey);
            bool cancel = Variables.GetBool(CancelableKey);
            bool skip = Variables.GetBool(SkipableKey);
            bool clear = Variables.GetBool(ClearKey);
            string line = Variables.GetString(TextKey);
            string name = Variables.GetString(NameKey);
            await WriteDialogue(name, line, next,cancel,skip,clear).SuppressCancellationThrow();
            Variables.SetBool(WritingKey, false);
        }
        protected virtual void OnEnable(){
            SetupListener(writingListener,WritingKey,StartWrite);
            SetupListener(windowListener,WindowKey,ChangeState);
        }
        protected virtual void OnDisable(){
            ShutDownListener(writingListener,StartWrite);
            ShutDownListener(windowListener,ChangeState);
        }
    }
    
}


