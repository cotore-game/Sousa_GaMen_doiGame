using UnityEngine;
using UnityEngine.InputSystem;

namespace ActionPrototype
{
    /// <summary>
    /// キーの入力を読み取るための、準備済みのクラスです。
    /// Player.csからinput.Horizontalやinput.JumpPressedを使ってください。
    ///
    /// キーの割り当てはAssets/Input/ActionControls.inputactionsにあります。
    /// Playerの「Use Debug Controls」のチェックで、テスト用と本番用を切り替えられます。
    /// まずはチェックを入れたままで、移動などの処理を作りましょう。
    ///
    /// 本番用はW+R/W+Lで移動、H+R/H+Lで向き変更、Jを離すとジャンプです。
    /// 本番の銃はE/D/下矢印/T/A/N/K/U。テスト用のキーはGun.csに書いてあります。
    /// </summary>
    [DisallowMultipleComponent]
    public class ActionInput : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("キーの割り当てが入ったファイルです。ActionControls.inputactionsを設定済みです。")]
        private InputActionAsset inputActions;

        [SerializeField]
        [Tooltip("オンならA/D・左右矢印で移動、Space・W・上矢印でジャンプ。本番操作の確認時はオフにします。")]
        private bool useDebugControls = true;

        // このプレイヤーだけで使う入力設定のコピーです。
        private InputActionAsset runtimeActions;

        // 今使っている、テスト用か本番用のキーの組み合わせです。
        private InputActionMap controls;

        // Horizontalは左=-1、止まる=0、右=1です。
        public float Horizontal => Held("MoveRight") - Held("MoveLeft");

        // Turnは向き変更の入力です。左=-1、何もしない=0、右=1です。
        public float Turn => Held("TurnRight") - Held("TurnLeft");

        // trueになるのはジャンプ入力があった瞬間だけです。
        // テスト用はキーを押した瞬間、本番用はJを離した瞬間にtrueになります。
        public bool JumpPressed => Performed("Jump");

        // 銃の操作です。押した瞬間だけtrueになります。PlayerからGunへ渡しています。
        public bool PowderPressed => Performed("Powder");
        public bool InsertBulletPressed => Performed("InsertBullet");
        public bool RamPressed => Performed("Ram");
        public bool ClosePanPressed => Performed("ClosePan");
        public bool RaiseHammerPressed => Performed("RaiseHammer");
        public bool InsertMatchPressed => Performed("InsertMatch");
        public bool AimPressed => Performed("Aim");
        public bool FirePressed => Performed("Fire");

        private void Awake()
        {
            if (inputActions == null)
            {
                Debug.LogError("ActionInputにActionControls.inputactionsを設定してください。", this);
                enabled = false;
                return;
            }

            runtimeActions = Instantiate(inputActions);
        }

        private void OnEnable()
        {
            // OnEnableは、この部品が使える状態になったときに呼ばれます。
            if (runtimeActions == null) return;
            controls = runtimeActions.FindActionMap(useDebugControls ? "Debug" : "Game", true);
            controls.Enable();
        }

        private void OnDisable()
        {
            // OnDisableは、この部品が使えない状態になったときに呼ばれます。
            if (controls != null) controls.Disable();
        }

        private void OnDestroy()
        {
            if (runtimeActions != null) Destroy(runtimeActions);
        }

        // 押している間は1、離している間は0を返します。
        private float Held(string actionName)
        {
            return isActiveAndEnabled && controls != null && controls[actionName].IsPressed() ? 1f : 0f;
        }

        // 入力があった瞬間かどうかを返します。
        private bool Performed(string actionName)
        {
            return isActiveAndEnabled && controls != null && controls[actionName].WasPerformedThisFrame();
        }
    }
}
