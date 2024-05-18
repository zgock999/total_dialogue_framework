using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using TotalDialogue.Core.Variables;
using UnityEngine;
using UnityEngine.Events;

namespace TotalDialogue
{
    public abstract class EmptyChoice : Skipper
    {
        protected abstract override ViewVariables ViewVariables { get; set; }
        public int depth = 0;
        public UnityEvent OnSelected;
        public UnityEvent<int> OnSelectionChanged;

        protected IntListener choiceListener = new();
        protected IntListener choosenListener = new();
        protected IntListener currentChoiceListener = new();
        protected BoolListener choiceCancelListener = new();
        protected string m_choiceKey = TDFConst.choiceKey;
        public string ChoiceKey => m_choiceKey + Id;
        protected string m_choosingKey = TDFConst.choosingKey;
        protected string ChoosingKey => m_choosingKey + Id;
        protected string m_chooserKey = TDFConst.chooserKey;
        protected string ChooserKey => m_chooserKey + Id;
        protected string m_coiceCancelableKry = TDFConst.choiceCancelableKey;
        protected string ChoiceCancelableKey => m_coiceCancelableKry + Id;

        protected string CurrentChoiceKey => TDFConst.currentChoiceKey;

        protected int choiceOnNext = -1;

        protected virtual UniTask OpenChoiceAsync(){
            return UniTask.CompletedTask;
        }
        protected virtual UniTask CloseChoiceAsync(){
            return UniTask.CompletedTask;
        }
        protected async void ChangeState(int state){
            switch(state){
                case 0: // Idle
                    break;
                case 1: // open choice
                    // fire opening
                    Variables.SetBool(TDFConst.choiceCancel,false);
                    Variables.SetInt(CurrentChoiceKey,Id);
                    await OpenChoiceAsync();
                    SetState(2);
                    break;
                case 2: // choosing
                    break;
                case 3: // close choice
                    // fire closging
                    int start = Variables.GetInt(TDFConst.choiceStartKey);
                    if (Id == start){
                        Variables.SetInt(CurrentChoiceKey,-1);
                    } else {
                        Variables.SetInt(CurrentChoiceKey,Id - 1);
                    }
                    await CloseChoiceAsync();
                    SetState(0);
                    break;
            }
        }
        public void SetChoice(int choice){
            //Debug.Log("Choice Set to " + choice);
            Variables.SetInt(ChoiceKey,choice);
        }

        public virtual void ChangeChoice(int choice){
            //Debug.Log("Choice Change to " + choice);
        }

        public void SetState(int state){
            Variables.SetInt(ChoosingKey,state);
        }
        public int GetState(){
            return Variables.GetInt(ChoosingKey);
        }

        protected void OnChoosen(int choice){
            if (Variables.GetInt(CurrentChoiceKey) != Id){
                return;
            }
            if (Variables.GetInt(ChoosingKey) != 2){
                return;
            }
            if (!Variables.GetBool(ChoiceCancelableKey) && choice < 0){
                return;
            }
            int start = Variables.GetInt(TDFConst.choiceStartKey);
            int depth = Variables.GetInt(TDFConst.choiceDepthKey);
            Variables.SetInt(ChoiceKey,choice);
            if (choice < 0 && Id > start){
                BackToPreviousChoice();
            }
            if (Id == start + depth) /// if this is the last choice
            {
                if (choice >= 0)    /// not cancelled
                {
                    // close all choices
                    for (int i = Id - 1;i >= start;i--){
                        CloseChoice(i);
                    }
                }
                CloseChoice(Id);
            }
            else
            {
                // open next choice
                if (choice >= 0)    /// not cancelled
                {
                    OpenNextChoice();
                }
                else /// cancelled
                {
                    CloseChoice(Id);
                }
            }
            OnSelected.Invoke();
        }

        protected virtual void OnBlock(){

        }

        protected virtual void OnUnBlock(){

        }

        protected virtual void OnBlockChanged(bool value){
            if (value){
                OnBlock();
            } else {
                OnUnBlock();
            }
        }

        protected virtual void OnSelect(object sender, EventArgs e){
            if (sender is ChoiceEventTrigger trigger){
                TextMeshProUGUI text = trigger.transform.GetComponentInChildren<TextMeshProUGUI>();
                //Debug.Log("Selected " + text.text);
            }
        }
        protected virtual void OnDeselect(object sender, EventArgs e){
            if (sender is ChoiceEventTrigger trigger){
                TextMeshProUGUI text = trigger.transform.GetComponentInChildren<TextMeshProUGUI>();
                //Debug.Log("Deselected " + text.text);
            }
        }

        protected virtual void OnCancelChoice(){
            if (Variables.GetInt(CurrentChoiceKey) == Id && Variables.GetBool(ChoiceCancelableKey)){
                OnChoosen(-1);
            }
        }
        protected virtual void BecomeCurrent(){
        }
        protected virtual void BecomeNotCurrent(){
        }
        protected virtual void ChangeCurrent(int id){
            if (id == Id){
                BecomeCurrent();
            }
            else
            {
                BecomeNotCurrent();
            }
        }

        protected void OnEnable(){
            SetupListener(currentChoiceListener,CurrentChoiceKey,ChangeCurrent);
            SetupListener(choiceListener,ChoosingKey,ChangeState);
            SetupListener(choosenListener,ChoiceKey,OnChoosen);
            SetupListener(choiceCancelListener,TDFConst.choiceCancel,OnCancelChoice);
        }
        protected void OnDisable(){
            ShutDownListener(choiceCancelListener,OnCancelChoice);
            ShutDownListener(choosenListener,OnChoosen);
            ShutDownListener(choiceListener,ChangeState);
            ShutDownListener(currentChoiceListener,ChangeCurrent);
        }
        public void CloseChoice(int id){
            Variables.SetInt(m_choosingKey + id,3);
        }
        public void OpenNextChoice(string choices){
            Variables.SetString(m_chooserKey + (Id + 1),choices);
            Variables.SetInt(m_choosingKey + (Id + 1),1);
        }
        public void BackToPreviousChoice(){
            //Variables.SetInt(CurrentChoiceKey,Id - 1);
        }
        public void OpenNextChoice(){
            choiceOnNext = Variables.GetInt(ChoiceKey);
            Variables.SetInt(CurrentChoiceKey,Id + 1);
            Variables.SetInt(m_choiceKey + (Id + 1),-1);
            Variables.SetBool(m_coiceCancelableKry + (Id + 1),true);
            Variables.SetInt(m_choosingKey + (Id + 1),1);
        }
    }
}
