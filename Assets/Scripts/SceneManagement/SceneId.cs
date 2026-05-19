namespace SceneManagement
{
    /// <summary>
    /// シーンを識別するための列挙型。
    /// 各シーンに一意のIDを割り当てます。
    /// </summary>
    /// <remarks>
    /// SceneIdとシーン名は1:1対応である必要があります。
    /// ただし、同じシーンに対して複数の遷移コンテキストを持つことは可能です。
    /// </remarks>
    public enum SceneId
    {
        Title,
        ADV,
        Stage1,
        Stage2
    }
}
