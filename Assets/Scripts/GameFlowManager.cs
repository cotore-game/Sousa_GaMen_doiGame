using UnityEngine;
using SceneManagement;
using ADV.System;

public class GameFlowManager : SingletonMonoBehaviour<GameFlowManager>
{
    private int _currentFlowStep = 0;

    void Start()
    {
        // 初期フロー開始
        StartGameFlow();
    }

    private void StartGameFlow()
    {
        _currentFlowStep = 0;
        // 初期状態はTitleシーン。この後Titleシーン側で GoToNextScene() が呼ばれて scene01 へ進む想定
    }

    public void GoToNextScene()
    {
        // Title画面での開始ボタンやEndコマンドなどで呼ばれた際の次のシーン・フロー制御
        _currentFlowStep++;

        switch (_currentFlowStep)
        {
            case 1:
                LoadAdvScene("Datas/scene01");
                break;
            case 2:
                LoadAdvScene("Datas/scene02");
                break;
            case 3:
                LoadAdvScene("Datas/scene03");
                break;
            default:
                // ADVシーン（scene03まで）終了後はフローをリセットしてTitleに戻る
                _currentFlowStep = 0;
                if (SceneTransitioner.Instance != null)
                {
                    SceneTransitioner.Instance.TransitionTo(SceneId.Title);
                }
                break;
        }
    }

    private void LoadAdvScene(string resourcePath)
    {
        // 指定されたパスのCSVをResourcesから読み込む
        TextAsset csv = Resources.Load<TextAsset>(resourcePath);
        
        // SceneExchangeManager にデータを渡してADVシーンに遷移
        if (SceneExchangeManager.Instance != null && csv != null)
        {
            SceneExchangeManager.Instance.StoreData<IAdvSceneData>(new AdvSceneData(SceneId.ADV, csv));
            SceneTransitioner.Instance.TransitionTo(SceneId.ADV);
        }
        else
        {
            Debug.LogWarning($"[GameFlowManager] Initialization failed. Missing manager or CSV at {resourcePath}.");
        }
    }
}

