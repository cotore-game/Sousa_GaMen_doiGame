using UnityEngine;

namespace ActionPrototype
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
    public class Player : MonoBehaviour
    {
        // まずこのクラスに、プレイヤーの移動とジャンプを実装してください。
        // Player.prefabにはRigidbody2D・CapsuleCollider2Dと、見た目用のVisualを設定済みです。
        // 入力、接地判定、物理更新をどう分けるか考え、必要な変数や関数を自分で定義してください。
        // 操作キーとジャンプの発火条件は、課題を受け取ったときの仕様に合わせてください。
        // VisualのAnimatorはMoving(bool)とFacingLeft(bool)で待機・歩行・向きを切り替えられます。
        // 完了の目安: 左右に動く・離すと止まる・床に立つ・接地中だけジャンプできる。
    }
}
