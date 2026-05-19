using SceneManagement;
using UnityEngine;

namespace ADV.System
{
    /// <summary>
    /// ADVシーン遷移データ用インターフェース
    /// </summary>
    public interface IAdvSceneData : ISceneExchangeData
    {
        TextAsset DataFile { get; }
    }
}
