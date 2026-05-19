using CSV4Unity;
using CSV4Unity.Fields;
using Cysharp.Threading.Tasks;
using ADV.Core;
using System.Collections.Generic;
using UnityEngine;

namespace ADV.Commands
{
    /// <summary>
    /// シナリオコマンドの基底抽象クラス
    /// </summary>
    public abstract class CommandBase
    {
        /// <summary>
        /// 依存関係コンテナ
        /// </summary>
        protected AdvDependencyContainer Dependencies { get; private set; }

        /// <summary>
        /// エンジンがこのコマンドの完了を待機するかどうか
        /// false: 非同期で即座に次のコマンドへ進む（演出系）
        /// true: 完了を待機してから次のコマンドへ進む（テキスト、待機系）
        /// </summary>
        public virtual bool ShouldEngineAwait => true;

        /// <summary>
        /// このコマンドが連続する場合にバッチ処理（まとめて実行）をサポートするか
        /// true の場合、Executorは ExecuteBatchAsync を呼び出す
        /// false の場合、Executorは ExecuteAsync を1行ずつ呼び出す
        /// </summary>
        public virtual bool CanBatchProcess => false;

        /// <summary>
        /// コマンドを実行します (バッチ処理)
        /// CanBatchProcess が true の場合に Executor から呼び出される
        /// </summary>
        public virtual UniTask ExecuteBatchAsync(List<LineData<ScenarioFields>> batchLineData, CancellableTask cancellable)
        {
            // デフォルト実装 (CanBatchProcessをtrueにしたのにオーバーライドしなかった場合)
            Debug.LogError($"[{GetType().Name}] CanBatchProcess is true, but ExecuteBatchAsync is not implemented.");
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// バッチ処理の完了を待機(await)すべきか判断する
        /// CanBatchProcess が true の場合に Executor から呼び出される
        /// </summary>
        public virtual bool ShouldBatchAwait(List<LineData<ScenarioFields>> batchLineData)
        {
            // デフォルトは待機する
            return true;
        }

        /// <summary>
        /// 依存関係を注入
        /// </summary>
        public void InjectDependencies(AdvDependencyContainer container)
        {
            Dependencies = container;
        }

        /// <summary>
        /// コマンドを実行します
        /// </summary>
        /// <param name="lineData">CSVの行データ</param>
        /// <param name="cancellable">キャンセル可能タスク</param>
        public abstract UniTask ExecuteAsync(LineData<ScenarioFields> lineData, CancellableTask cancellable);

        /// <summary>
        /// コマンドの実行前検証
        /// </summary>
        public virtual bool Validate(LineData<ScenarioFields> lineData, out string errorMessage)
        {
            errorMessage = null;
            return true;
        }

        /// <summary>
        /// 依存サービスを安全に取得
        /// </summary>
        protected T GetService<T>() where T : class
        {
            if (Dependencies == null)
            {
                UnityEngine.Debug.LogError($"[{GetType().Name}] Dependencies not injected");
                return null;
            }

            return Dependencies.Resolve<T>();
        }

        /// <summary>
        /// 依存サービスの取得を試行
        /// </summary>
        protected bool TryGetService<T>(out T service) where T : class
        {
            service = null;
            if (Dependencies == null) return false;
            return Dependencies.TryResolve(out service);
        }
    }
}
