using System;
using Cysharp.Threading.Tasks;
using ADV.Data;
using System.Threading;

namespace ADV.Presentation
{
    /// <summary>
    /// 背景表示のPresenter層
    /// View への参照のみ持ち、ゲームロジックを持たない
    /// </summary>
    public class BackgroundPresenter : IDisposable
    {
        private readonly BackgroundView _view;
        private readonly NovelAssetLoader _assetLoader;
        private readonly CancellationTokenSource _lifetime = new();

        public BackgroundPresenter(BackgroundView view, NovelAssetLoader assetLoader)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _assetLoader = assetLoader ?? throw new ArgumentNullException(nameof(assetLoader));
        }

        public async UniTask ChangeBackgroundAsync(string bgName, CancellationToken token = default)
        {
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(_lifetime.Token, token);
            var sprite = await _assetLoader.LoadBackgroundAsync(bgName, linked.Token);
            linked.Token.ThrowIfCancellationRequested();
            _view.SetSprite(sprite);
        }

        public void Dispose()
        {
            _lifetime.Cancel();
            _lifetime.Dispose();
        }
    }
}
