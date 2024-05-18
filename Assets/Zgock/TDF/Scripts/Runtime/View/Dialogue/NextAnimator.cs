using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TotalDialogue.View
{
    public class NextAnimator : MonoBehaviour
    {
        private CancellationTokenSource cts;

        void OnEnable()
        {
            this.transform.localPosition = Vector3.zero;
            cts = new CancellationTokenSource();
            MoveLoop(cts.Token).Forget();
        }

        void OnDisable()
        {
            cts.Cancel();
            this.transform.localPosition = Vector3.zero;
        }

        private async UniTaskVoid MoveLoop(CancellationToken token)
        {
            while (true)
            {
                await MoveY(-16f, 0.5f, token);
                await MoveY(0f, 0.5f, token);
            }
        }

        private async UniTask MoveY(float target, float duration, CancellationToken token)
        {
            Vector3 start = transform.localPosition;
            Vector3 end = new Vector3(start.x, target, start.z);
            float elapsed = 0f;

            // AnimationCurveを定義します。ここではEaseInOut型のカーブを使用しています。
            AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

            while (elapsed < duration)
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                elapsed += Time.deltaTime;
                float normalizedTime = elapsed / duration;
                float y = curve.Evaluate(normalizedTime) * (end.y - start.y) + start.y;
                transform.localPosition = new Vector3(start.x, y, start.z);

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            transform.localPosition = end;
        }
    }
}
