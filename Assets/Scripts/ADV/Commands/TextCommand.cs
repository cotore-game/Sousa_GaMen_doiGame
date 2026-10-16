using Cysharp.Threading.Tasks;
using UnityEngine;
using ADV.Core;
using ADV.Presentation;
using CSV4Unity;
using ADV.Data;
using System;

namespace ADV.Commands
{
    /// <summary>
    /// テキスト表示コマンド
    /// Arg1: キャラ名, Arg2: 表示間隔(ms), Arg3: スキップ閾値(%), Text: 本文
    /// </summary>
    public class TextCommand : CommandBase
    {
        private readonly TextPresenter _textPresenter;

        // 待機型コマンド
        public override bool ShouldEngineAwait => true;

        private const int DEFAULT_INTERVAL = 50;
        private const int DEFAULT_SKIP_THRESHOLD = 30;

        /// <summary>
        /// コンストラクタインジェクション
        /// </summary>
        public TextCommand(TextPresenter textPresenter)
        {
            _textPresenter = textPresenter ?? throw new ArgumentNullException(nameof(textPresenter));
        }

        public override async UniTask ExecuteAsync(CsvRow<ScenarioFields> lineData, CancellableTask cancellable)
        {
            // パラメータ取得
            string characterName = GetOrDefault<string>(lineData, ScenarioFields.Arg1, "");
            int interval = GetOrDefault<int>(lineData, ScenarioFields.Arg2, DEFAULT_INTERVAL);
            int skipThreshold = GetOrDefault<int>(lineData, ScenarioFields.Arg3, DEFAULT_SKIP_THRESHOLD);
            string bodyText = GetOrDefault<string>(lineData, ScenarioFields.Text, "");

            if (string.IsNullOrEmpty(bodyText))
            {
                Debug.LogWarning("[TextCommand] Text is empty");
                return;
            }

            bodyText = $"<rotate=90>{bodyText}</rotate>";

            // TextPresenterに委譲
            await _textPresenter.DisplayTextAsync(
                characterName,
                bodyText,
                interval,
                skipThreshold,
                cancellable
            );
        }

        public override bool Validate(CsvRow<ScenarioFields> lineData, out string errorMessage)
        {
            errorMessage = null;

            var text = GetOrDefault<string>(lineData, ScenarioFields.Text, null);
            if (string.IsNullOrWhiteSpace(text))
            {
                errorMessage = "Text command requires non-empty Text field";
                return false;
            }

            return true;
        }
    }
}
