using UnityEngine;

namespace ActionPrototype
{
    /// <summary>
    /// 【担当】敵自身の移動・HPと、接触したPlayerへのダメージ通知。
    /// ステージ1の丸型(Walker)は左へ歩き続ける。悪魔型(Hopper)は左へ定期的に跳ねる。
    /// Hopperは難しければWalkerとは違う移動でもよい、というステージ仕様。
    /// プレイヤーが踏んでも敵は倒れず、プレイヤーが被弾する。接触ダメージは1。
    /// 銃弾が命中したら敵は1ダメージ。敵自身のHPを減らす処理はTakeDamageが受け持つ。
    /// 敵の初期HP、速度、ジャンプ間隔は数値指定がないためInspectorで調整する。
    /// </summary>
    /// <remarks>
    /// 【配置済み】Assets/Prefabs/Actions/Walker.prefab、Hopper.prefab。
    /// 画像: Assets/Sprites/Action/Walker/Walk01.png・Walk02.png。
    /// Hopper画像: Assets/Sprites/Action/Hopper/Standing.png・Crouching.png・Airborne.png。
    /// クリップとController: Assets/Animations/Actions/Walker/、Hopper/。
    /// 見た目の補足はAssets/ArtReferences/WalkerAnimation.jpg、HopperAnimation.jpg。
    /// WalkerはSetWalkingAnimation(true)で歩行、falseで待機。直接ならAnimator.SetBool("Moving", true)。
    /// 歩行再生倍率は子VisualのAnimator.SetFloat("WalkSpeed", 2f)で2倍。実際の移動速度とは別。
    /// HopperはPlayHopAnimation()、または子VisualのAnimator.SetTrigger("Hop")で跳躍Clipを再生する。
    /// Hopは見た目だけなので、接地・ジャンプ速度・次に跳ねる時刻はFixedUpdate等で別に管理する。
    /// 【依存】Enemy→Player.TakeDamage、Bullet→Enemy.TakeDamage。EnemyはBulletやGunを参照しない。
    /// 共通クラスで始め、動きの違いが複雑になったら別クラスへ分けてもよい。
    /// 仕様元: アクション仕様と同資料の「レベルデザイン／ステージ1」。UIとボス攻撃は今回の範囲外。
    /// https://docs.google.com/document/d/1jVNNvVS2BbfkY3wJzQv73Hr0PVPPb0P7_iovSdvB80Y/edit?tab=t.h5xvg7s5bct
    /// </remarks>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
    public class Enemy : MonoBehaviour
    {
        public enum MovementKind { Walker, Hopper }

        [SerializeField] private MovementKind movementKind;
        [SerializeField, Min(0f)] private float moveSpeed = 2f;
        [SerializeField, Min(1)] private int maxHealth = 1;
        [SerializeField, Min(1)] private int contactDamage = 1;

        private Animator animator;

        public int CurrentHealth { get; private set; }
        public MovementKind Kind => movementKind;
        public float MoveSpeed => moveSpeed;
        public int ContactDamage => contactDamage;

        private void Awake()
        {
            CurrentHealth = maxHealth;
            animator = GetComponentInChildren<Animator>();
            // TODO (課題3): Rigidbody2Dを取得する。Hopperの間隔・ジャンプ力・接地状態なども考える。
        }

        private void FixedUpdate()
        {
            // TODO (課題3): movementKindがWalkerなら左へ歩かせる。Y速度を不用意に消さない。
            // TODO (課題3): Hopperなら接地と間隔を確認して左へジャンプさせ、PlayHopAnimation()も呼ぶ。
            // アニメーション再生だけではGameObjectは移動しない。
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // TODO (課題3): 接触相手にPlayerがあれば、そのTakeDamage(contactDamage)へ通知する。
            // 例: collision.gameObject.TryGetComponent<Player>(out var player)。
            // 踏みつけでも敵を倒さない。OnCollisionStay2Dで毎フレームHPを減らさないよう注意する。
        }

        public void TakeDamage(int amount)
        {
            // TODO (課題4): 0以下のamountを無視し、自身のHPを0未満にせず減らす。HP0の敵を消す。
            // Bulletはこの関数を呼ぶだけ。Enemy側でBulletを探したり参照したりする必要はない。
        }

        /// <summary>Walkerの見た目を切り替える。移動を実装した箇所から呼ぶ。</summary>
        public void SetWalkingAnimation(bool moving)
        {
            if (animator != null && movementKind == MovementKind.Walker)
                animator.SetBool("Moving", moving);
        }

        /// <summary>Hopperが実際に跳ぶタイミングで呼ぶ。Clipは再生後に待機へ戻る。</summary>
        public void PlayHopAnimation()
        {
            if (animator != null && movementKind == MovementKind.Hopper)
                animator.SetTrigger("Hop");
        }
    }
}
