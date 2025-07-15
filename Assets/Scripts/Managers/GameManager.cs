using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private bool _isGameOver = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver)
        {
            _isGameOver = false;
            StartGame();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
        }
    }

    public void GameFinished()
    {
        _isGameOver = true;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void GameOver()
    {
        _isGameOver = true;
    }

    // GETTERS
    public bool IsGameOver()
    {
        return _isGameOver;
    }
}
