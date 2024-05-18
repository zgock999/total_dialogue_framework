using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace TotalDialogue
{
    public class TDFForth : BasicForth
    {
        private readonly TDFDriver m_driver;
        private bool waitable = true;
        private bool cancelable = true;
        private bool skippable = true;
        private bool async = false;
        private bool clear = true;
        private int dialogue = 0;
        private int choiceid = 0;
        float alpha2d = 0f;
        float delay = 0f;
        public TDFForth(TDFDriver driver) : base()
        {
            m_driver = driver;

            RegistWord("getbool", GetBool);
            RegistWord("getint", GetInt);
            RegistWord("getfloat", GetFloat);
            RegistWord("getstring", GetString);

            RegistWord("setbool", SetBool);
            RegistWord("setint", SetInt);
            RegistWord("setfloat", SetFloat);
            RegistWord("setstring", SetString);

            RegistWord("=b", GetBool);
            RegistWord("=i", GetInt);
            RegistWord("=f", GetFloat);
            RegistWord("=s", GetString);

            RegistWord("b=", SetBool);
            RegistWord("i=", SetInt);
            RegistWord("f=", SetFloat);
            RegistWord("s=", SetString);

            RegistWord(".", Print);
            RegistWord("..", PrintContinue);
            RegistWord("open", Open);
            RegistWord("close", Close);
            RegistWord("wait", Wait);
            RegistWord("name", Name);

            RegistWord("dialogue", Dialogue);
            RegistWord("waitable", Waitable);
            RegistWord("cancelable", Cancelable);
            RegistWord("skippable", Skippable);
            RegistWord("async", Async);
            RegistWord("clear", Clear);
            RegistWord("delay", Delay);

            RegistWord("choice", Choice);
            RegistWord("choiceid", ChoiceId);

            RegistWord("spawn2D", Spawn2D);
            RegistWord("alpha2D", Alpha2D);
            RegistWord("dispose", Dispose);
            RegistWord("moveto2d", MoveTo2D);
            RegistWord("movetox2d", MoveToX2D);
            RegistWord("movetoy2d", MoveToY2D);
            RegistWord("yoyox2d", YoyoX2D);
            RegistWord("yoyoy2d", YoyoY2D);
        }

        private UniTask GetBool()
        {
            string key = stack.Pop();
            bool value = m_driver.Variables.GetBool(key);
            if (value){
                stack.Push("1");
            } else {
                stack.Push("0");
            }
            return UniTask.CompletedTask;
        }
        private UniTask GetInt()
        {
            string key = stack.Pop();
            int value = m_driver.Variables.GetInt(key);
            stack.Push(value.ToString());
            return UniTask.CompletedTask;
        }
        private UniTask GetFloat()
        {
            string key = stack.Pop();
            float value = m_driver.Variables.GetFloat(key);
            stack.Push(value.ToString());
            return UniTask.CompletedTask;
        }
        private UniTask GetString()
        {
            string key = stack.Pop();
            string value = m_driver.Variables.GetString(key);
            stack.Push(value);
            return UniTask.CompletedTask;
        }
        private UniTask SetBool()
        {
            string key = stack.Pop();
            string value = stack.Pop();
            if (float.TryParse(value,out float v)){
                m_driver.Variables.SetBool(key,v != 0);
            } else {
                m_driver.Variables.SetBool(key, false);
            }
            return UniTask.CompletedTask;
        }
        private UniTask SetInt()
        {
            string key = stack.Pop();
            string raw = stack.Pop();
            if (float.TryParse(raw,out float value)){
                m_driver.Variables.SetInt(key,(int)value);
            } else {
                m_driver.Variables.SetInt(key, 0);
            }
            return UniTask.CompletedTask;
        }
        private UniTask SetFloat()
        {
            string key = stack.Pop();
            string raw = stack.Pop();
            if (float.TryParse(raw,out float value)){
                m_driver.Variables.SetFloat(key,(int)value);
            } else {
                m_driver.Variables.SetFloat(key, 0);
            }
            return UniTask.CompletedTask;
        }
        private UniTask SetString()
        {
            string key = stack.Pop();
            string raw = stack.Pop();
            m_driver.Variables.SetString(key, raw);
            return UniTask.CompletedTask;
        }
        protected void SetStandard(){
            waitable = true;
            cancelable = true;
            skippable = true;
            async = false;
            clear = true;
        }
        protected async override UniTask Print()
        {
            string text = stack.Pop();
            await m_driver.WriteDialogue(null, text, waitable, cancelable, skippable, async,id:dialogue,clear:clear,setclear:true);
            SetStandard();
        }
        protected async UniTask PrintContinue()
        {
            string text = stack.Pop();
            await m_driver.WriteDialogue(null, text, false, cancelable, skippable, async,id:dialogue,clear:clear,setclear:true);
            SetStandard();
            clear = false;
        }
        private async UniTask Open(){
            await m_driver.OpenWindow(id:dialogue);
            SetStandard();
        }
        private async UniTask Close(){
            await m_driver.CloseWindow(id:dialogue);
            SetStandard();
        }
        private async UniTask Wait(){
            string raw = stack.Pop();
            if (float.TryParse(raw,out float value)){
                await UniTask.WaitForSeconds(value);
            }
        }
        private UniTask Dialogue(){
            string value = stack.Pop();
            if (int.TryParse(value,out int v)){
                dialogue = v;
            }
            return UniTask.CompletedTask;
        }
        private UniTask Waitable(){
            string value = stack.Pop();
            if (float.TryParse(value,out float v)){
                waitable = v != 0;
            }
            return UniTask.CompletedTask;
        }
        private UniTask Cancelable(){
            string value = stack.Pop();
            if (float.TryParse(value,out float v)){
                cancelable = v != 0;
            }
            return UniTask.CompletedTask;
        }
        private UniTask Skippable(){
            string value = stack.Pop();
            if (float.TryParse(value,out float v)){
                skippable = v != 0;
            }
            return UniTask.CompletedTask;
        }
        private UniTask Async(){
            string value = stack.Pop();
            if (float.TryParse(value,out float v)){
                async = v != 0;
            }
            return UniTask.CompletedTask;
        }
        private UniTask Delay(){
            string value = stack.Pop();
            if (float.TryParse(value,out float v)){
                delay = v;
            }
            return UniTask.CompletedTask;
        }
        private UniTask Clear(){
            string value = stack.Pop();
            if (float.TryParse(value,out float v)){
                clear = v != 0;
            }
            return UniTask.CompletedTask;
        }
        private UniTask Name(){
            string value = stack.Pop();
            m_driver.Variables.SetString(TDFConst.nameKey + dialogue,value);
            return UniTask.CompletedTask;
        }
        protected virtual async UniTask Choice()
        {
            string chooser = stack.Pop();
            await m_driver.Choice(chooser, id:choiceid);
            int choice = m_driver.Variables.GetInt(TDFConst.choiceKey + choiceid);
            stack.Push(choice.ToString());
        }
        private UniTask ChoiceId(){
            string value = stack.Pop();
            if (int.TryParse(value,out int v)){
                choiceid = v;
            }
            return UniTask.CompletedTask;
        }
        private UniTask Spawn2D(){
            string layerraw = stack.Pop();
            string target = stack.Pop();
            if (int.TryParse(layerraw,out int layer)){
                layer = 0;
            }
            m_driver.Spawn2D(target,layer,alpha:alpha2d);
            return UniTask.CompletedTask;
        }
        private UniTask Dispose(){
            string target = stack.Pop();
            m_driver.Dispose(target);
            return UniTask.CompletedTask;
        }
        private async UniTask MoveTo2D(){
            string durationRaw = stack.Pop();
            string yRaw = stack.Pop();
            string xRaw = stack.Pop();
            string target = stack.Pop();
            if (float.TryParse(yRaw,out float y)){
                if (float.TryParse(xRaw,out float x)){
                    if (float.TryParse(durationRaw,out float duration)){
                        await m_driver.MoveTo2D(target,x,y,duration,next:waitable,async: async,delay:delay);
                    }
                }
            }
            delay = 0f;
        }
        private async UniTask MoveToX2D(){
            string durationRaw = stack.Pop();
            string xRaw = stack.Pop();
            string target = stack.Pop();
            if (float.TryParse(xRaw,out float x)){
                if (float.TryParse(durationRaw,out float duration)){
                    await m_driver.MoveToX2D(target,x,duration,next:waitable,async: async,delay:delay);
                }
            }
            delay = 0f;
        }
        private async UniTask MoveToY2D(){
            string durationRaw = stack.Pop();
            string yRaw = stack.Pop();
            string target = stack.Pop();
            if (float.TryParse(yRaw,out float y)){
                if (float.TryParse(durationRaw,out float duration)){
                    await m_driver.MoveToY2D(target,y,duration,next:waitable,async: async,delay:delay);
                }
            }
        }
        private async UniTask YoyoX2D(){
            string durationRaw = stack.Pop();
            string xRaw = stack.Pop();
            string target = stack.Pop();
            if (float.TryParse(xRaw,out float x)){
                if (float.TryParse(durationRaw,out float duration)){
                    await m_driver.YoyoX2D(target,x,duration,async: async,delay:delay);
                }
            }
            delay = 0f;
        }
        private async UniTask YoyoY2D(){
            string durationRaw = stack.Pop();
            string yRaw = stack.Pop();
            string target = stack.Pop();
            if (float.TryParse(yRaw,out float y)){
                if (float.TryParse(durationRaw,out float duration)){
                    await m_driver.YoyoY2D(target,y,duration,async: async,delay:delay);
                }
            }
            delay = 0f;
        }
        private async UniTask Alpha2D(){
            string durationRaw = stack.Pop();
            string aRaw = stack.Pop();
            string target = stack.Pop();
            if (float.TryParse(aRaw,out float alpha)){
                if (float.TryParse(durationRaw,out float duration)){
                    await m_driver.Alpha2D(target,alpha,duration,waitable,cancelable,skippable,async: async);
                }
            }
            delay = 0f;
        }
    }
}

