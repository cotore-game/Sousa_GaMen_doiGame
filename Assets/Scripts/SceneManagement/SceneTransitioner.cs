using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace SceneManagement
{
    /// <summary>
    /// シーン遷移を実行するシングルトンクラス。
    /// データのTargetSceneIdとTransitionToのSceneIdが一致することを実行時に検証します。
    /// SingleモードとAdditiveモードの両方に対応しています。
    /// </summary>
    public class SceneTransitioner : SingletonMonoBehaviour<SceneTransitioner>
    {
        /// <summary>
        /// シーン遷移開始時に発火するイベント。
        /// パラメータ: (SceneId, シーン名, データ型名, LoadSceneMode)
        /// </summary>
        public event Action<SceneId, string, string, LoadSceneMode> OnTransitionStarted;

        /// <summary>
        /// シーン遷移完了時に発火するイベント。
        /// パラメータ: (SceneId, シーン名, LoadSceneMode)
        /// </summary>
        public event Action<SceneId, string, LoadSceneMode> OnTransitionCompleted;

        private SceneId? _currentTransitionSceneId;
        private LoadSceneMode _currentLoadMode;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            // レジストリの検証
            if (!SceneRegistry.ValidateRegistry())
            {
                Debug.LogError("[SceneTransitioner] SceneRegistry validation failed!");
            }

            // シーンロード完了イベントをリッスン
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        /// <summary>
        /// データなしでシーン遷移を実行します。（同期ロード）
        /// </summary>
        /// <param name="sceneId">遷移先のシーンID</param>
        /// <param name="mode">読み込みモード（Single/Additive）</param>
        public void TransitionTo(SceneId sceneId, LoadSceneMode mode = LoadSceneMode.Single)
        {
            string sceneName = SceneRegistry.GetSceneName(sceneId);
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError($"[SceneTransitioner] Failed to get scene name for SceneId: {sceneId}");
                return;
            }

            Debug.Log($"[SceneTransitioner] Transition to '{sceneId}' ({sceneName}) without data (Sync, {mode})");

            _currentTransitionSceneId = sceneId;
            _currentLoadMode = mode;
            OnTransitionStarted?.Invoke(sceneId, sceneName, "None", mode);

            try
            {
                SceneManager.LoadScene(sceneName, mode);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SceneTransitioner] Failed to load scene '{sceneName}': {ex.Message}");
                _currentTransitionSceneId = null;
            }
        }

        /// <summary>
        /// データありでシーン遷移を実行します。（同期ロード）
        /// dataのTargetSceneIdとTransitionToのSceneIdが一致することを検証します。
        /// </summary>
        /// <typeparam name="TData">遷移時に渡すデータの型</typeparam>
        /// <param name="sceneId">遷移先のシーンID</param>
        /// <param name="data">遷移先に渡すデータ</param>
        /// <param name="mode">読み込みモード（Single/Additive）</param>
        public void TransitionTo<TData>(SceneId sceneId, TData data, LoadSceneMode mode = LoadSceneMode.Single)
            where TData : ISceneExchangeData
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Transition data cannot be null.");
            }

            // データのTargetSceneIdと引数のsceneIdが一致するか検証
            if (!data.TargetSceneId.Equals(sceneId))
            {
                throw new InvalidOperationException(
                    $"Scene ID mismatch: TransitionTo called with SceneId.{sceneId}, " +
                    $"but data.TargetSceneId is SceneId.{data.TargetSceneId}. " +
                    $"Data type: {typeof(TData).Name}");
            }

            string sceneName = SceneRegistry.GetSceneName(sceneId);
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError($"[SceneTransitioner] Failed to get scene name for SceneId: {sceneId}");
                return;
            }

            string dataTypeName = typeof(TData).Name;
            Debug.Log($"[SceneTransitioner] Transition to '{sceneId}' ({sceneName}) with '{dataTypeName}' (Sync, {mode})");

            // データをマネージャーに格納
            SceneExchangeManager.Instance.StoreData(data);

            _currentTransitionSceneId = sceneId;
            _currentLoadMode = mode;
            OnTransitionStarted?.Invoke(sceneId, sceneName, dataTypeName, mode);

            try
            {
                SceneManager.LoadScene(sceneName, mode);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SceneTransitioner] Failed to load scene '{sceneName}': {ex.Message}");
                SceneExchangeManager.Instance.ClearData<TData>();
                _currentTransitionSceneId = null;
            }
        }

        /// <summary>
        /// データなしで非同期シーン遷移を実行します。
        /// </summary>
        /// <param name="sceneId">遷移先のシーンID</param>
        /// <param name="mode">読み込みモード（Single/Additive）</param>
        /// <param name="progress">読み込み進捗コールバック(0.0～1.0)</param>
        public async UniTask TransitionToAsync(
            SceneId sceneId,
            LoadSceneMode mode = LoadSceneMode.Single,
            IProgress<float> progress = null)
        {
            string sceneName = SceneRegistry.GetSceneName(sceneId);
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError($"[SceneTransitioner] Failed to get scene name for SceneId: {sceneId}");
                return;
            }

            Debug.Log($"[SceneTransitioner] Async transition to '{sceneId}' ({sceneName}) without data ({mode})");

            _currentTransitionSceneId = sceneId;
            _currentLoadMode = mode;
            OnTransitionStarted?.Invoke(sceneId, sceneName, "None", mode);

            try
            {
                await SceneManager.LoadSceneAsync(sceneName, mode).ToUniTask(progress);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SceneTransitioner] Failed to load scene '{sceneName}': {ex.Message}");
                _currentTransitionSceneId = null;
            }
        }

        /// <summary>
        /// データありで非同期シーン遷移を実行します。
        /// dataのTargetSceneIdとTransitionToAsyncのSceneIdが一致することを検証します。
        /// </summary>
        /// <typeparam name="TData">遷移時に渡すデータの型</typeparam>
        /// <param name="sceneId">遷移先のシーンID</param>
        /// <param name="data">遷移先に渡すデータ</param>
        /// <param name="mode">読み込みモード（Single/Additive）</param>
        /// <param name="progress">読み込み進捗コールバック(0.0～1.0)</param>
        public async UniTask TransitionToAsync<TData>(
            SceneId sceneId,
            TData data,
            LoadSceneMode mode = LoadSceneMode.Single,
            IProgress<float> progress = null)
            where TData : ISceneExchangeData
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Transition data cannot be null.");
            }

            // データのTargetSceneIdと引数のsceneIdが一致するか検証
            if (!data.TargetSceneId.Equals(sceneId))
            {
                throw new InvalidOperationException(
                    $"Scene ID mismatch: TransitionToAsync called with SceneId.{sceneId}, " +
                    $"but data.TargetSceneId is SceneId.{data.TargetSceneId}. " +
                    $"Data type: {typeof(TData).Name}");
            }

            string sceneName = SceneRegistry.GetSceneName(sceneId);
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError($"[SceneTransitioner] Failed to get scene name for SceneId: {sceneId}");
                return;
            }

            string dataTypeName = typeof(TData).Name;
            Debug.Log($"[SceneTransitioner] Async transition to '{sceneId}' ({sceneName}) with '{dataTypeName}' ({mode})");

            // データをマネージャーに格納
            SceneExchangeManager.Instance.StoreData(data);

            _currentTransitionSceneId = sceneId;
            _currentLoadMode = mode;
            OnTransitionStarted?.Invoke(sceneId, sceneName, dataTypeName, mode);

            try
            {
                await SceneManager.LoadSceneAsync(sceneName, mode).ToUniTask(progress);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SceneTransitioner] Failed to load scene '{sceneName}': {ex.Message}");
                SceneExchangeManager.Instance.ClearData<TData>();
                _currentTransitionSceneId = null;
            }
        }

        /// <summary>
        /// 指定したシーンをアンロードします（非同期）。
        /// </summary>
        /// <param name="sceneId">アンロードするシーンID</param>
        /// <param name="progress">読み込み進捗コールバック(0.0～1.0)</param>
        public async UniTask UnloadSceneAsync(SceneId sceneId, IProgress<float> progress = null)
        {
            string sceneName = SceneRegistry.GetSceneName(sceneId);
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError($"[SceneTransitioner] Failed to get scene name for SceneId: {sceneId}");
                return;
            }

            Debug.Log($"[SceneTransitioner] Unloading scene '{sceneId}' ({sceneName})");

            try
            {
                await SceneManager.UnloadSceneAsync(sceneName).ToUniTask(progress);
                Debug.Log($"[SceneTransitioner] Scene unloaded: '{sceneId}' ({sceneName})");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SceneTransitioner] Failed to unload scene '{sceneName}': {ex.Message}");
            }
        }

        /// <summary>
        /// 指定したシーンが現在ロードされているかを確認します。
        /// </summary>
        /// <param name="sceneId">確認するシーンID</param>
        /// <returns>シーンがロードされている場合はtrue</returns>
        public bool IsSceneLoaded(SceneId sceneId)
        {
            string sceneName = SceneRegistry.GetSceneName(sceneId);
            if (string.IsNullOrEmpty(sceneName))
            {
                return false;
            }

            Scene scene = SceneManager.GetSceneByName(sceneName);
            return scene.isLoaded;
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_currentTransitionSceneId.HasValue)
            {
                Debug.Log($"[SceneTransitioner] Scene loaded: {_currentTransitionSceneId.Value} ({scene.name}, {mode})");
                OnTransitionCompleted?.Invoke(_currentTransitionSceneId.Value, scene.name, _currentLoadMode);
                _currentTransitionSceneId = null;
            }
        }
    }
}
