using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TotalDialogue
{
    public class ChoiceEventTrigger : EventTrigger
    {
        public EmptyChoice choice;
        public int id;
        public UnityEvent<GameObject> onSelect = new();
        public override void OnSelect(BaseEventData eventData) => onSelect.Invoke(this.gameObject);
        public UnityEvent<GameObject> onDeselect = new();
        public override void OnDeselect(BaseEventData eventData) => onDeselect.Invoke(this.gameObject);
        public UnityEvent<GameObject> onUpdateSelected = new();
        public override void OnUpdateSelected(BaseEventData eventData) => onUpdateSelected.Invoke(this.gameObject);
        void OnEnable(){
            if (choice == null){
                for(Transform t = transform;t != null;t = t.parent){
                    if(t.TryGetComponent<EmptyChoice>(out var c))
                    {
                        choice = c;
                        break;
                    }
                }
                if (choice == null) Debug.LogError("ChoiceEventTrigger: No EmptyChoice found in parent hierarchy",gameObject);
            }
            onSelect.AddListener(OnSelectLocal);
        }
        void OnDisable(){
            onSelect.RemoveListener(OnSelectLocal);
        }
        void OnSelectLocal(GameObject go){
            choice.ChangeChoice(id);
        }
    }
}
