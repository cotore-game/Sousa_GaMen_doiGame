using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SceneManagement
{
    /// <summary>
    /// SceneIdとシーン名の1:1対応を管理するレジストリ。
    /// </summary>
    public static class SceneRegistry
    {
        // SceneId -> シーン名の辞書
        private static readonly Dictionary<SceneId, string> _sceneNames = new Dictionary<SceneId, string>();

        // シーン名 -> SceneIdの逆引き辞書
        private static readonly Dictionary<string, SceneId> _reverseMapping = new Dictionary<string, SceneId>();

        // 初期化済みフラグ
        private static bool _isInitialized = false;

        /// <summary>
        /// レジストリを初期化します。
        /// ゲーム起動時に一度だけ呼び出されます。
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (_isInitialized) return;

            // ========================================
            // ここにシーンのマッピングを定義
            // ========================================
            Register(SceneId.Title, "TitleScene");
            Register(SceneId.ADV, "ADVScene");
            Register(SceneId.Stage1, "Stage1Scene");
            Register(SceneId.Stage2, "Stage2Scene");

            _isInitialized = true;
            Debug.Log("[SceneRegistry] Initialized successfully.");
        }

        /// <summary>
        /// SceneIdとシーン名を登録します。
        /// </summary>
        private static void Register(SceneId sceneId, string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError($"[SceneRegistry] Scene name cannot be null or empty for SceneId: {sceneId}");
                return;
            }

            // 重複チェック
            if (_sceneNames.ContainsKey(sceneId))
            {
                Debug.LogError($"[SceneRegistry] SceneId '{sceneId}' is already registered.");
                return;
            }

            if (_reverseMapping.TryGetValue(sceneName, out SceneId existingSceneId))
            {
                Debug.LogError($"[SceneRegistry] Scene name '{sceneName}' is already registered to SceneId '{existingSceneId}'.");
                return;
            }

            _sceneNames[sceneId] = sceneName;
            _reverseMapping[sceneName] = sceneId;
        }

        /// <summary>
        /// SceneIdからシーン名を取得します。
        /// </summary>
        public static string GetSceneName(SceneId sceneId)
        {
            EnsureInitialized();

            if (_sceneNames.TryGetValue(sceneId, out string sceneName))
            {
                return sceneName;
            }

            Debug.LogError($"[SceneRegistry] No scene name registered for SceneId: {sceneId}");
            return null;
        }

        /// <summary>
        /// シーン名からSceneIdを取得します。
        /// </summary>
        public static SceneId? GetSceneId(string sceneName)
        {
            EnsureInitialized();

            if (_reverseMapping.TryGetValue(sceneName, out SceneId sceneId))
            {
                return sceneId;
            }

            return null;
        }

        /// <summary>
        /// SceneIdが登録されているかを確認します。
        /// </summary>
        public static bool IsRegistered(SceneId sceneId)
        {
            EnsureInitialized();
            return _sceneNames.ContainsKey(sceneId);
        }

        /// <summary>
        /// レジストリの検証を行います。
        /// すべてのSceneId列挙値が登録されているかをチェックします。
        /// テスト用シーンや開発用シーンなど、意図的に登録されていないSceneIdがある場合は警告を出力します。
        /// </summary>
        public static bool ValidateRegistry()
        {
            EnsureInitialized();

            bool hasWarnings = false;
            var allSceneIds = (SceneId[])Enum.GetValues(typeof(SceneId));

            // 未登録のSceneIdを明示的にフィルタリング
            var unregisteredSceneIds = allSceneIds.Where(sceneId => !IsRegistered(sceneId)).ToList();

            if (unregisteredSceneIds.Count > 0)
            {
                Debug.LogWarning($"[SceneRegistry] {unregisteredSceneIds.Count} SceneId(s) are not registered. " +
                                 $"This may be intentional for test/development scenes: {string.Join(", ", unregisteredSceneIds)}");
                hasWarnings = true;
            }

            // 登録数と列挙値数の不一致を警告として扱う
            if (_sceneNames.Count != allSceneIds.Length)
            {
                Debug.LogWarning($"[SceneRegistry] Registry count mismatch: {_sceneNames.Count} registered scenes vs {allSceneIds.Length} SceneId enum values. " +
                                 $"This is acceptable if some SceneIds are intentionally unregistered.");
                hasWarnings = true;
            }

            if (!hasWarnings)
            {
                Debug.Log($"[SceneRegistry] Validation passed. All {allSceneIds.Length} SceneId values are registered.");
            }

            // 警告があってもtrueを返す
            return true;
        }

        private static void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                Initialize();
            }
        }
    }
}
