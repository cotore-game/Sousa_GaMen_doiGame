using UnityEngine;

namespace ActionPrototype
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
    public class Enemy : MonoBehaviour
    {
        // Walker.prefabとHopper.prefabには見た目と物理コンポーネントを設定済みです。
        // まず丸型の敵が一定方向へ歩く処理を考えて実装してください。
        // 次に、跳ねる敵の処理を追加してください。敵ごとのクラスに分けても構いません。
        // 速度・方向・接地の状態をどう持つか、必要な変数や関数は自分で考えてください。
        // WalkerのVisual: AnimatorのMoving(bool)で歩行アニメーション。
        // HopperのVisual: AnimatorのHop(trigger)で跳躍アニメーション。
        // HP・攻撃・ダメージ処理は、移動課題の次の段階で扱います。
    }
}
