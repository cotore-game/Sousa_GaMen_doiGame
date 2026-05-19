using Cysharp.Threading.Tasks;
using ADV.Core;
using CSV4Unity;
using CSV4Unity.Fields;

namespace ADV.Commands
{
    /// <summary>
    /// シナリオを終了して、次のシーンに遷移する
    /// </summary>
    public class EndCommand : CommandBase
    {
        public override bool ShouldEngineAwait => true;

        public override async UniTask ExecuteAsync(LineData<ScenarioFields> lineData, CancellableTask cancellable)
        {
            // GameFlowManager.Instance.GoToNextScene();
            await UniTask.Yield();
        }
    }
}
