using UnityEngine;

namespace ActionPrototype
{
    /// <summary>
    /// 銃への弾の装填と、発射を管理するクラスです。
    /// キーの入力を受け取る部分は準備済みです。下のTODOを実装しましょう。
    ///
    /// 装填は次の順番で行います。
    /// 1. 火薬を入れる: 1秒待つ。
    /// 2. 弾を入れる: 1秒待つ。
    /// 3. 弾を押し込む: 2〜4回押す。
    /// 4. 口薬を入れる: 1秒待つ。
    /// 5. 火蓋を閉じる。
    /// 6. 火バサミを半分上げる。
    /// 7. 火縄を挟む: 1秒待つ。
    /// 8. 火バサミを上げる。これで発射の準備完了です。
    ///
    /// テスト用キー: Eで火薬、Rで弾、Fで押し込み、Tで火蓋、Qで火バサミ、Nで火縄。
    /// Kで構え、Uで発射します。弾を撃つには、装填と構えの両方が必要です。
    /// 撃ったら未装填に戻ります。歩く・向きを変える・跳ぶと、装填や構えを中断します。
    ///
    /// 弾のPrefab: Assets/Prefabs/Actions/Bullet.prefab
    /// 銃を持ったアニメーション: Assets/Animations/Actions/Player/GunWalkRight.anim など。
    /// 装填中の画像はまだないので、今は銃を持った待機画像を使います。
    /// </summary>
    [DisallowMultipleComponent]
    public class Gun : MonoBehaviour
    {
        // enumは、用意した選択肢から1つを選ぶための型です。
        // ReloadCommandは「どの装填操作をしたか」を表します。
        // 例: HandleReloadInput(ReloadCommand.Powder); で火薬を入れる操作を渡せます。
        public enum ReloadCommand
        {
            Powder,       // 火薬を入れる
            InsertBullet, // 弾を入れる
            Ram,          // 弾を押し込む
            ClosePan,     // 火蓋を閉じる
            RaiseHammer,  // 火バサミを上げる
            InsertMatch   // 火縄を挟む
        }

        [SerializeField]
        [Tooltip("銃を持っていればチェックを入れます。テストではオンにしてあります。")]
        private bool isEquipped = true;

        [SerializeField]
        [Tooltip("発射するときに複製する弾です。Bullet.prefabを設定済みです。")]
        private Bullet bulletPrefab;

        [SerializeField]
        [Tooltip("右向きで撃ったときに弾が出る位置です。MuzzleRightを設定済みです。")]
        private Transform muzzleRight;

        [SerializeField]
        [Tooltip("左向きで撃ったときに弾が出る位置です。MuzzleLeftを設定済みです。")]
        private Transform muzzleLeft;

        [SerializeField, Min(0f)]
        [Tooltip("装填で待つ工程の秒数です。火薬・弾・口薬・火縄で、それぞれ1秒待ちます。")]
        private float loadingDuration = 1f;

        // boolはtrue（はい）かfalse（いいえ）のどちらかを入れる型です。
        public bool IsEquipped => isEquipped; // 銃を持っているか
        public bool IsLoaded { get; private set; } // 装填が終わったか
        public bool IsAiming { get; private set; } // 銃を構えているか

        // 設定した弾・発射位置・待ち時間を、ほかのクラスから読むためのものです。
        public Bullet BulletPrefab => bulletPrefab;
        public Transform MuzzleRight => muzzleRight;
        public Transform MuzzleLeft => muzzleLeft;
        public float LoadingDuration => loadingDuration;

        private void Update()
        {
            // Updateには、毎フレーム行う処理を書きます。
            // TODO: 時間が必要な工程では、loadingDuration秒経ったら次へ進めるようにしましょう。
        }

        public void HandleReloadInput(ReloadCommand command)
        {
            // commandには、上のenumのどれか1つが入ってきます。
            // TODO: 今の装填段階を覚えて、正しい順番の操作だけ受け付けましょう。
            // Powderは工程1と4、RaiseHammerは工程6と8で使います。
            // TODO: 押し込む回数は2〜4回から決めましょう。押した回数も数えます。
        }

        public void TryAim()
        {
            // TODO: 銃を持っていたら、IsAimingをtrueにしましょう。
        }

        public void TryFire(bool facingLeft)
        {
            // facingLeftがtrueなら左、falseなら右に撃ちます。
            // TODO: 銃を持ち、装填が終わり、構えているときだけ弾を1つ作りましょう。
            // 弾を作るにはInstantiateを使います。bulletPrefabが複製する弾です。
            // 作る位置にはmuzzleLeft.positionかmuzzleRight.positionを使います。
            // 作った弾のInitializeに、Vector2.leftかVector2.rightを渡しましょう。
            // TODO: 撃ったらIsLoadedをfalseにして、もう一度装填が必要になるようにしましょう。
        }

        public void CancelAction()
        {
            // TODO: 途中の装填や構えをやめる処理を書きましょう。
        }
    }
}
