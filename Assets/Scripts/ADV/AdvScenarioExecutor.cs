using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;
using ADV.Commands;
using ADV.Core;
using ADV.Presentation;
using CSV4Unity;
using CSV4Unity.Fields;
using SceneManagement;

namespace ADV.System
{
    /// <summary>
    /// ADVシナリオ実行を統括するメインエンジン
    /// 責務: CSV読み込み、コマンド実行制御、非同期タスク管理、Presenter生成
    /// </summary>
    public class AdvScenarioExecutor : MonoBehaviour
    {
        [Header("View Prefabs")]
        [SerializeField] private TextDisplayView textViewPrefab;
        [SerializeField] private CharacterView characterViewPrefab;
        [SerializeField] private Transform characterContainer;
        [SerializeField] private ChoiceView choiceViewPrefab;
        [SerializeField] private BackgroundView backgroundViewPrefab;

        [Header("Debug Settings")]
        [SerializeField] private bool enableDebugLog = true;
        [SerializeField] private TextAsset defaultScenarioData = null;

        // プレゼンター層
        private TextPresenter _textPresenter;
        private CharacterPresenter _characterPresenter;
        private ChoicePresenter _choicePresenter;
        private BackgroundPresenter _backgroundPresenter;

        // 実行状態
        private CsvData<ScenarioFields> _currentScenario;
        private int _currentLineIndex;
        private CancellableTask _scenarioCancellable;
        private string _currentScenarioName;

        // 非同期演出タスク管理
        private readonly List<UniTask> _activeVisualTasks = new();

        // コマンドファクトリー
        private CommandFactory _commandFactory;

        // 実行状態プロパティ
        public bool IsExecuting { get; private set; }
        public bool IsPaused { get; private set; }
        public int TotalLines => _currentScenario?.RowCount ?? 0;
        public int CurrentLine => _currentLineIndex;
        public float Progress => TotalLines > 0 ? (float)_currentLineIndex / TotalLines : 0f;
        public string CurrentScenarioName => _currentScenarioName;

        // シングルトンアクセス
        private static AdvScenarioExecutor _instance;
        public static AdvScenarioExecutor Instance => _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;

            InitializePresenters();
        }

        /// <summary>
        /// Presenter層の初期化とファクトリーへの依存関係注入
        /// </summary>
        private void InitializePresenters()
        {
            // View層のインスタンス化
            var textView = Instantiate(textViewPrefab, transform);
            var backgroundView = backgroundViewPrefab != null ? Instantiate(backgroundViewPrefab, transform) : null;

            // Presenter層の生成
            _textPresenter = new TextPresenter(textView);
            _characterPresenter = new CharacterPresenter(characterContainer, characterViewPrefab);
            _choicePresenter = new ChoicePresenter(choiceViewPrefab, _textPresenter);
            
            if (backgroundView != null)
                _backgroundPresenter = new BackgroundPresenter(backgroundView);

            // コマンドが必要とする依存関係をまとめる
            var dependencies = new CommandDependencies(
                _textPresenter,
                _characterPresenter,
                SceneTransitioner.Instance,
                _choicePresenter,
                _backgroundPresenter
            );

            // ファクトリー生成
            _commandFactory = new CommandFactory(dependencies);

            DebugLog("Presenters initialized and dependencies injected to factory");
        }

        private void Start()
        {
            // シーン遷移データからCSVを取得して実行開始
            InitializeFromSceneData().Forget();
        }

        public class TmpSceneData : IAdvSceneData
        {
            public TextAsset DataFile { get; set; }

            public SceneId TargetSceneId { get; set; }
        }
        /// <summary>
        /// 
        /// シーン遷移データからシナリオを初期化して実行
        /// </summary>
        private async UniTaskVoid InitializeFromSceneData()
        {
            try
            {
                // GameFlow用のデータ取得
                var sceneData = SceneExchangeManager.Instance?.GetData<IAdvSceneData>();

                // シーン遷移データがない、またはCSVが設定されていない場合
                if (sceneData == null || sceneData.DataFile == null)
                {
                    Debug.LogWarning("[AdvScenarioExecutor] CSV data not found in scene exchange data");

                    if (defaultScenarioData != null)
                    {
                        Debug.LogWarning("Using default scenario data");
                        await LoadAndExecuteScenario(defaultScenarioData);
                    }
                    else
                    {
                        Debug.LogError("[AdvScenarioExecutor] No scenario data available");
                    }
                    return;
                }

                // シーン遷移データからCSVを取得して実行
                await LoadAndExecuteScenario(sceneData.DataFile);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AdvScenarioExecutor] Failed to initialize: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// シナリオをロードして実行開始
        /// </summary>
        public async UniTask LoadAndExecuteScenario(TextAsset csvFile)
        {
            if (IsExecuting)
            {
                Debug.LogWarning("[AdvScenarioExecutor] Already executing scenario. Stopping current.");
                await StopScenario();
            }

            try
            {
                // CSVロード
                var options = new CsvLoaderOptions
                {
                    HasHeader = true,
                    TrimFields = true,
                    IgnoreEmptyLines = true,
                    CommentPrefix = "#",
                    ValidationEnabled = true,
                    ThrowOnValidationError = false
                };

                _currentScenarioName = csvFile.name;
                _currentScenario = CSVLoader.LoadCSV<ScenarioFields>(csvFile, options);
                _currentLineIndex = 0;

                DebugLog($"Loaded scenario: {csvFile.name} ({_currentScenario.RowCount} lines)");

                // シナリオ実行開始
                _scenarioCancellable = new CancellableTask();
                IsExecuting = true;

                await ExecuteScenarioLoop();
            }
            catch (OperationCanceledException)
            {
                DebugLog("Scenario execution cancelled");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AdvScenarioExecutor] Scenario execution failed: {ex}");
            }
            finally
            {
                await CleanupScenario();
            }
        }

        /// <summary>
        /// メインシナリオ実行ループ
        /// </summary>
        private async UniTask ExecuteScenarioLoop()
        {
            while (_currentLineIndex < _currentScenario.RowCount)
            {
                if (_scenarioCancellable.IsCancellationRequested) break;

                // ポーズ中は待機
                await UniTask.WaitUntil(() => !IsPaused);

                var lineData = _currentScenario.Rows[_currentLineIndex];
                var commandName = lineData.Get<string>(ScenarioFields.Command);

                // コマンドインスタンスを取得
                var command = _commandFactory.CreateCommandInstance(commandName);
                if (command == null)
                {
                    Debug.LogWarning($"[AdvScenarioExecutor] Unknown command: {commandName} at line {_currentLineIndex + 1}. Skipping.");
                    _currentLineIndex++;
                    continue;
                }

                try
                {
                    // バッチ処理に対応しているか確認
                    if (command.CanBatchProcess)
                    {
                        // 連続する同じコマンドを収集
                        var batchLineData = CollectBatchCommands(_currentLineIndex, commandName);

                        DebugLog($"[{_currentLineIndex + 1}-{_currentLineIndex + batchLineData.Count}] Batch executing {batchLineData.Count} '{commandName}' commands");

                        // バッチ実行とawait判断をコマンド自身に委譲
                        bool shouldAwait = command.ShouldBatchAwait(batchLineData);
                        var task = command.ExecuteBatchAsync(batchLineData, _scenarioCancellable);

                        if (shouldAwait)
                        {
                            // 待機型コマンドと同様に、先に全演出完了を待つ
                            await WaitForAllVisualTasks();
                            // バッチの完了を待つ
                            await task;
                        }
                        else
                        {
                            // 非待機タスクとして登録
                            _activeVisualTasks.Add(task);
                        }

                        // バッチ処理した行数分、インデックスを進める
                        _currentLineIndex += batchLineData.Count;
                    }
                    else
                    {
                        // 通常の単一行コマンド実行 
                        DebugLog($"[{_currentLineIndex + 1}/{TotalLines}] Executing: {commandName}");

                        // 従来の ExecuteCommand ロジック
                        await ExecuteCommand(command, lineData);

                        _currentLineIndex++;
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[AdvScenarioExecutor] Command '{commandName}' failed at line {_currentLineIndex + 1}: {ex.Message}\n{ex.StackTrace}");
                    // エラーが発生しても次の行に進む
                    _currentLineIndex++;
                }
            }

            DebugLog("Scenario execution completed");
        }

        /// <summary>
        /// 連続する同じコマンドの行データを収集
        /// </summary>
        private List<LineData<ScenarioFields>> CollectBatchCommands(int startIndex, string batchCommandName)
        {
            var batch = new List<LineData<ScenarioFields>>();
            int scanIndex = startIndex;

            while (scanIndex < _currentScenario.RowCount)
            {
                var lineData = _currentScenario.Rows[scanIndex];
                var commandName = lineData.Get<string>(ScenarioFields.Command);

                // 指定されたコマンド名と一致するか
                if (commandName.Equals(batchCommandName, StringComparison.OrdinalIgnoreCase))
                {
                    batch.Add(lineData);
                    scanIndex++;
                }
                else
                {
                    break;
                }
            }

            return batch;
        }


        /// <summary>
        /// コマンド実行の中核ロジック
        /// </summary>
        private async UniTask ExecuteCommand(CommandBase command, LineData<ScenarioFields> lineData)
        {
            // コマンド検証
            if (!command.Validate(lineData, out var errorMsg))
            {
                Debug.LogError($"[AdvScenarioExecutor] Validation failed: {errorMsg}");
                return;
            }

            // 待機が必要なコマンドの場合、先に全演出完了を待つ
            if (command.ShouldEngineAwait)
            {
                await WaitForAllVisualTasks();

                // 待機型コマンドを実行
                await command.ExecuteAsync(lineData, _scenarioCancellable);
            }
            else
            {
                // タスクをリストに追加して即座に次へ
                var task = command.ExecuteAsync(lineData, _scenarioCancellable);
                _activeVisualTasks.Add(task);
            }
        }

        /// <summary>
        /// 全ての実行中演出タスクの完了を待機
        /// </summary>
        private async UniTask WaitForAllVisualTasks()
        {
            if (_activeVisualTasks.Count == 0) return;

            try
            {
                await UniTask.WhenAll(_activeVisualTasks).AttachCancellation(_scenarioCancellable);
            }
            catch (OperationCanceledException)
            {
                // キャンセルは正常
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AdvScenarioExecutor] Visual task error: {ex.Message}");
            }
            finally
            {
                _activeVisualTasks.Clear();
            }
        }

        /// <summary>
        /// シナリオ実行を停止
        /// </summary>
        public async UniTask StopScenario()
        {
            if (!IsExecuting) return;

            DebugLog("Stopping scenario execution");

            _scenarioCancellable?.Cancel();
            await CleanupScenario();
        }

        /// <summary>
        /// リソースクリーンアップ
        /// </summary>
        private async UniTask CleanupScenario()
        {
            IsExecuting = false;
            _activeVisualTasks.Clear();

            // キャッシュクリア
            _characterPresenter?.ClearCache();

            await UniTask.Yield();
        }

        /// <summary>
        /// ポーズ切り替え
        /// </summary>
        public void TogglePause()
        {
            IsPaused = !IsPaused;
            DebugLog($"Scenario {(IsPaused ? "paused" : "resumed")}");
        }

        /// <summary>
        /// 指定行へジャンプ
        /// </summary>
        public void JumpToLine(int lineIndex)
        {
            if (lineIndex < 0 || lineIndex >= TotalLines)
            {
                Debug.LogWarning($"[AdvScenarioExecutor] Invalid line index: {lineIndex}");
                return;
            }

            _currentLineIndex = lineIndex;
            DebugLog($"Jumped to line {lineIndex}");
        }

        private void DebugLog(string message)
        {
            if (enableDebugLog)
            {
                Debug.Log($"<color=cyan>[AdvScenarioExecutor]</color> {message}");
            }
        }

        private void OnDestroy()
        {
            _scenarioCancellable?.Dispose();
            _textPresenter?.Dispose();
            _characterPresenter?.Dispose();
            _choicePresenter?.Dispose();
            _backgroundPresenter?.Dispose();

            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}
