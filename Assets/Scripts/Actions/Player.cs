using UnityEngine;

namespace ActionPrototype
{
    /// <summary>
    /// 【担当】入力、移動、向き、ジャンプ、プレイヤー自身のHP。装填・発射はGunへ指示する。
    /// 【仕様】横スクロール2Dアクション。開始HPは4、被弾ごとに1減り、0でゲームオーバー。
    /// UI表示と画面遷移の実装は今回の課題外。外からはCurrentHealthで残りHPを読める。
    /// 右向きでW+Rを押している間だけ右へ、左向きでW+Lの間だけ左へ歩く。
    /// 歩行入力だけで向きは変わらない。H+Lで左向き、H+Rで右向きになる。
    /// 接地中にJを押して離した瞬間にジャンプ。長押し時間で高さは変えず、空中での再ジャンプは禁止。
    /// 空中でも歩行入力による横移動は可能。重力はRigidbody2Dに任せる。
    /// 歩行・方向転換・ジャンプをしたら、Gun.CancelAction()へ装填／構えの中断を通知する。
    /// 銃の入力はE,D,下矢印,T,A,N,K,U。工程と秒数はGun.cs冒頭を参照。
    /// 3秒無操作で思い出しの吹き出しを表示し、4秒で透明度100→0、操作時に瞬時に100という仕様もある。
    /// 吹き出しを含むUI実装は後の課題として、このクラスへ表示処理を詰め込まない。
    /// </summary>
    /// <remarks>
    /// 【素材】すべてAssets/Sprites/Action/TeruoNakata/以下。
    /// 右待機=IdleRight.png、左待機=IdleLeft.png。
    /// 右歩行=WalkRight01.png(右足前)とWalkRight02.png(左足前)。
    /// 左歩行=WalkLeft01.png(左足前)とWalkLeft02.png(右足前)。
    /// 銃あり右歩行=GunWalkRight01.pngとGunWalkRight02.png、右待機はGunWalkRight02.png。
    /// 銃あり左歩行=GunWalkLeft01.pngとGunWalkLeft02.png、左待機はGunWalkLeft02.png。
    /// 仕様書の銃あり左歩行に右向き画像の記載が混ざっているため、ここでは左向き2枚を使用する。
    /// 【作成済み】Assets/Prefabs/Actions/Player.prefabの子VisualにAnimatorがある。
    /// ControllerとClipはAssets/Animations/Actions/Player/以下。
    /// Moving(bool), FacingLeft(bool), HasGun(bool)で8状態を切り替える。歩行は60fps換算で20fごとに画像変更。
    /// SetLocomotionAnimation(true, false)で右歩き、(true, true)で左歩き、moving=falseでその向きの待機。
    /// この関数は見た目だけを変える。実際の移動はFixedUpdateで別に実装する。
    /// SetWalkAnimationSpeed(2f)で歩行アニメだけ2倍、0.5fで半分。移動速度moveSpeedとは別の値。
    /// Animatorを直接操作するならGetComponentInChildren&lt;Animator&gt;()で取得し、
    /// SetBool("Moving", true)、SetBool("FacingLeft", false)、SetBool("HasGun", true)、SetFloat("WalkSpeed", 1f)。
    /// Animator.speedは全状態に影響する。装填1.0秒の計測はアニメ速度ではなくTime.deltaTimeなどで行う。
    /// 【依存】Player→Gun→Bullet→Enemy、およびEnemy→Player。Gun/BulletからPlayerへ逆参照しない。
    /// 仕様元: https://docs.google.com/document/d/1jVNNvVS2BbfkY3wJzQv73Hr0PVPPb0P7_iovSdvB80Y/edit?tab=t.6v0tp4lmqdo1
    /// </remarks>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
    public class Player : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float moveSpeed = 4f;
        [SerializeField, Min(0f)] private float jumpImpulse = 8f;
        [SerializeField, Min(1)] private int maxHealth = 4;
        [SerializeField, Min(0.01f)] private float walkAnimationSpeed = 1f;

        private Animator animator;
        private Gun gun;

        public int CurrentHealth { get; private set; }
        public float MoveSpeed => moveSpeed;
        public float JumpImpulse => jumpImpulse;

        private void Awake()
        {
            CurrentHealth = maxHealth;
            animator = GetComponentInChildren<Animator>();
            gun = GetComponent<Gun>();
            SetWalkAnimationSpeed(walkAnimationSpeed);
            SetLocomotionAnimation(false, false);
            // TODO (課題1): 物理更新で使うRigidbody2Dと接地用Collider2Dを取得しておく。
        }

        private void Update()
        {
            // TODO (課題1): 向きを保持し、W+R / W+Lの歩行、H+R / H+Lの方向転換を読む。
            // Input Systemは導入済み。例: UnityEngine.InputSystem.Keyboard.current。
            // rKey.isPressedは押している間、wasPressedThisFrameは押した瞬間、
            // jKey.wasReleasedThisFrameは離した瞬間。Keyboard.currentがnullの環境にも備える。
            // TODO (課題1): 接地中にJを離した瞬間だけ、次のFixedUpdate用にジャンプを予約する。
            // TODO (課題1): 決めた歩行状態・向きをSetLocomotionAnimationへ渡す。
            // TODO (課題4): E/D/下矢印/T/A/NをGun.HandleReloadInputへ、KをTryAimへ、UをTryFireへ渡す。
            // Eは工程1と4、Aは工程6と8で同じキー。判定するのはGun側の責務。
            // TODO (課題4): 歩行・方向転換・ジャンプの成立時にgun.CancelAction()を呼ぶ。
            // 同時入力時の優先順位を決め、gunが未設定なら銃関連の呼び出しをしない。
        }

        private void FixedUpdate()
        {
            // TODO (課題1): Floorレイヤーで接地判定を更新し、空中での再ジャンプを防ぐ。
            // TODO (課題1): Rigidbody2D.linearVelocityで横移動、予約済みジャンプを処理する。
            // Xを変更するとき、落下中のY速度を上書きしない。transformの直接移動と混在させない。
            // 入力はUpdate、物理はFixedUpdate。予約を消費し、1回のキー操作で何度も跳ばないようにする。
        }

        public void TakeDamage(int amount)
        {
            // TODO (課題3): 0以下のamountは無視し、CurrentHealthを0未満にせず減らす。
            // HPを変更するのはこの窓口だけにする。EnemyがPlayerのHPを直接書き換えない。
            // HP0時のUI／遷移との接続は後で行う。無敵時間は仕様未指定なので追加が必要なら相談する。
        }

        /// <summary>準備済みの待機・歩行アニメを切り替える補助。向きの判定・移動処理は呼び出し側で行う。</summary>
        public void SetLocomotionAnimation(bool moving, bool facingLeft)
        {
            if (animator == null) return;
            animator.SetBool("Moving", moving);
            animator.SetBool("FacingLeft", facingLeft);
            animator.SetBool("HasGun", gun != null && gun.IsEquipped);
        }

        /// <summary>歩行Clipの再生倍率だけを変更する。物理の歩行速度・装填時間は変えない。</summary>
        public void SetWalkAnimationSpeed(float multiplier)
        {
            walkAnimationSpeed = Mathf.Max(0.01f, multiplier);
            if (animator != null) animator.SetFloat("WalkSpeed", walkAnimationSpeed);
        }
    }
}
