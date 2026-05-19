using ADV.System;
using SceneManagement;
using UnityEngine;

namespace ADV.System
{
    public class AdvSceneData : IAdvSceneData
    {
        public SceneId TargetSceneId { get; private set; }
        public TextAsset DataFile { get; private set; }
        public AdvSceneData(SceneId targetSceneId, TextAsset dataFile)
        {
            TargetSceneId = targetSceneId;
            DataFile = dataFile;
        }
    }
}