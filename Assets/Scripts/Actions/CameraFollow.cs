using UnityEngine;

namespace ActionPrototype
{
    /// <summary>
    /// プレイヤーに合わせてカメラを左右に動かすクラスです。
    /// Cameraという名前はUnityでも使われていて、同じ名前だと間違えて使って
    /// エラーになることがあるので、ここではCameraFollowにしています。
    ///
    /// Assets/Scenes/Dev/ActionDev.unityのMain Cameraに付けてあります。
    /// targetには、追いかけるPlayerを設定済みです。
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("カメラが追いかける相手です。シーンにあるPlayerを入れます。")]
        private Transform target;

        // カメラが追いかける相手を、ほかのクラスから読むためのものです。
        public Transform Target => target;

        private void Awake()
        {
            // Awakeには、このオブジェクトが作られたときの準備を書きます。
            // TODO: カメラの最初の位置を覚えておきましょう。
        }

        private void LateUpdate()
        {
            // LateUpdateは、Updateの後に毎フレーム呼ばれる関数です。
            // TODO: targetのX座標に合わせてカメラを動かしましょう。Y/Zは変えません。
            // TODO: targetが入っていないときは何もしないようにしましょう。
        }
    }
}
