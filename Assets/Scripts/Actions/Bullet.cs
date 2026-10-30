using UnityEngine;

namespace ActionPrototype
{
    /// <summary>
    /// 【担当】発射後の弾自身の移動、命中時のEnemyへのダメージ通知、自身の消滅。
    /// 【仕様】向いていた方向へ等速で水平移動。速度は目で追える程度に調整する。
    /// 敵と重なったらEnemy.TakeDamage(1)へ通知し、弾は消える。
    /// ダメージ量は1。敵のHPを直接書き換えず、HP管理はEnemyへ任せる。
    /// Gun／Playerの状態は知らなくてよい。Initializeの引数で発射方向だけ受け取る。
    /// </summary>
    /// <remarks>
    /// 【作成済み】Assets/Prefabs/Actions/Bullet.prefab。
    /// 画像: Assets/Sprites/Action/Projectiles/Bullet.png。別の銃口・銃本体スプライトは不要。
    /// Rigidbody2Dは重力0、回転固定、CircleCollider2DはIs Triggerを有効化済み。
    /// 重なりはOnTriggerEnter2Dで扱う。通常のOnCollisionEnter2Dでは通知されない。
    /// GunのTryFireからInstantiateしてInitialize(Vector2.right/left)を呼ぶ想定。
    /// Playerの当たり判定内で生成せず、設定済みのMuzzleRight/MuzzleLeftを発射位置に使う。
    /// 壁・画面外・寿命での消滅はGoogle仕様に明記がない。弾が残り続けないよう寿命を設けるのは推奨追加案。
    /// InspectorのlifeTime=5秒はその仮値。Awakeで消す処理はまだ入れていないので課題として実装する。
    /// 初期speed=6も調整用の仮値。歩行速度やアニメ再生速度とは独立した値。
    /// 仕様元: https://docs.google.com/document/d/1jVNNvVS2BbfkY3wJzQv73Hr0PVPPb0P7_iovSdvB80Y/edit?tab=t.6v0tp4lmqdo1
    /// </remarks>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
    public class Bullet : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float speed = 6f;
        [SerializeField, Min(1)] private int damage = 1;
        [SerializeField, Min(0.1f)] private float lifeTime = 5f;

        public float Speed => speed;
        public int Damage => damage;
        public float LifeTime => lifeTime;

        private void Awake()
        {
            // TODO (課題4): Rigidbody2Dを取得する。寿命で消すならDestroy(gameObject, lifeTime)を調べる。
        }

        public void Initialize(Vector2 direction)
        {
            // TODO (課題4): 方向を保持し、Y速度を0にした等速の水平移動を準備する。
            // Gunそのものを保存する必要はない。右／左の値だけ渡してもらう。
        }

        private void FixedUpdate()
        {
            // TODO (課題4): Rigidbody2D.linearVelocityなどで等速移動させる。
            // 速度を代入する方式と毎フレームAddForceする方式の違いを考える。
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // TODO (課題4): EnemyならTakeDamage(damage)を1回だけ呼び、この弾をDestroyする。
            // 例: other.TryGetComponent<Enemy>(out var enemy)。GunやPlayerへ逆参照しない。
            // Destroyは即時ではないので、消えるまでに複数のColliderへ命中しても二重加算しない工夫を考える。
            // 敵以外（床やPlayer）に触れたときの扱いは別に決める。
        }
    }
}
