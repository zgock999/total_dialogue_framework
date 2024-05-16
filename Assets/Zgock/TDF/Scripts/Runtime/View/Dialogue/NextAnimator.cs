using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TotalDialogue.View
{
    public class NextAnimator : MonoBehaviour
    {
        
        void OnEnable(){
            this.transform.localPosition = Vector3.zero;
            this.transform.DOLocalMoveY(-16f, 0.5f).SetLoops(-1,LoopType.Yoyo);
        }
        void OnDisable(){
            this.transform.DOKill();
            this.transform.localPosition = Vector3.zero;
        }
    }
}
