using Cysharp.Threading.Tasks;
using ADV.Core;
using ADV.Presentation;
using CSV4Unity;
using CSV4Unity.Fields;
using System;

namespace ADV.Commands
{
    /// <summary>
    /// 表示されている全てのキャラクターを非表示（フェードアウト）させるコマンド
    /// </summary>
    public class HideCharacterCommand : CommandBase
    {
        private readonly CharacterPresenter _characterPresenter;

        // 待機型コマンド
        public override bool ShouldEngineAwait => true;

        /// <summary>
        /// コンストラクタインジェクション
        /// </summary>
        public HideCharacterCommand(CharacterPresenter characterPresenter)
        {
            _characterPresenter = characterPresenter ?? throw new ArgumentNullException(nameof(characterPresenter));
        }

        public override async UniTask ExecuteAsync(LineData<ScenarioFields> lineData, CancellableTask cancellable)
        {
            // CharacterPresenterのClearAllを呼ぶ
            await _characterPresenter.ClearAll(cancellable);
        }

        public override bool Validate(LineData<ScenarioFields> lineData, out string errorMessage)
        {
            // このコマンドに引数は不要
            errorMessage = null;
            return true;
        }
    }
}
