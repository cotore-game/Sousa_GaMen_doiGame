using UnityEngine;

namespace ActionPrototype
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public sealed class Enemy : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private int contactDamage = 1;

        private void FixedUpdate()
        {
            // TODO (課題3): まずステージ1の丸型敵を左へ歩かせる。
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // TODO (課題3): Playerに接触したら1回だけダメージを与える。
            // 踏みつけでも敵を倒さない（ステージ1仕様）。
        }
    }
}
