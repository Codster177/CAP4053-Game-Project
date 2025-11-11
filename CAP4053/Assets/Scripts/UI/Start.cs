using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1f;
    private Image fadeImage;

    public void StartGame()
    {
        StartCoroutine(LoadIntroWithFade());
    }

    private IEnumerator LoadIntroWithFade()
    {
        CreateFadeOverlay();
        
        // Fade to black
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        
        // Load intro scene instead of directly to game
        SceneManager.LoadScene("Intro");
        
        // Fade back in (optional - or let intro scene handle its own fade in)
        StartCoroutine(FadeOutOverlay());
    }

    private IEnumerator FadeOutOverlay()
    {
        if (fadeImage != null)
        {
            float elapsedTime = 0f;
            Color startColor = fadeImage.color;
            
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                fadeImage.color = new Color(0, 0, 0, alpha);
                yield return null;
            }
            
            Destroy(fadeImage.gameObject);
        }
    }

    private void CreateFadeOverlay()
    {
        // Check if overlay already exists
        GameObject existingFade = GameObject.Find("FadeOverlay");
        if (existingFade != null)
        {
            Destroy(existingFade);
        }

        GameObject fadeObject = new GameObject("FadeOverlay");
        Canvas canvas = fadeObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999; 
        
        fadeImage = fadeObject.AddComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0);
        
        RectTransform rect = fadeImage.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        DontDestroyOnLoad(fadeObject);
    }
}