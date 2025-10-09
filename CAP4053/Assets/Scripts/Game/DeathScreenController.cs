using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;


public class DeathScreenController : MonoBehaviour
{
    [Header("Assign in Inspector")]
    [SerializeField] private GameObject deathScreen;       // Drag your DeathScreen Panel here
    [SerializeField] private Selectable firstSelected;     // (Optional) Button to auto-select for keyboard/controller
    [SerializeField] private float fadeDuration = 1f;    // Fade time in seconds

    /*
    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    } */

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (deathScreen != null)
        {
            canvasGroup = deathScreen.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = deathScreen.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0f;
            deathScreen.SetActive(false);
        }
    }

    public void ShowDeathScreen()
    {
        Debug.Log("ShowDeathScreen() called!");
        deathScreen.SetActive(true);
        StartCoroutine(FadeInDeathScreen());
    }

    private IEnumerator FadeInDeathScreen()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime; // unaffected by Time.timeScale
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        // Pauses game once death screen is fully faded in
        Time.timeScale = 0f;

        // Optional: focus the Restart button so Enter/Space works immediately
        if (firstSelected != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
        }
    }
}
