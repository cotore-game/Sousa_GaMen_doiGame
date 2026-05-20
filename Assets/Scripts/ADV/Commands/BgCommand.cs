using Cysharp.Threading.Tasks;
using ADV.Core;
using ADV.Presentation;
using CSV4Unity;
using CSV4Unity.Fields;
using System;
using UnityEngine;

namespace ADV.Commands
{
    /// <summary>
    /// 背景切り替えコマンド
    /// Arg1: 背景名
    /// </summary>
    public class BgCommand : CommandBase
    {
        private readonly BackgroundPresenter _presenter;

        public BgCommand(BackgroundPresenter presenter)
        {
            _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
        }

        // 待機するコマンド
        public override bool ShouldEngineAwait => true;

        public override async UniTask ExecuteAsync(LineData<ScenarioFields> lineData, CancellableTask cancellable)
        {
            string bgName = lineData.GetOrDefault<string>(ScenarioFields.Arg1, null);

            if (!string.IsNullOrWhiteSpace(bgName))
            {
                await _presenter.ChangeBackgroundAsync(bgName.Trim());
            }
            else
            {
                Debug.LogWarning("[BgCommand] Arg1 (Background name) is empty.");
            }
        }
    }
}
