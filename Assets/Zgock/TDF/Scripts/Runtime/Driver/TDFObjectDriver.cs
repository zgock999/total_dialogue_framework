using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TotalDialogue
{
    public partial class TDFDriver : Skipper
    {
        protected Dictionary<string,GameObject> instances = new();
        [SerializeField] public List<Transform> layers2D = new();
        public void Spawn(string objectVar,Transform parent = null)
        {
            GameObject prefab = Variables.GetGameObject(objectVar);
            if (prefab == null) return;
            bool active = prefab.activeSelf;
            prefab.SetActive(false);
            GameObject newObject = Instantiate(prefab,parent);
            prefab.SetActive(active);
            if (newObject == null) return;
            if (instances.ContainsKey(objectVar))
            {
                Destroy(instances[objectVar]);
                instances.Remove(objectVar);
            }
            instances.Add(objectVar,newObject);
        }
        public void Dispose(string objectVar)
        {
            if (instances.ContainsKey(objectVar))
            {
                Destroy(instances[objectVar]);
                instances.Remove(objectVar);
            }
        }
        public void Spawn2D(string objectVar,int layer = 0,float alpha = -1,string positionVar = null,bool active = true){
            Spawn(objectVar,layers2D[layer]);
            GameObject newObject = instances[objectVar];
            if (newObject == null) return;
            if (alpha >= 0){
                Image[] images = newObject.GetComponentsInChildren<Image>();
                foreach(var image in images){
                    image.color = new Color(image.color.r,image.color.g,image.color.b,alpha);
                }
                RawImage[] raws = newObject.GetComponentsInChildren<RawImage>();
                foreach(var image in raws){
                    image.color = new Color(image.color.r,image.color.g,image.color.b,alpha);
                }
            }
            TransformBinder binder = newObject.GetComponent<TransformBinder>() ?? newObject.AddComponent<TransformBinder>();
            if (!String.IsNullOrEmpty(positionVar)){
                binder.positionVar = positionVar;
                newObject.transform.position = Variables.GetVector3(positionVar);
            }
            newObject.SetActive(active);
        }
 
        public void Spawn3D(string objectVar,string positionVar = null,bool active = true){
            Spawn(objectVar);
            GameObject newObject = instances[objectVar];
            if (newObject == null) return;
            TransformBinder binder = newObject.GetComponent<TransformBinder>() ?? newObject.AddComponent<TransformBinder>();
            if (!String.IsNullOrEmpty(positionVar)){
                binder.positionVar = positionVar;
                newObject.transform.position = Variables.GetVector3(positionVar);
            }
            newObject.SetActive(active);
        }
 
        public void Dispose2D(string objectVar){
            Dispose(objectVar);
        }

        public GameObject GetInstance(string objectVar){
            if (instances.ContainsKey(objectVar)) return instances[objectVar];
            return null;
        }
        public async UniTask MoveTo2D(string objectVar,float x,float y,float duration,bool next = false, bool cancel = true, bool skip = true, bool async = true,float delay = 0){
            GameObject go = GetInstance(objectVar);
            if (go == null) return;
            RectTransform rect = go.GetComponent<RectTransform>();
            if (duration <= 0){
                rect.anchoredPosition = new Vector2(x,y);
                return;
            }
            if (async){
                MoveTo2DAsync(objectVar,x,y,duration,next,cancel,skip,delay).Forget();
            } else {
                await MoveTo2DAsync(objectVar,x,y,duration,next,cancel,skip,delay);
            }
        }
        public async UniTask MoveToX2D(string objectVar,float x,float duration,bool next = false, bool cancel = true, bool skip = true, bool async = true,float delay = 0){
            GameObject go = GetInstance(objectVar);
            if (go == null) return;
            RectTransform rect = go.GetComponent<RectTransform>();
            float y =  rect.anchoredPosition.y;
            if (duration <= 0){
                rect.anchoredPosition = new Vector2(x,y);
                return;
            }
            if (async){
                MoveTo2DAsync(objectVar,x,y,duration,next,cancel,skip,delay).Forget();
            } else {
                await MoveTo2DAsync(objectVar,x,y,duration,next,cancel,skip,delay);
            }
        }

        public async UniTask MoveToY2D(string objectVar,float y,float duration,bool next = false, bool cancel = true, bool skip = true, bool async = true,float delay = 0){
            GameObject go = GetInstance(objectVar);
            if (go == null) return;
            RectTransform rect = go.GetComponent<RectTransform>();
            float x =  rect.anchoredPosition.x;
            if (duration <= 0){
                rect.anchoredPosition = new Vector2(x,y);
                return;
            }
            if (async){
                MoveTo2DAsync(objectVar,x,y,duration,next,cancel,skip,delay).Forget();
            } else {
                await MoveTo2DAsync(objectVar,x,y,duration,next,cancel,skip,delay);
            }
        }
        private async UniTask MoveTo2DAsync(string objectVar,float x,float y,float duration, bool next, bool cancel, bool skip,float delay){
            GameObject go = GetInstance(objectVar);
            if (go == null) return;
            SkipSource source = GetSkipSource(next,cancel,skip);
            RectTransform trn = go.GetComponent<RectTransform>();
            try {
                if (delay > 0f){
                    SkipSource source2 = GetSkipSource(false,cancel,skip);
                    await UniTask.WaitForSeconds(delay,cancellationToken: source2.Token);
                }
                await GetInstance(objectVar).GetComponent<RectTransform>().DOAnchorPos(new Vector2(x,y),duration).ToUniTask(cancellationToken: source.Token);
            } finally {
                trn.anchoredPosition = new Vector2(x,y);
            }
        }
        public async UniTask YoyoX2D(string objectVar,float target,float duration,bool async = true,float delay = 0){
            if (duration <= 0) return;
            if (async){
                YoyoX2DAsync(objectVar,target,duration,delay).Forget();
            } else {
                await YoyoX2DAsync(objectVar,target,duration,delay);
            }
        }
        public async UniTask YoyoY2D(string objectVar,float target,float duration,bool async = true, float delay = 0){
            if (duration <= 0) return;
            if (async){
                YoyoY2DAsync(objectVar,target,duration,delay).Forget();
            } else {
                await YoyoY2DAsync(objectVar,target,duration,delay);
            }
        }
        private async UniTask YoyoX2DAsync(string objectVar,float target,float duration,float delay){
            GameObject go = GetInstance(objectVar);
            if (go == null) return;
            SkipSource source = GetSkipSource(true,false,true);
            RectTransform trn = go.GetComponent<RectTransform>();
            Vector2 pos = new Vector2(trn.anchoredPosition.x,trn.anchoredPosition.y);
            try {
                if (delay > 0f){
                    SkipSource source2 = GetSkipSource(false,true,true);
                    await UniTask.WaitForSeconds(delay,cancellationToken: source2.Token);
                }
                await GetInstance(objectVar).GetComponent<RectTransform>().DOAnchorPosX(target,duration).SetLoops(-1,LoopType.Yoyo).ToUniTask(cancellationToken: source.Token);
            } finally {
                float x = pos.x;
                float y = trn.anchoredPosition.y;
                trn.anchoredPosition = new Vector2(x,y);
            }
        }
        private async UniTask YoyoY2DAsync(string objectVar,float target,float duration,float delay){
            GameObject go = GetInstance(objectVar);
            if (go == null) return;
            SkipSource source = GetSkipSource(true,false,true);
            RectTransform trn = go.GetComponent<RectTransform>();
            Vector2 pos = new Vector2(trn.anchoredPosition.x,trn.anchoredPosition.y);
            try {
                if (delay > 0f){
                    SkipSource source2 = GetSkipSource(false,false,true);
                    await UniTask.WaitForSeconds(delay,cancellationToken: source2.Token);
                }
                await GetInstance(objectVar).GetComponent<RectTransform>().DOAnchorPosY(target,duration).SetLoops(-1,LoopType.Yoyo).ToUniTask(cancellationToken: source.Token);
            } finally {
                float x = trn.anchoredPosition.x;
                float y = pos.y;
                trn.anchoredPosition = new Vector2(x,y);
            }
        }
         public async UniTask Alpha2D(string objectVar, float alpha, float duration, bool next, bool cancel, bool skip, bool async = false, float delay = 0){
            if (duration <= 0) {
                GameObject go = GetInstance(objectVar);
                if (go == null) return;
                Image[] images = go.GetComponentsInChildren<Image>();
                RawImage[] rawImages = go.GetComponentsInChildren<RawImage>();
                foreach (var image in images)
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
                }
                foreach (RawImage image in rawImages)
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
                }
            };
            if (async){
                Alpha2DAsync(objectVar,alpha,duration,next,cancel,skip,delay).Forget();
            } else {
                await Alpha2DAsync(objectVar,alpha,duration,next,cancel,skip,delay);
            }
        }
        private async UniTask Alpha2DAsync(string objectVar,float alpha,float duration, bool next, bool cancel, bool skip,float delay){
            GameObject go = GetInstance(objectVar);
            if (go == null) return;
            SkipSource source = GetSkipSource(next,cancel,skip);
            RectTransform trn = go.GetComponent<RectTransform>();
            Image[] images = go.GetComponentsInChildren<Image>();
            RawImage[] rawImages = go.GetComponentsInChildren<RawImage>();
            try {
                if (delay > 0f){
                    SkipSource source2 = GetSkipSource(false,false,true);
                    await UniTask.WaitForSeconds(delay,cancellationToken: source2.Token);
                }
                List<UniTask> tasks = new();
                foreach(Image image in images){
                    tasks.Add(DOTween.ToAlpha(() => image.color,color => image.color = color,alpha,duration).ToUniTask(cancellationToken: source.Token));
                }
                foreach(RawImage image in rawImages){
                    tasks.Add(DOTween.ToAlpha(() => image.color,color => image.color = color,alpha,duration).ToUniTask(cancellationToken: source.Token));
                }
                await UniTask.WhenAll(tasks);
            } finally {
                foreach(var image in images){
                    image.color = new Color(image.color.r,image.color.g,image.color.b,alpha);
                }
                foreach(RawImage image in rawImages){
                    image.color = new Color(image.color.r,image.color.g,image.color.b,alpha);
                }
            }
        }
        public UniTask Activate(string objectVar,bool active){
            GameObject go = GetInstance(objectVar);
            if (go != null){
                go.SetActive(active);
            }
            return UniTask.CompletedTask;
        }
   }
}