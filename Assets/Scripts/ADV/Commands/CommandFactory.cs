using UnityEngine;
using System.Collections.Generic;
using System;
using ADV.Presentation;
using SceneManagement;

namespace ADV.Commands
{
    /// <summary>
    /// コマンドインスタンスを生成し、必要な依存関係のみを注入するファクトリークラス
    /// コマンド名からインスタンスを生成、個別DI、登録管理
    /// </summary>
    public class CommandFactory
    {
        // コマンド登録用デリゲート
        private readonly Dictionary<string, Func<CommandDependencies, CommandBase>> _commandRegistry;

        // 各コマンドが必要とする依存関係
        private readonly CommandDependencies _dependencies;

        public CommandFactory(CommandDependencies dependencies)
        {
            _dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
            _commandRegistry = new Dictionary<string, Func<CommandDependencies, CommandBase>>(StringComparer.OrdinalIgnoreCase);

            RegisterDefaultCommands();
        }

        /// <summary>
        /// デフォルトコマンドの登録
        /// </summary>
        private void RegisterDefaultCommands()
        {
            // テキスト
            Register("Text", deps => new TextCommand(deps.TextPresenter));

            
            // キャラクター表示系
            Register("Character", deps => new CharacterCommand(deps.CharacterPresenter));

            Register("HideCharacter", deps => new HideCharacterCommand(deps.CharacterPresenter));

            // シーン遷移系
            Register("LoadScene", deps => new LoadSceneCommand(deps.SceneTransitioner));

            Register("End", deps => new EndCommand());


            // 選択肢系
            Register("Choice", deps => new ChoiceCommand(deps.ChoicePresenter));

            // 背景・演出系
            Register("Bg", deps => new BgCommand(deps.BackgroundPresenter));

            /*
            // その他の背景・演出系（今後実装）
            // Register("Bg", deps => new BgCommand());
            Register("Day", deps => new DayCommand());
            Register("DayOff", deps => new DayOffCommand());

            // 音声系（今後実装）
            Register("Bgm", deps => new BgmCommand());
            Register("Se", deps => new SeCommand());
            Register("StopBgm", deps => new StopBgmCommand());
            Register("PauseBgm", deps => new PauseBgmCommand());
            Register("SetVol", deps => new SetVolCommand());

            // 制御系（依存なし）
            Register("Wait", deps => new WaitCommand());
            Register("Goto", deps => new GotoCommand());
            Register("If", deps => new IfCommand());
            Register("Flag", deps => new FlagCommand());
            Register("Param", deps => new ParamCommand());

            // ウィンドウ制御（TextPresenterが必要）
            Register("HideWindow", deps => new HideWindowCommand(deps.TextPresenter));
            Register("ShowWindow", deps => new ShowWindowCommand(deps.TextPresenter));

            // 特殊制御
            Register("Await", deps => new AwaitCommand());
            Register("Button", deps => new ButtonCommand());
            */
        }

        /// <summary>
        /// カスタムコマンドを登録
        /// </summary>
        public void Register(string commandName, Func<CommandDependencies, CommandBase> factory)
        {
            if (string.IsNullOrWhiteSpace(commandName))
            {
                Debug.LogWarning("[CommandFactory] Cannot register command with empty name");
                return;
            }

            if (_commandRegistry.ContainsKey(commandName))
            {
                Debug.LogWarning($"[CommandFactory] Command '{commandName}' is already registered. Overwriting.");
            }

            _commandRegistry[commandName] = factory;
        }

        /// <summary>
        /// コマンドインスタンスを生成（必要な依存関係のみ注入）
        /// </summary>
        public CommandBase CreateCommandInstance(string commandName)
        {
            if (string.IsNullOrWhiteSpace(commandName))
            {
                commandName = ""; // 空欄はデフォルトコマンド
            }

            if (_commandRegistry.TryGetValue(commandName, out var factory))
            {
                return factory(_dependencies);
            }

            Debug.LogError($"[CommandFactory] Command '{commandName}' not found");
            return null;
        }

        /// <summary>
        /// 登録されているコマンド一覧を取得
        /// </summary>
        public IReadOnlyCollection<string> GetRegisteredCommands()
        {
            return _commandRegistry.Keys;
        }
    }
}
