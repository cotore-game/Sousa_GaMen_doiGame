using UnityEngine;
using UnityEngine.UI;

namespace ADV.Presentation
{
    /// <summary>
    /// 背景表示のView層
    /// インスペクターから表示先のImageと各背景Spriteを直接参照して管理する
    /// </summary>
    public class BackgroundView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image bgImage;

        [Header("Background Sprites")]
        [SerializeField] private Sprite slothHomeSprite;   // ナマケモノの家
        [SerializeField] private Sprite greengrocerSprite; // 八百屋
        [SerializeField] private Sprite forestSprite;      // 森の背景
        
        public void ChangeBackground(string bgName)
        {
            if (bgImage == null)
            {
                Debug.LogWarning("[BackgroundView] Background Image reference is missing.");
                return;
            }

            switch (bgName)
            {
                case "ナマケモノの家":
                    bgImage.sprite = slothHomeSprite;
                    break;
                case "八百屋":
                    bgImage.sprite = greengrocerSprite;
                    break;
                case "森の背景":
                    bgImage.sprite = forestSprite;
                    break;
                default:
                    Debug.LogWarning($"[BackgroundView] Unknown background name: {bgName}");
                    break;
            }
            
            // スプライトが設定されたら表示（必要に応じて）
            if (bgImage.sprite != null && !bgImage.gameObject.activeSelf)
            {
                bgImage.gameObject.SetActive(true);
            }
        }
    }
}
