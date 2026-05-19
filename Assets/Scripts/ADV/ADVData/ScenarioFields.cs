using CSV4Unity.Validation;

namespace CSV4Unity.Fields
{
    public enum ScenarioFields
    {
        Command,
        Arg1,
        Arg2,
        Arg3,
        Arg4,
        Arg5,
        Arg6,
        WaitType,
        [MaxLength(160)]
        [Regex(@"^(?!.*(<br>.*){5}).*$")]
        Text,
        PageCtrl,
        Voice,
        WindowType,
        English
    }
}
