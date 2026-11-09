using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ADV.Data
{
    /// <summary>ADVシーン内でロード結果を共有し、シーン終了時にまとめて解放する。</summary>
    public sealed class NovelAssetLoader : IDisposable
    {
        private readonly NovelAssetCatalog _catalog;
        private readonly Dictionary<object, AsyncOperationHandle<Sprite>> _handles = new();
        private bool _disposed;

        public NovelAssetLoader(NovelAssetCatalog catalog)
        {
            _catalog = catalog != null ? catalog : throw new ArgumentNullException(nameof(catalog));
        }

        public UniTask<Sprite> LoadCharacterAsync(string name, string expression, CancellationToken token)
            => LoadAsync(_catalog.GetCharacter(name, expression), token);

        public UniTask<Sprite> LoadBackgroundAsync(string name, CancellationToken token = default)
            => LoadAsync(_catalog.GetBackground(name), token);

        private async UniTask<Sprite> LoadAsync(AssetReferenceSprite reference, CancellationToken token)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(NovelAssetLoader));
            token.ThrowIfCancellationRequested();
            if (reference == null || !reference.RuntimeKeyIsValid())
                throw new InvalidOperationException("Catalogの画像参照が未設定です。");

            object key = reference.RuntimeKey;
            if (!_handles.TryGetValue(key, out var handle))
            {
                handle = Addressables.LoadAssetAsync<Sprite>(key);
                _handles.Add(key, handle);
            }

            // ロード中のハンドルも共有する。待機のキャンセルでは共有ロードを解放しない。
            await UniTask.WaitUntil(() => _disposed || !handle.IsValid() || handle.IsDone, cancellationToken: token);
            token.ThrowIfCancellationRequested();
            if (_disposed) throw new OperationCanceledException("ADVの素材ローダーは終了しました。");
            if (!handle.IsValid() || handle.Status != AsyncOperationStatus.Succeeded)
            {
                var error = handle.IsValid() ? handle.OperationException : null;
                if (_handles.TryGetValue(key, out var cached) && cached.Equals(handle))
                {
                    _handles.Remove(key);
                    if (handle.IsValid()) Addressables.Release(handle);
                }
                throw new InvalidOperationException($"ノベル画像をロードできません: {key}", error);
            }
            return handle.Result;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            foreach (var handle in _handles.Values)
                if (handle.IsValid()) Addressables.Release(handle);
            _handles.Clear();
        }
    }
}
