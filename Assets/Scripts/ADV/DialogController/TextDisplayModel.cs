namespace ADV.Presentation
{
    /// <summary>
    /// テキスト表示の状態を保持するモデル
    /// </summary>
    public class TextDisplayModel
    {
        public string CharacterName { get; set; }
        public string BodyText { get; set; }
        public int VisibleCharacterCount { get; set; }
        public int TotalCharacterCount { get; set; }
        public bool IsSkippable { get; set; }
        public bool IsComplete { get; set; }

        public float Progress => TotalCharacterCount > 0
            ? (float)VisibleCharacterCount / TotalCharacterCount
            : 1f;
    }
}
