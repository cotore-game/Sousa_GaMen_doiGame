using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using Cysharp.Threading.Tasks;

/// <summary>
/// タイトル画面管理。TMP + InputSystem(キーボードのみ) + UniTask。
///
/// [Hierarchy]
/// Canvas
///   TitleText  : （タイトル）  ※このスクリプトは触らない
///   HajimeText : Inspector で "はじめ" とセットしておく
///   RuText     : Inspector で "る"    とセットしておく
///   NaiText    : Inspector で "ない"  とセットしておく
///   HintText   : ヒント文（動的に書き換え）
///   Manager    : このスクリプトをアタッチ
/// </summary>
public class TitleSceneManager : MonoBehaviour
{
    // ────────── Inspector ──────────

    [Header("TMP")]
    [SerializeField] private TextMeshProUGUI hajimeText;
    [SerializeField] private TextMeshProUGUI ruText;
    [SerializeField] private TextMeshProUGUI naiText;
    [SerializeField] private TextMeshProUGUI hintText;

    private const string Cursor = ">";

    // ベーステキスト
    private string _hajimeBase;
    private string _ruBase;
    private string _naiBase;

    private enum State { Menu, Confirm }
    private State _state;

    /// <summary>Confirm 状態でのカーソル位置。0 = る、1 = ない</summary>
    private int _confirmIndex;

    private bool _isTransitioning;

    private void Start()
    {
        // TMP に最初からセットされているテキストをベースとして保存
        _hajimeBase = hajimeText.text;
        _ruBase = ruText.text;
        _naiBase = naiText.text;

        ruText.gameObject.SetActive(false);
        naiText.gameObject.SetActive(false);

        EnterMenu();
    }

    private void Update()
    {
        if (_isTransitioning) return;

        var kb = Keyboard.current;
        if (kb == null) return;

        bool up = kb.upArrowKey.wasPressedThisFrame;
        bool down = kb.downArrowKey.wasPressedThisFrame;
        bool enter = kb.enterKey.wasPressedThisFrame
                  || kb.numpadEnterKey.wasPressedThisFrame;

        switch (_state)
        {
            case State.Menu:
                if (enter) EnterConfirm();
                break;

            case State.Confirm:
                if (up) MoveConfirm(-1);
                if (down) MoveConfirm(+1);
                if (enter) DecideConfirm();
                break;
        }
    }

    // State transitions

    private void EnterMenu()
    {
        _state = State.Menu;

        ruText.gameObject.SetActive(false);
        naiText.gameObject.SetActive(false);

        if (hintText != null)
            hintText.text = "Enter で決定";

        // > はじめ
        hajimeText.text = Cursor + _hajimeBase;
    }

    private void EnterConfirm()
    {
        _state = State.Confirm;
        _confirmIndex = 0;   // "る" をデフォルト選択

        ruText.gameObject.SetActive(true);
        naiText.gameObject.SetActive(true);

        // はじめ からカーソルを外す
        hajimeText.text = _hajimeBase;

        if (hintText != null)
            hintText.text = "↑↓で選択 / Enterで決定";

        RefreshConfirm();
    }

    // Confirm 操作

    private void MoveConfirm(int dir)
    {
        _confirmIndex = (_confirmIndex + dir + 2) % 2;
        RefreshConfirm();
    }

    /// <summary>カーソル位置に応じて る / ない のテキストを更新する。</summary>
    private void RefreshConfirm()
    {
        ruText.text = (_confirmIndex == 0 ? Cursor : "") + _ruBase;
        naiText.text = (_confirmIndex == 1 ? Cursor : "") + _naiBase;
    }

    private void DecideConfirm()
    {
        if (_confirmIndex == 0)
        {
            GameFlowManager.Instance.GoToNextScene();
        }
        else
        {
            EnterMenu();
        }
    }
}
