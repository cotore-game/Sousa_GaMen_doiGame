using Unity.VisualScripting;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    [SerializeField, Header("GameOverUI")]
    private GameObject gameOverUI;

    private GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<PlayerMover>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        ShowGameOverUI();
    }

    private void ShowGameOverUI()
    {
        if (player != null) return;

        gameOverUI.SetActive(true);
    }
}
