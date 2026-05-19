using System;
using System.Collections.Generic;
using UnityEngine;

namespace SceneManagement
{
    /// <summary>
    /// シーン間のデータ受け渡しを管理するシングルトンクラス。
    /// シーン遷移時にデータを一時的に保持し、遷移先のシーンで取得できるようにします。
    /// </summary>
    public class SceneExchangeManager : SingletonMonoBehaviour<SceneExchangeManager>
    {
        // シーン遷移中に一時的にデータを保持する辞書
        // Key: データの型, Value: データインスタンス
        private readonly Dictionary<Type, ISceneExchangeData> _dataStorage = new Dictionary<Type, ISceneExchangeData>();

        /// <summary>
        /// シーン遷移用のデータを格納します（内部使用）。
        /// 同じ型のデータが既に格納されている場合は上書きされます。
        /// </summary>
        internal void StoreData<TData>(TData data) where TData : ISceneExchangeData
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Cannot store null data.");
            }

            Type dataType = typeof(TData);
            _dataStorage[dataType] = data;

            Debug.Log($"[SceneExchangeManager] Data stored for type: {dataType.Name}");
        }

        /// <summary>
        /// 格納されたデータを取得し、ストレージから削除します。
        /// データが存在しない場合はdefault(TData)を返します。
        /// </summary>
        /// <typeparam name="TData">取得するデータの型</typeparam>
        /// <returns>格納されていたデータ。データが存在しない場合はdefault(TData)</returns>
        public TData GetData<TData>() where TData : class, ISceneExchangeData
        {
            Type dataType = typeof(TData);

            if (_dataStorage.TryGetValue(dataType, out ISceneExchangeData data))
            {
                _dataStorage.Remove(dataType);
                Debug.Log($"[SceneExchangeManager] Data retrieved and cleared for type: {dataType.Name}");
                return (TData)data;
            }

            Debug.LogWarning($"[SceneExchangeManager] No data found for type: {dataType.Name}. Returning default (null).");
            return default;
        }

        /// <summary>
        /// 格納されたデータを取得しますが、ストレージからは削除しません。
        /// データの確認や複数回の取得が必要な場合に使用します。
        /// </summary>
        /// <typeparam name="TData">取得するデータの型</typeparam>
        /// <returns>格納されていたデータ。データが存在しない場合はdefault(TData)</returns>
        public TData PeekData<TData>() where TData : class, ISceneExchangeData
        {
            Type dataType = typeof(TData);

            if (_dataStorage.TryGetValue(dataType, out ISceneExchangeData data))
            {
                return (TData)data;
            }

            return default;
        }

        /// <summary>
        /// 指定した型のデータがストレージに存在するかを確認します。
        /// </summary>
        /// <typeparam name="TData">確認するデータの型</typeparam>
        /// <returns>データが存在する場合はtrue、存在しない場合はfalse</returns>
        public bool HasData<TData>() where TData : ISceneExchangeData
        {
            return _dataStorage.ContainsKey(typeof(TData));
        }

        /// <summary>
        /// 指定した型のデータをストレージから削除します。
        /// </summary>
        /// <typeparam name="TData">削除するデータの型</typeparam>
        /// <returns>データが存在して削除された場合はtrue、存在しない場合はfalse</returns>
        public bool ClearData<TData>() where TData : ISceneExchangeData
        {
            Type dataType = typeof(TData);
            bool removed = _dataStorage.Remove(dataType);

            if (removed)
            {
                Debug.Log($"[SceneExchangeManager] Data cleared for type: {dataType.Name}");
            }

            return removed;
        }

        /// <summary>
        /// ストレージ内のすべてのデータを削除します。
        /// </summary>
        public void ClearAllData()
        {
            int count = _dataStorage.Count;
            _dataStorage.Clear();
            Debug.Log($"[SceneExchangeManager] All data cleared. {count} entries removed.");
        }

        /// <summary>
        /// 現在ストレージに格納されているデータの数を取得します。
        /// </summary>
        public int StoredDataCount => _dataStorage.Count;
    }
}
