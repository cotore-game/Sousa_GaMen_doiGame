using UnityEngine;

namespace ActionPrototype
{
    public sealed class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;

        private void LateUpdate()
        {
            // TODO (課題2): targetのX座標を追い、YとZは固定する。
            // TODO (課題2): 開始地点より左へカメラが戻らないようにする。
            // TODO (課題2): targetが未設定または消えた場合を安全に扱う。
        }
    }
}
