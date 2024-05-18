using UnityEngine;
namespace TotalDialogue
{
    public enum EaseType
    {
        Linear,
        InQuad,
        OutQuad,
        InOutQuad,
        InSine,
        OutSine,
        InOutSine,
        // 他のEaseタイプもここに追加できます
    }

    public static class EaseCurve
    {
        public static AnimationCurve GetCurve(EaseType easeType)
        {
            switch (easeType)
            {
                case EaseType.Linear:
                    return new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
                case EaseType.InQuad:
                    return new AnimationCurve(new Keyframe(0, 0, 0, 2), new Keyframe(1, 1, 0, 0));
                case EaseType.OutQuad:
                    return new AnimationCurve(new Keyframe(0, 0, 0, 0), new Keyframe(1, 1, 2, 0));
                case EaseType.InOutQuad:
                    return new AnimationCurve(new Keyframe(0, 0, 0, 2), new Keyframe(0.5f, 0.5f, 1, 1), new Keyframe(1, 1, 2, 0));
                case EaseType.InSine:
                    return new AnimationCurve(new Keyframe(0, 0, 0, 0), new Keyframe(1, 1, 1, 0));
                case EaseType.OutSine:
                    return new AnimationCurve(new Keyframe(0, 0, 0, 1), new Keyframe(1, 1, 0, 0));
                case EaseType.InOutSine:
                    return new AnimationCurve(new Keyframe(0, 0, 0, 0), new Keyframe(0.5f, 0.5f, 1, 1), new Keyframe(1, 1, 0, 0));
                // 他のEaseタイプの場合の処理もここに追加できます
                default:
                    return new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
            }
        }
    }
}
