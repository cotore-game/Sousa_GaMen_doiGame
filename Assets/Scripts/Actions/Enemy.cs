using UnityEngine;

namespace ActionPrototype
{
    /// <summary>
    /// 敵の移動とHPを管理するクラスです。
    /// 丸い敵は左へ歩き、悪魔の敵は左へ跳ねて進みます。
    /// プレイヤーに当たると1ダメージを与えます。踏まれても敵は倒れません。
    /// 弾が当たると敵のHPが1減り、0になると消えます。
    ///
    /// 敵のPrefab: Assets/Prefabs/Actions/Walker.prefab、Hopper.prefab
    /// 画像: Assets/Sprites/Action/Walker/、Hopper/
    /// アニメーション: Assets/Animations/Actions/Walker/、Hopper/
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
    public class Enemy : MonoBehaviour
    {
        // enumは、用意した選択肢から1つを選ぶための型です。
        // ここでは「歩く敵」か「跳ねる敵」かを選べます。
        // 例: movementKind == MovementKind.Walker なら、歩く敵という意味です。
        public enum MovementKind
        {
            Walker, // 歩く敵
            Hopper  // 跳ねる敵
        }

        [SerializeField]
        [Tooltip("敵の動き方です。Walkerは歩く敵、Hopperは跳ねる敵です。")]
        private MovementKind movementKind;

        [SerializeField, Min(0f)]
        [Tooltip("敵が横に進む速さです。大きくすると速くなります。")]
        private float moveSpeed = 2f;

        [SerializeField, Min(1)]
        [Tooltip("敵の最初のHPです。1なら弾が1発当たると倒れます。")]
        private int maxHealth = 1;

        [SerializeField, Min(1)]
        [Tooltip("プレイヤーにぶつかったときに与えるダメージです。今は1です。")]
        private int contactDamage = 1;

        // アニメーションを再生するための部品です。
        private Animator animator;

        // 今のHPです。変更する処理は、このクラスのTakeDamageに書きましょう。
        public int CurrentHealth { get; private set; }

        // Inspectorで設定した値を、ほかのクラスから読むためのものです。
        public MovementKind Kind => movementKind;
        public float MoveSpeed => moveSpeed;
        public int ContactDamage => contactDamage;

        private void Awake()
        {
            // Awakeには、敵が作られたときの準備を書きます。
            CurrentHealth = maxHealth;
            animator = GetComponentInChildren<Animator>();
            // TODO: Rigidbody2Dも取得しておきましょう。
        }

        private void FixedUpdate()
        {
            // FixedUpdateには、Rigidbody2Dで動かすなど、物理演算の処理を書きます。
            // TODO: movementKindがWalkerなら、moveSpeedの速さで左へ歩かせましょう。
            // TODO: Hopperなら、一定時間ごとに左へジャンプさせましょう。
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // 何かにぶつかったときに呼ばれます。collisionにはぶつかった相手の情報が入ります。
            // TODO: 相手がPlayerなら、そのPlayerのTakeDamage(contactDamage)を呼びましょう。
        }

        public void TakeDamage(int amount)
        {
            // amountは受けるダメージの数です。
            // TODO: CurrentHealthを減らし、0になったらDestroyでこの敵を消しましょう。
        }

        /// <summary>歩く敵のアニメーションです。trueで歩き、falseで待機します。</summary>
        public void SetWalkingAnimation(bool moving)
        {
            // 歩く処理を作ったらSetWalkingAnimation(true)を呼んでください。
            // 再生速度を変える例: animator.SetFloat("WalkSpeed", 2f); で2倍です。
            if (animator != null && movementKind == MovementKind.Walker)
                animator.SetBool("Moving", moving);
        }

        /// <summary>跳ねる敵のアニメーションです。ジャンプするタイミングで呼んでください。</summary>
        public void PlayHopAnimation()
        {
            if (animator != null && movementKind == MovementKind.Hopper)
                animator.SetTrigger("Hop");
        }
    }
}
