using UnityEngine;
using Cysharp.Threading.Tasks;
using ADV.Core;
using System;

namespace ADV.Presentation
{
    /// <summary>
    /// テキスト表示のプレゼンター
    /// </summary>
    public class TextPresenter : IDisposable
    {
        private readonly TextDisplayView _view;
        private readonly TextDisplayModel _model;
        private CancellableTask _displayTask;

        public TextPresenter(TextDisplayView view)
        {
            _view = view;
            _model = new TextDisplayModel();
        }

        /// <summary>
        /// テキストを1文字ずつ表示
        /// </summary>
        public async UniTask DisplayTextAsync(
            string characterName,
            string bodyText,
            int intervalMs = 50,
            int skipThresholdPercent = 30,
            CancellableTask cancellable = null,
            bool requireInputAtEnd = true)
        {
            // モデル初期化
            _model.CharacterName = characterName;
            _model.BodyText = bodyText;
            _model.VisibleCharacterCount = 0;
            _model.IsComplete = false;

            // View更新
            _view.SetActive(true);
            _view.SetCharacterName(characterName);
            _view.SetBodyText(bodyText);
            _view.SetVisibleCharacters(0);
            _view.SetSkipIconActive(false);

            _model.TotalCharacterCount = _view.GetTotalCharacterCount();
            int skipThreshold = Mathf.CeilToInt(_model.TotalCharacterCount * (skipThresholdPercent / 100f));

            // 1文字ずつ表示
            while (_model.VisibleCharacterCount < _model.TotalCharacterCount)
            {
                // キャンセルチェック
                if (cancellable?.IsCancellationRequested ?? false)
                {
                    _view.SetVisibleCharacters(_model.TotalCharacterCount);
                    break;
                }

                // スキップ可能になったか判定
                _model.IsSkippable = _model.VisibleCharacterCount >= skipThreshold;

                if (_model.IsSkippable)
                {
                    _view.SetSkipIconActive(true);

                    // 入力とディレイを競争
                    var delayTask = UniTask.Delay(intervalMs);
                    var inputTask = UniTask.WaitUntil(() => IsSkipInput());

                    var winner = await UniTask.WhenAny(delayTask, inputTask);

                    if (winner == 1) // 入力が先
                    {
                        _view.SetVisibleCharacters(_model.TotalCharacterCount);
                        break;
                    }
                }
                else
                {
                    await UniTask.Delay(intervalMs);
                }

                _model.VisibleCharacterCount++;
                _view.SetVisibleCharacters(_model.VisibleCharacterCount);
            }

            _model.IsComplete = true;
            
            if (requireInputAtEnd)
            {
                _view.SetSkipIconActive(true);
                // 最終的な入力待機
                await UniTask.WaitUntil(() => IsSkipInput());
            }
            _view.SetSkipIconActive(false);
        }

        /// <summary>
        /// スキップ入力判定
        /// </summary>
        private bool IsSkipInput()
        {
            if (_view.ShouldIgnoreInput()) return false;

            bool isShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            return isShift && Input.GetKeyDown(KeyCode.Period);
        }

        /// <summary>
        /// ウィンドウを非表示
        /// </summary>
        public void Hide()
        {
            _view.SetActive(false);
        }

        public void Dispose()
        {
            _displayTask?.Dispose();
        }
    }
}
