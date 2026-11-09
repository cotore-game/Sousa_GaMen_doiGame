using UnityEngine;

namespace ActionPrototype
{
    /// <summary>
    /// 発射された弾を動かすクラスです。
    /// 右か左へ同じ速さで進み、敵に当たったら1ダメージを与えて消えます。
    ///
    /// 弾が当たらなかったときの動きは仕様に書かれていないので、lifeTimeを用意しました。
    /// この秒数が経ったら、Destroyで弾を削除しましょう。
    ///
    /// 弾のPrefab: Assets/Prefabs/Actions/Bullet.prefab
    /// 弾の画像: Assets/Sprites/Action/Projectiles/Bullet.png
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
    public class Bullet : MonoBehaviour
    {
        [SerializeField, Min(0f)]
        [Tooltip("弾が進む速さです。目で追えるくらいの速さに調整しましょう。")]
        private float speed = 6f;

        [SerializeField, Min(1)]
        [Tooltip("敵に当たったときに与えるダメージです。今は1です。")]
        private int damage = 1;

        [SerializeField, Min(0.1f)]
        [Tooltip("弾が消えるまでの秒数です。5なら、作られてから5秒後に消します。")]
        private float lifeTime = 5f;

        // 設定した速さ・ダメージ・寿命を、ほかのクラスから読むためのものです。
        public float Speed => speed;
        public int Damage => damage;
        public float LifeTime => lifeTime;

        private void Awake()
        {
            // Awakeには、弾が作られたときの準備を書きます。
            // TODO: Rigidbody2Dを取得しましょう。
            // TODO: Destroy(gameObject, lifeTime)で、lifeTime秒後に消す予約をしましょう。
        }

        public void Initialize(Vector2 direction)
        {
            // directionには進む向きが入ります。Vector2.rightは右、Vector2.leftは左です。
            // TODO: この向きを覚えて、弾を動かすときに使いましょう。
        }

        private void FixedUpdate()
        {
            // FixedUpdateには、Rigidbody2Dで動かすなど、物理演算の処理を書きます。
            // TODO: 覚えた向きとspeedを使って、横へ同じ速さで進ませましょう。
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // 弾の当たり判定が何かに重なったときに呼ばれます。otherが重なった相手です。
            // TODO: 相手がEnemyなら、TakeDamage(damage)でダメージを与えましょう。
            // TODO: 当たったらDestroy(gameObject)で弾を消しましょう。
        }
    }
}
