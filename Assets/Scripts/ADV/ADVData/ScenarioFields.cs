using CSV4Unity;
using CSV4Unity.Validation;

namespace ADV.Data
{
    [CsvSchema]
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
