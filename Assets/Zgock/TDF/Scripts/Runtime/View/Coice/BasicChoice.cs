using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Threading;
namespace TotalDialogue{
    public class BasicChoice : EmptyChoice
    {
        [SerializeField]
        private ViewVariables m_variables = new();
        protected override ViewVariables ViewVariables { get => m_variables; set => m_variables = value; }
        public GameObject template;
        public GameObject selection;

        public float spacing = 120.0f;
        public float duration = 0.2f;

        protected List<GameObject> buttons = new();
        protected List<ColorBlock> colors = new();

        private void Choosen(string value){
            int choice = int.Parse(value);
            SetChoice(choice);
        }
        protected override void BecomeCurrent()
        {
            int start = Variables.GetInt(TDFConst.choiceStartKey);
            int depth = Variables.GetInt(TDFConst.choiceDepthKey);
            int lastChoice = Variables.GetInt(m_choiceKey + (start + depth));
            if (lastChoice != -1) return;
            if (Variables.GetInt(ChoosingKey) != 2) return;
            int choice = Variables.GetInt(ChoiceKey);
            for (int i = 0; i < buttons.Count; i++)
            {
                Button button = buttons[i].GetComponent<Button>();
                button.interactable = true;
                ColorBlock colors = button.colors;
                colors.disabledColor = colors.disabledColor;
                button.colors = colors;
                if (i == choice)
                {
                    button.Select();
                }
            }
            bool cancelable = Variables.GetBool(ChoiceCancelableKey);
            Variables.SetBool(ChoiceCancelableKey, false);
            Variables.SetInt(ChoiceKey, -1);
            Variables.SetBool(ChoiceCancelableKey, cancelable);
        }
        protected override void BecomeNotCurrent()
        {
            if (Variables.GetInt(ChoosingKey) != 2) return;
            int choice = Variables.GetInt(ChoiceKey);
            for (int i = 0; i < buttons.Count; i++)
            {
                Button button = buttons[i].GetComponent<Button>();
                button.interactable = false;
                ColorBlock colors = button.colors;
                if (i == choice)
                {
                    colors.disabledColor = colors.normalColor;
                }
                else
                {
                    colors.disabledColor = this.colors[i].disabledColor;
                }
                button.colors = colors;
            }
        }
        protected override async UniTask OpenChoiceAsync()
        {
            var token = this.GetCancellationTokenOnDestroy();
            string[] chooser = Variables.GetString(ChooserKey).Split(",");
            buttons.Clear();
            colors.Clear();
            for (int i = 0; i < chooser.Length; i++)
            {
                GameObject choice = Instantiate(template, selection.transform);
                choice.name = "Choice" + i;
                choice.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                choice.GetComponent<ChoiceEventTrigger>().id = i;
                choice.GetComponentInChildren<TextMeshProUGUI>().text = chooser[i];
                string sel = "" + i;
                choice.GetComponent<Button>().onClick.AddListener(() => Choosen(sel));
                choice.GetComponent<Button>().onClick.AddListener(() =>
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(choice);
                });
                buttons.Add(choice);
                colors.Add(choice.GetComponent<Button>().colors);
                choice.SetActive(true);
                Image img = choice.GetComponent<Image>();
                var color = img.color;
                color.a = 0f;
                img.color = color;

                // DoTweenを使わずにアニメーションを実装
                _ = FadeIn(img, 1f, duration,token);
                _ = MoveTo(choice.GetComponent<RectTransform>(), new Vector2(0, -i * spacing), duration,token);
            }
            await UniTask.Delay(TimeSpan.FromSeconds(duration),cancellationToken:token);
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -i * spacing);
            }
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(buttons[0]);
        }

        protected override async UniTask CloseChoiceAsync()
        {
            var token = this.GetCancellationTokenOnDestroy(); 
            foreach (GameObject button in buttons)
            {
                _ = MoveTo(button.GetComponent<RectTransform>(), new Vector2(0, 0), duration, token);
            }
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: token);
            foreach (GameObject button in buttons)
            {
                button.SetActive(false);
                GameObject.Destroy(button);
            }
            buttons.Clear();
            //Debug.Log("Choice Closed");
        }
        private async UniTask FadeIn(Image img, float targetAlpha, float duration, CancellationToken token)
        {
            float elapsed = 0f;
            Color startColor = img.color;
            try
            {
                while (elapsed < duration)
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }
        
                    elapsed += Time.deltaTime;
                    float normalizedTime = elapsed / duration;
                    Color newColor = new Color(startColor.r, startColor.g, startColor.b, normalizedTime * targetAlpha);
                    img.color = newColor;
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }
            }
            finally
            {
                Color finalColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
                img.color = finalColor;
            }
        }
        private async UniTask MoveTo(RectTransform rectTransform, Vector2 targetPosition, float duration, CancellationToken token)
        {
            float elapsed = 0f;
            Vector2 startPosition = rectTransform.anchoredPosition;
            try
            {
                while (elapsed < duration)
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }
        
                    elapsed += Time.deltaTime;
                    float normalizedTime = elapsed / duration;
                    Vector2 newPosition = Vector2.Lerp(startPosition, targetPosition, normalizedTime);
                    rectTransform.anchoredPosition = newPosition;
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }
            }
            finally
            {
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = targetPosition;
                }
            }
        }
    }
}
