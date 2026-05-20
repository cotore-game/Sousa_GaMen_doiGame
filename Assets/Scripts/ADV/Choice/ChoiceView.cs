using UnityEngine;
using TMPro;

namespace ADV.Presentation
{
    /// <summary>
    /// 選択肢表示のView層
    /// </summary>
    public class ChoiceView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TMP_Text errorText;        // null 可

        // ─── 公開プロパティ ───────────────────

        /// <summary>Presenter が onSubmit を購読するために公開</summary>
        public TMP_InputField InputField => inputField;

        // ─── 操作メソッド ─────────────────────

        public void SetActive(bool active)
        {
            panel.SetActive(active);
        }

        public void SetBodyText(string text)
        {
            bodyText.text = text;
        }

        /// <summary>InputField をクリアしてフォーカスを当てる</summary>
        public void ActivateInput()
        {
            inputField.text = "";
            inputField.ActivateInputField();
        }

        public void ShowError(string message)
        {
            if (errorText == null) return;
            errorText.text = message;
            errorText.gameObject.SetActive(true);
        }

        public void ClearError()
        {
            if (errorText == null) return;
            errorText.gameObject.SetActive(false);
            errorText.text = "";
        }
    }
}
