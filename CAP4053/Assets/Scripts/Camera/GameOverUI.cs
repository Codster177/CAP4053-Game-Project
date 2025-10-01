using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public void RestartGame()
    {
        Time.timeScale = 1f; // unpause in case the game was paused
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
