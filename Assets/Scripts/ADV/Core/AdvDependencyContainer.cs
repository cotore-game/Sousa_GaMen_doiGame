using System;
using System.Collections.Generic;
using UnityEngine;

namespace ADV.Core
{
    /// <summary>
    /// ADVシステム専用の軽量DIコンテナ
    /// シーンスコープの依存関係を管理
    /// </summary>
    public class AdvDependencyContainer : IDisposable
    {
        private readonly Dictionary<Type, object> _services = new();
        private readonly List<IDisposable> _disposables = new();
        private bool _disposed;

        /// <summary>
        /// サービスを登録（シングルトンインスタンス）
        /// </summary>
        public void Register<TService>(TService instance) where TService : class
        {
            if (_disposed)
            {
                Debug.LogWarning("[AdvDI] Cannot register to disposed container");
                return;
            }

            var type = typeof(TService);

            if (_services.ContainsKey(type))
            {
                Debug.LogWarning($"[AdvDI] Service {type.Name} is already registered. Overwriting.");
            }

            _services[type] = instance;

            // IDisposableなら破棄リストに追加
            if (instance is IDisposable disposable)
            {
                _disposables.Add(disposable);
            }
        }

        /// <summary>
        /// サービスを取得
        /// </summary>
        public TService Resolve<TService>() where TService : class
        {
            if (_disposed)
            {
                Debug.LogError("[AdvDI] Cannot resolve from disposed container");
                return null;
            }

            var type = typeof(TService);

            if (_services.TryGetValue(type, out var service))
            {
                return service as TService;
            }

            Debug.LogError($"[AdvDI] Service {type.Name} not found");
            return null;
        }

        /// <summary>
        /// サービスの取得を試行
        /// </summary>
        public bool TryResolve<TService>(out TService service) where TService : class
        {
            if (_disposed)
            {
                service = null;
                return false;
            }

            var type = typeof(TService);

            if (_services.TryGetValue(type, out var obj))
            {
                service = obj as TService;
                return service != null;
            }

            service = null;
            return false;
        }

        /// <summary>
        /// サービスが登録されているか確認
        /// </summary>
        public bool IsRegistered<TService>() where TService : class
        {
            return !_disposed && _services.ContainsKey(typeof(TService));
        }

        /// <summary>
        /// 全サービスをクリア
        /// </summary>
        public void Clear()
        {
            _services.Clear();
        }

        public void Dispose()
        {
            if (_disposed) return;

            // 登録されたIDisposableを逆順で破棄
            for (int i = _disposables.Count - 1; i >= 0; i--)
            {
                try
                {
                    _disposables[i]?.Dispose();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[AdvDI] Error disposing service: {ex}");
                }
            }

            _disposables.Clear();
            _services.Clear();
            _disposed = true;
        }
    }
}
