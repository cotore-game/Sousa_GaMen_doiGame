using UnityEngine;

namespace ActionPrototype
{
    /// <summary>
    /// プレイヤーの移動、向き、ジャンプ、HPを管理するクラスです。
    /// 最初のHPは4。敵に当たると1減り、0になるとゲームオーバーです。
    ///
    /// 今はテスト用に、A/D・←/→で左右、Space・W・↑でジャンプする入力を用意しています。
    /// 本番では同時押しが必要な面倒な操作に変えます。今は気にせず移動処理を作りましょう。
    /// 空中でも横に移動できますが、ジャンプは地面にいるときだけです。
    ///
    /// アニメーションは作成済みです。再生には下のSetLocomotionAnimationを使ってください。
    /// 元画像: Assets/Sprites/Action/TeruoNakata/
    /// アニメーション: Assets/Animations/Actions/Player/
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D), typeof(ActionInput))]
    public class Player : MonoBehaviour
    {
        [SerializeField, Min(0f)]
        [Tooltip("左右に歩く速さです。大きくすると速くなります。")]
        private float moveSpeed = 4f;

        [SerializeField, Min(0f)]
        [Tooltip("ジャンプするときの上向きの強さです。大きくすると高く跳びます。")]
        private float jumpImpulse = 8f;

        [SerializeField, Min(1)]
        [Tooltip("ゲーム開始時のHPです。今は4にしてください。")]
        private int maxHealth = 4;

        [SerializeField, Min(0.01f)]
        [Tooltip("歩くアニメーションの速さです。1で通常、2で2倍、0.5で半分です。")]
        private float walkAnimationSpeed = 1f;

        // アニメーションを再生するための部品です。
        private Animator animator;

        // 銃の処理をするクラスです。
        private Gun gun;

        // キーの入力を読み取る準備済みの部品です。
        private ActionInput input;

        // trueなら左向き、falseなら右向きです。
        private bool facingLeft = false;

        // 今のHPです。ほかのクラスからは読むだけで、変更はPlayer内で行います。
        public int CurrentHealth { get; private set; }

        // Inspectorで設定した値を、ほかのクラスから読むためのものです。
        public float MoveSpeed => moveSpeed;
        public float JumpImpulse => jumpImpulse;

        private void Awake()
        {
            // Awakeは、このオブジェクトが作られたときの準備を書く場所です。
            CurrentHealth = maxHealth;
            animator = GetComponentInChildren<Animator>();
            gun = GetComponent<Gun>();
            input = GetComponent<ActionInput>();
            SetWalkAnimationSpeed(walkAnimationSpeed);
            SetLocomotionAnimation(false, facingLeft);

            // TODO: Rigidbody2DとCollider2Dも取得しておきましょう。
        }

        private void Update()
        {
            // Updateには、入力を読む処理など、物理演算以外の処理を書きます。
            // input.Horizontalは左=-1、止まる=0、右=1です。
            // input.Turnも左=-1、右=1。これを使ってfacingLeftを変えましょう。
            // input.JumpPressedは、ジャンプの入力があったときだけtrueになります。
            // 本番操作では、今向いている方向への移動だけ受け付けます。

            // TODO: 左右の入力を覚え、地面にいるときだけジャンプを予約しましょう。
            // TODO: 向きを決めて、SetLocomotionAnimationへ歩いているかと向きを渡しましょう。
            // TODO: 移動・方向転換・ジャンプをしたらgun.CancelAction()を呼びましょう。

            ReadGunInput();
        }

        private void FixedUpdate()
        {
            // FixedUpdateには、Rigidbody2Dで動かすなど、物理演算の処理を書きます。
            // TODO: 地面に立っているか調べましょう。地面のレイヤー名はFloorです。
            // TODO: Updateで覚えた入力とmoveSpeedを使って左右に動かしましょう。
            // TODO: ジャンプが予約されていたら、jumpImpulseを使って跳ばしましょう。
        }

        public void TakeDamage(int amount)
        {
            // amountは受けるダメージの数です。TakeDamage(1)ならHPを1減らします。
            // TODO: CurrentHealthを減らしましょう。0より小さくならないようにします。
        }

        /// <summary>
        /// 歩く見た目を変える関数です。キャラクターの位置は変わりません。
        /// movingは歩くならtrue、止まるならfalse。
        /// facingLeftは左向きならtrue、右向きならfalseです。
        /// </summary>
        public void SetLocomotionAnimation(bool moving, bool facingLeft)
        {
            // 例: SetLocomotionAnimation(true, false); で右向きの歩くアニメーション。
            //     SetLocomotionAnimation(false, true); で左向きの待機アニメーション。
            if (animator == null) return;
            animator.SetBool("Moving", moving);
            animator.SetBool("FacingLeft", facingLeft);
            animator.SetBool("HasGun", gun != null && gun.IsEquipped);
        }

        /// <summary>歩くアニメーションの速さを変えます。2fで2倍、0.5fで半分です。</summary>
        public void SetWalkAnimationSpeed(float multiplier)
        {
            walkAnimationSpeed = Mathf.Max(0.01f, multiplier);
            if (animator != null) animator.SetFloat("WalkSpeed", walkAnimationSpeed);
        }

        // 銃の入力をGunへ渡す部分は準備済みです。装填や発射の中身はGun.csに書きましょう。
        private void ReadGunInput()
        {
            if (input == null || gun == null) return;

            if (input.PowderPressed) gun.HandleReloadInput(Gun.ReloadCommand.Powder);
            if (input.InsertBulletPressed) gun.HandleReloadInput(Gun.ReloadCommand.InsertBullet);
            if (input.RamPressed) gun.HandleReloadInput(Gun.ReloadCommand.Ram);
            if (input.ClosePanPressed) gun.HandleReloadInput(Gun.ReloadCommand.ClosePan);
            if (input.RaiseHammerPressed) gun.HandleReloadInput(Gun.ReloadCommand.RaiseHammer);
            if (input.InsertMatchPressed) gun.HandleReloadInput(Gun.ReloadCommand.InsertMatch);
            if (input.AimPressed) gun.TryAim();
            if (input.FirePressed) gun.TryFire(facingLeft);
        }
    }
}
