using UnityEngine;

namespace ActionPrototype
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public sealed class Player : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float jumpImpulse = 8f;
        [SerializeField] private int maxHealth = 4;

        public int CurrentHealth { get; private set; }

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        private void Update()
        {
            // TODO (課題1): W+R / W+L の歩行、H+R / H+L の方向転換を入力として読む。
            // TODO (課題1): 接地中にJを離した瞬間だけジャンプを予約する。
        }

        private void FixedUpdate()
        {
            // TODO (課題1): Rigidbody2Dで横移動とジャンプを処理する。
            // TODO (課題1): 接地判定を更新し、空中での再ジャンプを防ぐ。
        }

        public void TakeDamage(int amount)
        {
            // TODO (課題3): HPを0未満にせず減らす。敵との接触判定時に使う。
        }
    }
}
