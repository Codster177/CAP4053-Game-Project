using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class DeathScreenController : MonoBehaviour
{
    [Header("Assign in Inspector")]
    [SerializeField] private GameObject deathScreen;       // Drag your DeathScreen Panel here
    [SerializeField] private Selectable firstSelected;     // (Optional) Button to auto-select for keyboard/controller

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.Death)
        {
            // Show the UI and pause gameplay
            deathScreen.SetActive(true);
            Time.timeScale = 0f;

            // Optional: focus the Restart button so Enter/Space works immediately
            if (firstSelected != null && EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
            }
        }
        else
        {
            // Hide if we ever return to gameplay
            deathScreen.SetActive(false);
        }
    }
}
