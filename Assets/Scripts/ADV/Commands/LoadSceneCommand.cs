using Cysharp.Threading.Tasks;
using ADV.Core;
using CSV4Unity;
using CSV4Unity.Fields;
using System;
using SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace ADV.Commands
{
    /// <summary>
    /// 指定したシーンに遷移するコマンド
    /// Arg1: シーン名
    /// Arg2: (Optional) LoadSceneMode ("Single" or "Additive"). Default: "Single"
    /// </summary>
    public class LoadSceneCommand : CommandBase
    {
        private readonly SceneTransitioner _sceneTransitioner;

        // 待機型コマンド
        public override bool ShouldEngineAwait => true;

        public LoadSceneCommand(SceneTransitioner sceneTransitioner)
        {
            _sceneTransitioner = sceneTransitioner ?? throw new ArgumentNullException(nameof(sceneTransitioner));
        }

        public override async UniTask ExecuteAsync(LineData<ScenarioFields> lineData, CancellableTask cancellable)
        {
            string sceneName = lineData.GetOrDefault<string>(ScenarioFields.Arg1, null);
            string modeStr = lineData.GetOrDefault<string>(ScenarioFields.Arg2, "Single");

            // Arg1からSceneIdを取得
            SceneId? sceneId = SceneRegistry.GetSceneId(sceneName);
            if (!sceneId.HasValue)
            {
                Debug.LogError($"[LoadSceneCommand] Invalid scene name (not in Registry): {sceneName}");
                return;
            }

            // Arg2からLoadSceneModeを決定
            LoadSceneMode mode = modeStr.Equals("Additive", StringComparison.OrdinalIgnoreCase)
                ? LoadSceneMode.Additive
                : LoadSceneMode.Single;

            // シーン遷移を実行
            await _sceneTransitioner.TransitionToAsync(sceneId.Value, mode);
        }

        public override bool Validate(LineData<ScenarioFields> lineData, out string errorMessage)
        {
            string sceneName = lineData.GetOrDefault<string>(ScenarioFields.Arg1, null);

            if (string.IsNullOrWhiteSpace(sceneName))
            {
                errorMessage = "LoadScene command requires a scene name (Arg1)";
                return false;
            }

            // SceneRegistryに登録されているか確認
            if (!SceneRegistry.GetSceneId(sceneName).HasValue)
            {
                errorMessage = $"[LoadSceneCommand] Scene name '{sceneName}' (Arg1) not found in SceneRegistry.";
                return false;
            }

            errorMessage = null;
            return true;
        }
    }
}
