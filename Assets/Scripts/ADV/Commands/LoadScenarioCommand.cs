using Cysharp.Threading.Tasks;
using ADV.Core;
using ADV.System;
using CSV4Unity;
using CSV4Unity.Fields;
using System;
using UnityEngine;

namespace ADV.Commands
{
    /// <summary>
    /// シナリオ読み込みコマンド
    /// Arg1: 読み込むシナリオファイル名（Resources/Datas以下）
    /// </summary>
    public class LoadScenarioCommand : CommandBase
    {
        public override bool ShouldEngineAwait => true;

        public override async UniTask ExecuteAsync(LineData<ScenarioFields> lineData, CancellableTask cancellable)
        {
            string scenarioName = lineData.GetOrDefault<string>(ScenarioFields.Arg1, null);

            if (!string.IsNullOrWhiteSpace(scenarioName))
            {
                scenarioName = scenarioName.Trim();
                TextAsset textAsset = Resources.Load<TextAsset>($"Datas/{scenarioName}");
                
                if (textAsset != null)
                {
                    // AdvScenarioExecutorにシナリオの読み込みと実行を依頼
                    // LoadAndExecuteScenario()が呼び出されると、現在のシナリオ実行が破棄（キャンセル）される
                    AdvScenarioExecutor.Instance.LoadAndExecuteScenario(textAsset).Forget();
                    
                    // 現在のシナリオ実行がキャンセルされるまで待機して、続くコマンドを防止
                    if (cancellable != null && cancellable.Token.CanBeCanceled)
                    {
                        try
                        {
                            await UniTask.WaitUntilCanceled(cancellable.Token);
                        }
                        catch (OperationCanceledException)
                        {
                            // キャンセルによる例外は無視する
                        }
                    }
                }
                else
                {
                    Debug.LogError($"[LoadScenarioCommand] Scenario not found: Resources/Datas/{scenarioName}");
                }
            }
            else
            {
                Debug.LogWarning("[LoadScenarioCommand] Arg1 (Scenario name) is empty.");
            }
        }
    }
}
