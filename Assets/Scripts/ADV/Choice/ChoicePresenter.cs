using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using ADV.Core;

namespace ADV.Presentation
{
    /// <summary>
    /// 選択肢表示のPresenter層
    /// View への参照のみ持ち、ゲームロジックを持たない
    /// </summary>
    public class ChoicePresenter : IDisposable
    {
        private readonly ChoiceView _view;
        private readonly TextPresenter _textPresenter;

        public ChoicePresenter(ChoiceView view, TextPresenter textPresenter)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _textPresenter = textPresenter ?? throw new ArgumentNullException(nameof(textPresenter));
            _view.SetActive(false);
        }

        /// <summary>
        /// 選択肢UIを表示し、プレイヤーの有効入力を待機して選択肢ラベルを返す。
        /// キャンセル時は null を返す。
        /// </summary>
        public async UniTask<string> ShowChoiceAsync(
            string question,
            List<string> choices,
            CancellableTask cancellable)
        {
            _view.ClearError();
            _view.SetActive(true);
            await _textPresenter.DisplayTextAsync("", BuildBodyText(question, choices), 50, 30, cancellable, false);

            string selected = await WaitForValidInputAsync(choices, cancellable);

            _view.SetActive(false);
            return selected;
        }

        /// <summary>
        /// 表示テキストを組み立てる。
        /// </summary>
        private static string BuildBodyText(string question, List<string> choices)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(question))
                sb.AppendLine(question);

            foreach (var choice in choices)
                sb.AppendLine($"・{choice}");

            return "<rotate=90>" + sb.ToString().TrimEnd() + "</rotate>";
        }

        /// <summary>
        /// 選択肢と完全一致する入力が来るまでループする。
        /// 不一致の場合はエラーを一定時間表示してリトライ。
        /// </summary>
        private async UniTask<string> WaitForValidInputAsync(
            List<string> choices,
            CancellableTask cancellable)
        {
            // プレイヤーが文字を打ち直した瞬間にエラーを消すようにする
            UnityAction<string> onValueChanged = _ => _view.ClearError();

            try
            {
                _view.InputField.onValueChanged.AddListener(onValueChanged);

                while (true)
                {
                    if (cancellable?.IsCancellationRequested ?? false)
                        return null;

                    // InputField の onSubmit を UniTask で待機
                    string raw = await WaitForSubmitAsync(cancellable);

                    if (raw == null) return null;   // キャンセル

                    string trimmed = raw.Trim();

                    if (choices.Contains(trimmed))
                        return trimmed;

                    // 不正入力: フィードバックを出すが、待機(Delay)せずにすぐ次の待機へ戻る
                    _view.ShowError($"「{trimmed}」は選択肢にありません");
                    _view.ActivateInput();
                }
            }
            finally
            {
                _view.InputField.onValueChanged.RemoveListener(onValueChanged);
            }
        }

        /// <summary>
        /// TMP_InputField.onSubmit を UniTaskCompletionSource でラップして1回分待機する。
        /// </summary>
        private UniTask<string> WaitForSubmitAsync(CancellableTask cancellable)
        {
            var tcs = new UniTaskCompletionSource<string>();

            UnityAction<string> handler = null;
            handler = text =>
            {
                _view.InputField.onSubmit.RemoveListener(handler);
                tcs.TrySetResult(text);
            };

            _view.InputField.onSubmit.AddListener(handler);
            _view.ActivateInput();

            // キャンセルが来た場合は null で解決
            if (cancellable != null)
            {
                if (cancellable.Token.CanBeCanceled)
                {
                    cancellable.Token.Register(() =>
                    {
                        _view.InputField.onSubmit.RemoveListener(handler);
                        tcs.TrySetResult(null);
                    });
                }
            }

            return tcs.Task;
        }

        public void Dispose() { }
    }
}
