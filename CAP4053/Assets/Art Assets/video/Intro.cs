using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class IntroVideoManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Button skipButton;
    public TextMeshProUGUI scaryText; // Assign a TMP Text object for the red shaking text
    
    [Header("Settings")]
    public float textDuration = 5f;
    public float shakeIntensity = 3f;
    
    private Image fadeImage;
    private bool isVideoFinished = false;
    
    void Start()
    {
        // Set up video completion event
        videoPlayer.loopPointReached += OnVideoFinished;
        
        // Set up skip button
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipIntro);
            skipButton.gameObject.SetActive(true);
        }
        
        // Hide scary text initially
        if (scaryText != null)
        {
            scaryText.gameObject.SetActive(false);
        }
        
        // Clean up any existing fade overlay from menu
        GameObject existingFade = GameObject.Find("FadeOverlay");
        if (existingFade != null)
        {
            Destroy(existingFade);
        }
        
        // Fade in the video (from the menu fade)
        StartCoroutine(FadeInVideo());
        
        // Auto-play video when scene loads
        videoPlayer.Play();
    }
    
    private IEnumerator FadeInVideo()
    {
        CreateFadeOverlay();
        fadeImage.color = Color.black;
        
        float elapsedTime = 0f;
        float fadeDuration = 1f;
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        
        Destroy(fadeImage.gameObject);
    }
    
    void OnVideoFinished(VideoPlayer vp)
    {
        if (!isVideoFinished)
        {
            isVideoFinished = true;
            StartCoroutine(PlayScaryTransition());
        }
    }
    
    public void SkipIntro()
    {
        // Skip immediately to game scene (no transition)
        if (!isVideoFinished)
        {
            isVideoFinished = true;
            LoadGameScene();
        }
    }
    
    private IEnumerator PlayScaryTransition()
    {
        // Fade to black first
        yield return StartCoroutine(FadeToBlack());
        
        // Show and shake the scary text
        yield return StartCoroutine(ShowShakingText());
        
        // Then load the game scene
        LoadGameScene();
    }
    
    private IEnumerator FadeToBlack()
    {
        CreateFadeOverlay();
        fadeImage.color = new Color(0, 0, 0, 0);
        
        float elapsedTime = 0f;
        float fadeDuration = 1.5f; // Slower fade for dramatic effect
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        
        // Hide video and skip button
        if (skipButton != null) skipButton.gameObject.SetActive(false);
        // You might need to hide your video display here too
    }
    
    private IEnumerator ShowShakingText()
    {
        if (scaryText != null)
        {
            // Set up the text
            scaryText.gameObject.SetActive(true);
            scaryText.color = Color.red;
            scaryText.text = "WARNING\nDANGER AHEAD\nPROCEED WITH CAUTION"; // Change this text
            scaryText.alignment = TextAlignmentOptions.Center;
            
            // Get original position
            Vector3 originalPos = scaryText.transform.localPosition;
            
            float elapsedTime = 0f;
            
            while (elapsedTime < textDuration)
            {
                elapsedTime += Time.deltaTime;
                
                // Shake effect
                float shakeX = Random.Range(-1f, 1f) * shakeIntensity;
                float shakeY = Random.Range(-1f, 1f) * shakeIntensity;
                scaryText.transform.localPosition = originalPos + new Vector3(shakeX, shakeY, 0);
                
                // Optional: Pulsing effect
                float pulse = Mathf.PingPong(elapsedTime * 2f, 0.3f) + 0.7f;
                scaryText.color = new Color(1f, 0f, 0f, pulse);
                
                yield return null;
            }
            
            // Reset position
            scaryText.transform.localPosition = originalPos;
        }
        else
        {
            // If no text object, just wait
            yield return new WaitForSeconds(textDuration);
        }
    }
    
    void LoadGameScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
    
    private void CreateFadeOverlay()
    {
        GameObject existingFade = GameObject.Find("FadeOverlay");
        if (existingFade != null) return;

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