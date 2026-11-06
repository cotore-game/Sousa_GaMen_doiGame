using UnityEngine;
using UnityEngine.UI;

namespace ADV.Presentation
{
    /// <summary>ロード済みの背景を表示する。</summary>
    public class BackgroundView : MonoBehaviour
    {
        [SerializeField]
        private Image bgImage;

        public void SetSprite(Sprite sprite)
        {
            if (bgImage == null)
            {
                Debug.LogWarning("[BackgroundView] Background Image reference is missing.");
                return;
            }
            bgImage.sprite = sprite;
            if (sprite != null && !bgImage.gameObject.activeSelf)
                bgImage.gameObject.SetActive(true);
        }
    }
}