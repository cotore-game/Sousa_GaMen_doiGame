using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ADV.Core;
using ADV.Presentation;
using ADV.System;
using CSV4Unity;
using CSV4Unity.Fields;

namespace ADV.Commands
{
    /// <summary>
    /// 選択肢コマンド
    ///
    /// [CSV 書き方]
    /// 単一行で選択肢を定義します。
    ///
    ///   Command | Text           | Arg1   | Arg2   | Arg3
    ///   --------+----------------+--------+--------+-------
    ///   Choice  | どっちがすき？ | りんご | バナナ | メロン
    ///
    ///   ・Text を質問文として使います
    ///   ・Arg1 ～ Arg6 が各選択肢（ラベル）になります。空のフィールドは無視されます。
    ///
    /// [遷移先 CSV]
    /// Resources/Datas/{現在のCSV名}_{選択ラベル}.csv
    /// 例: 序章.csv 再生中に「りんご」を選んだ場合 → Resources/Datas/序章_りんご
    /// </summary>
    public class ChoiceCommand : CommandBase
    {
        private readonly ChoicePresenter _choicePresenter;

        public ChoiceCommand(ChoicePresenter choicePresenter)
        {
            _choicePresenter = choicePresenter ?? throw new ArgumentNullException(nameof(choicePresenter));
        }

        // 選択完了まで次のコマンドに進まない
        public override bool ShouldEngineAwait => true;

        // 単一行での実行
        public override async UniTask ExecuteAsync(LineData<ScenarioFields> lineData, CancellableTask cancellable)
        {
            // CSVデータのパース

            // Text を質問文として使用
            string question = lineData.GetOrDefault<string>(ScenarioFields.Text, "");

            // Arg1 ～ Arg6 を選択肢ラベルとして収集
            var choices = new List<string>();
            var argFields = new[]
            {
                ScenarioFields.Arg1,
                ScenarioFields.Arg2,
                ScenarioFields.Arg3,
                ScenarioFields.Arg4,
                ScenarioFields.Arg5,
                ScenarioFields.Arg6
            };

            foreach (var field in argFields)
            {
                string label = lineData.GetOrDefault<string>(field, "");
                if (!string.IsNullOrWhiteSpace(label))
                {
                    choices.Add(label.Trim());
                }
            }

            if (choices.Count == 0)
            {
                Debug.LogError("[ChoiceCommand] No choices found. Check Arg1-Arg6 fields.");
                return;
            }

            // 選択肢表示・入力待機

            string selected = await _choicePresenter.ShowChoiceAsync(question, choices, cancellable);

            // キャンセルされた場合は何もしない
            if (string.IsNullOrEmpty(selected)) return;

            // 遷移先 CSV の解決

            var executor = AdvScenarioExecutor.Instance;
            if (executor == null)
            {
                Debug.LogError("[ChoiceCommand] AdvScenarioExecutor.Instance is null.");
                return;
            }

            string currentName = executor.CurrentScenarioName;
            if (string.IsNullOrEmpty(currentName))
            {
                Debug.LogError("[ChoiceCommand] CurrentScenarioName is null. Has the executor loaded a scenario?");
                return;
            }

            // Resources/Datas/{currentName}_{selected} 
            string resourcePath = $"Datas/{currentName}_{selected}";
            var newCsv = Resources.Load<TextAsset>(resourcePath);

            if (newCsv == null)
            {
                Debug.LogError($"[ChoiceCommand] CSV not found: Resources/{resourcePath}");
                return;
            }

            // シナリオ分岐
            // LoadAndExecuteScenario は IsExecuting=true を検知して
            // StopScenario → 現在ループをキャンセル → 新シナリオ開始 の順に処理する
            executor.LoadAndExecuteScenario(newCsv).Forget();
        }
    }
}
