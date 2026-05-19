using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    [SerializeField, Header("GameOverUI")]
    private GameObject gameOverUI;

    private GameObject player;
    private bool bShowUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<PlayerMover>().gameObject;
        bShowUI = false;
    }

    // Update is called once per frame
    void Update()
    {
        ShowGameOverUI();
        bShowUI = true;
    }

    private void ShowGameOverUI()
    {
        if (player != null) return;

        gameOverUI.SetActive(true);
    }

    public void OnRestart(InputAction.CallbackContext context)
    {
        if (! bShowUI || !context.performed)return;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
}