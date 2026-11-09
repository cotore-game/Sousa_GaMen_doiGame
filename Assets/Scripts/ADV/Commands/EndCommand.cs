using Cysharp.Threading.Tasks;
using ADV.Core;
using CSV4Unity;
using ADV.Data;

namespace ADV.Commands
{
    /// <summary>
    /// シナリオを終了して、次のシーンに遷移する
    /// </summary>
    public class EndCommand : CommandBase
    {
        public override bool ShouldEngineAwait => true;

        public override async UniTask ExecuteAsync(CsvRow<ScenarioFields> lineData, CancellableTask cancellable)
        {
            if (GameFlowManager.Instance != null)
            {
                GameFlowManager.Instance.GoToNextScene();
            }
            await UniTask.Yield();
        }
    }
}
