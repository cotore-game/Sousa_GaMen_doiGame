using Cysharp.Threading.Tasks;
using UnityEngine;
using ADV.Core;
using ADV.Presentation;
using CSV4Unity;
using CSV4Unity.Fields;
using System;
using System.Collections.Generic;

namespace ADV.Commands
{
    /// <summary>
    /// キャラクター表示コマンド
    /// Arg1: キャラ名, Arg2: 表情, Arg3: フェード時間
    /// 連続する複数のCharacterコマンドは同時実行される（バッチ処理）
    /// </summary>
    public class CharacterCommand : CommandBase
    {
        private readonly CharacterPresenter _characterPresenter;

        public CharacterCommand(CharacterPresenter characterPresenter)
        {
            _characterPresenter = characterPresenter ?? throw new ArgumentNullException(nameof(characterPresenter)); ;
        }

        // 非待機型コマンド（演出系）
        public override bool ShouldEngineAwait => false;

        /// <summary>
        /// このコマンドはバッチ処理をサポートする
        /// </summary>
        public override bool CanBatchProcess => true;

        /// <summary>
        /// 単一行実行
        /// </summary>
        public override async UniTask ExecuteAsync(LineData<ScenarioFields> lineData, CancellableTask cancellable)
        {
            await UniTask.Yield();
            Debug.LogWarning("[CharacterCommand] Should be executed via batch processing in AdvScenarioExecutor");
        }

        /// <summary>
        /// バッチ処理の実行ロジック
        /// </summary>
        public override async UniTask ExecuteBatchAsync(List<LineData<ScenarioFields>> batchLineData, CancellableTask cancellable)
        {
            if (_characterPresenter == null)
            {
                Debug.LogError("[CharacterCommand] CharacterPresenter is null");
                return;
            }

            // 全キャラクターデータを収集
            var allCharacters = new List<CharacterDisplayData>();
            foreach (var lineData in batchLineData)
            {
                var charInfo = ParseCharacterCommandData(lineData);
                if (charInfo != null)
                {
                    allCharacters.Add(charInfo);
                }
            }

            if (allCharacters.Count > 0)
            {
                // Presenterにバッチ実行を依頼
                await _characterPresenter.ShowCharacters(allCharacters, cancellable);
            }
            else
            {
                // データが空でも、以前のキャラをクリアするなどのために空リストで呼ぶ
                await _characterPresenter.ShowCharacters(new List<CharacterDisplayData>(), cancellable);
            }
        }

        /// <summary>
        /// バッチ全体を待機すべきか判断する
        /// </summary>
        public override bool ShouldBatchAwait(List<LineData<ScenarioFields>> batchLineData)
        {
            // デフォルトは待機
            // バッチ内に一つでもfalseがあれば false になる
            foreach (var lineData in batchLineData)
            {
                string waitFlag = lineData.GetOrDefault<string>(ScenarioFields.WaitType, "true");
                if (waitFlag.Equals("nowait", StringComparison.OrdinalIgnoreCase))
                {
                    return false; // 待機しない
                }
            }
            return true; // バッチ全体を待機する
        }

        /// <summary>
        /// Characterコマンドからキャラクターデータをパース
        /// </summary>
        private CharacterDisplayData ParseCharacterCommandData(LineData<ScenarioFields> lineData)
        {
            string name = lineData.GetOrDefault<string>(ScenarioFields.Arg1, null);
            string expression = lineData.GetOrDefault<string>(ScenarioFields.Arg2, null);
            float fade = lineData.GetOrDefault<float>(ScenarioFields.Arg3, 0.5f);

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(expression))
            {
                return null;
            }

            return new CharacterDisplayData
            {
                Name = name.Trim(),
                Expression = expression.Trim(),
                FadeTime = fade
            };
        }
    }
}
