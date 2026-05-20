using ADV.Presentation;
using SceneManagement;
using System;

namespace ADV.Commands
{
    /// <summary>
    /// コマンドが必要とする依存関係をまとめたデータクラス
    /// 各コマンドは必要なものだけを受け取る
    /// </summary>
    public class CommandDependencies
    {
        public TextPresenter TextPresenter { get; }
        public CharacterPresenter CharacterPresenter { get; }
        public SceneTransitioner SceneTransitioner { get; }
        public ChoicePresenter ChoicePresenter { get; }

        public CommandDependencies(
            TextPresenter textPresenter,
            CharacterPresenter characterPresenter,
            SceneTransitioner sceneTransitioner,
            ChoicePresenter choicePresenter)
        {
            TextPresenter = textPresenter ?? throw new ArgumentNullException(nameof(textPresenter));
            CharacterPresenter = characterPresenter ?? throw new ArgumentNullException(nameof(characterPresenter));
            SceneTransitioner = sceneTransitioner ?? throw new ArgumentNullException(nameof(sceneTransitioner));
            ChoicePresenter = choicePresenter ?? throw new ArgumentNullException(nameof(choicePresenter));
        }
    }
}
