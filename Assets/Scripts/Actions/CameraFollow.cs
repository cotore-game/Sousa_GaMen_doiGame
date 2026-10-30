using UnityEngine;

namespace ActionPrototype
{
    /// <summary>
    /// 【担当】PDFのCameraクラスに相当。プレイヤーの位置へカメラを追従させる。
    /// UnityEngine.Cameraとの名前の衝突を避け、既存のCameraFollowという名前を使う。
    /// PDFは追従を指定しているが、追従速度や画面端の制約は数値指定していない。
    /// 【課題2の方針】まずXだけ追従し、開始時のY/Zは固定。開始地点より左へ戻らないようにする。
    /// これは今回の課題用の方針で、Google仕様書に細かい追従条件が書かれているわけではない。
    /// </summary>
    /// <remarks>
    /// Assets/Scenes/Dev/ActionDev.unityのMain Cameraに取付済み。Targetには同シーンのPlayerを設定する。
    /// 必要なのはTransformの位置だけ。PlayerのHP・ジャンプ状態・Gunへの参照は不要。
    /// InspectorのOrthographic Sizeが見える範囲。現在4.5。追従ロジックの速度とは別の値。
    /// Playerの物理更新の後にLateUpdateで追従すると見た目を揃えやすい。
    /// 実装前のDevカメラは固定。既存の画像・Animatorを操作する役割は持たない。
    /// </remarks>
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;

        public Transform Target => target;

        private void Awake()
        {
            // TODO (課題2): 開始時の位置を保存し、固定するY/Zと左端の基準を考える。
        }

        private void LateUpdate()
        {
            // TODO (課題2): target未設定／消滅時は何もせず、それ以外ではX座標へ追従する。
            // TODO (課題2): Y/Zを固定し、開始地点より左へ戻らないようにする。
            // 滑らかに追従させるならMathf.SmoothDamp等を調べる。まず即時追従からでもよい。
        }
    }
}
