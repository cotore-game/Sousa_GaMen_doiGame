using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using ADV.Core;

namespace ADV.Presentation
{
    /// <summary>
    /// 個別キャラクターのView
    /// </summary>
    public class CharacterView : MonoBehaviour
    {
        [SerializeField] private Image characterImage;
        [SerializeField] private CanvasGroup canvasGroup;

        public void SetSprite(Sprite sprite)
        {
            Debug.Log(sprite);
            characterImage.sprite = sprite;
        }

        public void SetAlpha(float alpha)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = alpha;
            }
        }

        public async UniTask FadeIn(float duration, CancellableTask cancellable)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                if (cancellable?.IsCancellationRequested ?? false) break;

                SetAlpha(elapsed / duration);
                elapsed += Time.deltaTime;
                await UniTask.Yield();
            }
            SetAlpha(1f);
        }

        public async UniTask FadeOut(float duration, CancellableTask cancellable)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                if (cancellable?.IsCancellationRequested ?? false) break;

                SetAlpha(1f - elapsed / duration);
                elapsed += Time.deltaTime;
                await UniTask.Yield();
            }
            SetAlpha(0f);
        }

        public async UniTask MoveTo(Vector3 targetPosition, float duration, CancellableTask cancellable)
        {
            Vector3 startPos = transform.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                if (cancellable?.IsCancellationRequested ?? false) break;

                float t = elapsed / duration;
                t = t * t * (3f - 2f * t); // Smoothstep
                transform.localPosition = Vector3.Lerp(startPos, targetPosition, t);

                elapsed += Time.deltaTime;
                await UniTask.Yield();
            }

            transform.localPosition = targetPosition;
        }
    }
}
