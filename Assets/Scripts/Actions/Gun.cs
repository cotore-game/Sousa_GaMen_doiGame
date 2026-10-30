using UnityEngine;

namespace ActionPrototype
{
    /// <summary>
    /// 【担当】火縄銃の所持、装填工程、工程の経過時間、構え、発射可否。操作入力はPlayerが渡す。
    /// Playerと銃の絵は一体だが、処理の責務は分けられる。Player.prefabと同じルートに付けて使う。
    /// MonoBehaviourなのでnew Gun()では作らず、PlayerのGetComponent&lt;Gun&gt;()から利用する。
    /// 【装填仕様】1:Eで火薬注入(1.0秒)、2:Dで弾丸挿入(1.0秒)、3:下矢印を2〜4回押し込む。
    /// 必要回数は最初の押し込みで決定。ただし要件を満たせば決定タイミングは厳密でなくてもよい。
    /// 4:Eで口薬注入(1.0秒)、5:Tで火蓋を閉じる(即時)、6:Aで火バサミを半分上げる(即時)、
    /// 7:Nで火縄を挟む(1.0秒)、8:Aで火バサミを上げる(即時)。8の完了で装填済みになる。
    /// 各工程は直前の工程が完了したときのみ受付。時間のかかる工程中に次の入力を通さない。
    /// 押し込みは押下ごとに1回数え、長押しを何回も数えない。最後の押下で押し込みモードを解除。
    /// 歩行・方向転換・ジャンプで装填と構えを中断する。中断した工程の再開方法は仕様に明記がないため相談する。
    /// Kで構え、構え中かつ装填済みでUを押すと向いている方向へ1発。未装填ならUは何もしない。
    /// 発射すると装填済み状態を解除する。追加の発射クールダウン秒数は仕様未指定。
    /// </summary>
    /// <remarks>
    /// 【依存】GunはPlayerを参照しない。Playerから入力の意味と向きだけを受け取る。
    /// Gun→Bullet.Initializeで方向を渡す。BulletはGunを参照しない。
    /// 【準備済み】bulletPrefab=Assets/Prefabs/Actions/Bullet.prefab。
    /// PlayerのMuzzleRight/MuzzleLeftは発射位置の目安。向きに合うTransformを使う。
    /// Instantiate(bulletPrefab, 発射位置, Quaternion.identity)で複製し、そのBullet.Initialize(Vector2.right/left)へ渡す。
    /// 銃専用の別SpriteRendererは不要。PlayerのSetLocomotionAnimationがHasGun(bool)をAnimatorへ伝える。
    /// 銃ありClipはAssets/Animations/Actions/Player/GunIdleRight.anim・GunIdleLeft.anim・GunWalkRight.anim・GunWalkLeft.anim。
    /// 画像はAssets/Sprites/Action/TeruoNakata/GunWalkRight01.png・02.png／GunWalkLeft01.png・02.png。
    /// 火薬注入／弾丸挿入／押し込み／火縄挟み／構え専用の画像は現在の素材にない。
    /// その工程は銃あり待機画像で仮表示する。未作成Clip名をAnimator.Playへ渡さない。
    /// 押し込み中の押下／非押下による絵の切替や、工程7/8の歩行差分も素材追加後の対応になる。
    /// 装填時間はTime.deltaTimeなどで管理。アニメ速度を変えても1.0秒という仕様を変えない。
    /// Devでは銃ありを初期設定。本番ステージ1は攻撃手段がないのでisEquippedをfalseにする。
    /// 仕様元: https://docs.google.com/document/d/1jVNNvVS2BbfkY3wJzQv73Hr0PVPPb0P7_iovSdvB80Y/edit?tab=t.6v0tp4lmqdo1
    /// </remarks>
    [DisallowMultipleComponent]
    public class Gun : MonoBehaviour
    {
        // EとAは工程によって意味が変わる。同じ入力を現在の工程に応じて判断するための入口。
        public enum ReloadCommand { Powder, InsertBullet, Ram, ClosePan, RaiseHammer, InsertMatch }

        [SerializeField] private bool isEquipped = true;
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private Transform muzzleRight;
        [SerializeField] private Transform muzzleLeft;
        [SerializeField, Min(0f)] private float loadingDuration = 1f;

        public bool IsEquipped => isEquipped;
        public bool IsLoaded { get; private set; }
        public bool IsAiming { get; private set; }
        public Bullet BulletPrefab => bulletPrefab;
        public Transform MuzzleRight => muzzleRight;
        public Transform MuzzleLeft => muzzleLeft;
        public float LoadingDuration => loadingDuration;

        private void Update()
        {
            // TODO (課題4): 現在工程と経過時間を保持し、時間が必要な工程だけloadingDuration秒待つ。
            // 入力をここで直接読まない。PlayerからHandleReloadInputへ指示を受ける。
        }

        public void HandleReloadInput(ReloadCommand command)
        {
            // TODO (課題4): 未所持なら受付せず、工程に対応した入力だけ受け付ける。
            // 現在工程、処理中か、押し込み必要回数／実施回数をどう保持するか考える。
            // 整数のRandom.Range(2, 5)は2〜4。上限5は含まれない。
            // EとAの再利用、処理中の連打、工程の飛び越しを確認する。
        }

        public void TryAim()
        {
            // TODO (課題4): 所持時に構える。IsAimingの変更はGun内に閉じ込める。
        }

        public void TryFire(bool facingLeft)
        {
            // TODO (課題4): 所持・構え・装填済み・Prefab／発射位置の設定を確認して1発だけ生成する。
            // 生成したBulletへInitialize(向き)で指示し、IsLoadedを解除する。
            // UpdateからUのwasPressedThisFrameで呼び、長押しで同じ弾を連発しないようにする。
        }

        public void CancelAction()
        {
            // TODO (課題4): 構えと進行中の装填を中断する。完了済み工程の扱いは相談して決める。
        }
    }
}
