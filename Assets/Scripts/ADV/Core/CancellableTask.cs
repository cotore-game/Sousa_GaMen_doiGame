using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace ADV.Core
{
    /// <summary>
    /// CancellationTokenの管理を隠蔽するラッパークラス
    /// Dispose時に自動的にキャンセル＆リソース解放
    /// </summary>
    public sealed class CancellableTask : IDisposable
    {
        private CancellationTokenSource _cts;
        private bool _disposed;

        public CancellableTask()
        {
            _cts = new CancellationTokenSource();
        }

        /// <summary>
        /// キャンセルトークン（読み取り専用）
        /// </summary>
        public CancellationToken Token => _cts?.Token ?? CancellationToken.None;

        /// <summary>
        /// キャンセルが要求されているか
        /// </summary>
        public bool IsCancellationRequested => _cts?.IsCancellationRequested ?? true;

        /// <summary>
        /// キャンセルを要求
        /// </summary>
        public void Cancel()
        {
            if (_disposed || _cts == null) return;

            try
            {
                if (!_cts.IsCancellationRequested)
                {
                    _cts.Cancel();
                }
            }
            catch (ObjectDisposedException)
            {
                // 既に破棄済み
            }
        }

        /// <summary>
        /// 新しいCancellationTokenSourceにリセット
        /// </summary>
        public void Reset()
        {
            if (_disposed) return;

            _cts?.Dispose();
            _cts = new CancellationTokenSource();
        }

        public void Dispose()
        {
            if (_disposed) return;

            Cancel();
            _cts?.Dispose();
            _cts = null;
            _disposed = true;
        }
    }

    /// <summary>
    /// CancellableTask用の拡張メソッド
    /// </summary>
    public static class CancellableTaskExtensions
    {
        /// <summary>
        /// UniTaskにCancellableTaskのトークンを紐付け
        /// </summary>
        public static UniTask AttachCancellation(this UniTask task, CancellableTask cancellable)
        {
            return task.AttachExternalCancellation(cancellable.Token);
        }

        /// <summary>
        /// UniTask<T>にCancellableTaskのトークンを紐付け
        /// </summary>
        public static UniTask<T> AttachCancellation<T>(this UniTask<T> task, CancellableTask cancellable)
        {
            return task.AttachExternalCancellation(cancellable.Token);
        }
    }
}
