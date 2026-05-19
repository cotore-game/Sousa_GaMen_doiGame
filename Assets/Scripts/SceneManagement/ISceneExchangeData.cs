namespace SceneManagement
{
    /// <summary>
    /// シーン間でやり取りするデータクラスが実装するインターフェース。
    /// </summary>
    /// <remarks>
    /// 使用例:
    /// <code>
    /// public class GamePlayData : ISceneExchangeData
    /// {
    ///     public string PlayerName { get; set; }
    ///     public int Level { get; set; }
    /// }
    /// </code>
    /// </remarks>
    public interface ISceneExchangeData
    {
        /// <summary>
        /// この遷移データが対応するシーンIDを取得します。
        /// </summary>
        SceneId TargetSceneId { get; }
    }
}
