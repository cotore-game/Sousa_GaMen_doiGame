using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace ADV.Presentation
{
    /// <summary>
    /// テキスト表示のView
    /// </summary>
    public class TextDisplayView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject speechBubble;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private GameObject skipIcon;

        public void SetActive(bool active)
        {
            speechBubble.SetActive(active);
        }

        public void SetCharacterName(string name)
        {
            nameText.text = name;
            nameText.ForceMeshUpdate();
        }

        public void SetBodyText(string text)
        {
            bodyText.text = text;
            bodyText.ForceMeshUpdate();
        }

        public void SetVisibleCharacters(int count)
        {
            bodyText.maxVisibleCharacters = count;
        }

        public void SetSkipIconActive(bool active)
        {
            skipIcon.SetActive(active);
        }

        public int GetTotalCharacterCount()
        {
            bodyText.ForceMeshUpdate();
            return bodyText.GetParsedText().Length;
        }

        /// <summary>
        /// UIの上にポインターがある場合の入力無視判定
        /// </summary>
        public bool ShouldIgnoreInput()
        {
            if (EventSystem.current == null) return false;
            if (!EventSystem.current.IsPointerOverGameObject()) return false;

            var pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (var result in results)
            {
                if (result.gameObject.CompareTag("ExclusionUI"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
