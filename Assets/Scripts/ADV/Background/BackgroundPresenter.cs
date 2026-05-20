using System;
using Cysharp.Threading.Tasks;

namespace ADV.Presentation
{
    /// <summary>
    /// 背景表示のPresenter層
    /// View への参照のみ持ち、ゲームロジックを持たない
    /// </summary>
    public class BackgroundPresenter : IDisposable
    {
        private readonly BackgroundView _view;

        public BackgroundPresenter(BackgroundView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
        }

        public async UniTask ChangeBackgroundAsync(string bgName)
        {
            _view.ChangeBackground(bgName);
            
            // 将来的にクロスフェード演出などを実装する場合はここで待機処理を追加・実装します
            await UniTask.Yield();
        }

        public void Dispose()
        {
        }
    }
}
