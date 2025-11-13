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
    public TextMeshProUGUI scaryText;
    
    [Header("Settings")]
    public float textDuration = 5f;
    public float shakeIntensity = 3f;
    
    private Image fadeImage;
    private bool isVideoFinished = false;
    private bool fadeStarted = false;
    private bool inScaryTextPhase = false;
    
    void Start()
    {
        // Set up video completion event
        videoPlayer.loopPointReached += OnVideoFinished;
        
        // Set up skip button - make sure it always works
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
        
        // Fade in the video
        StartCoroutine(FadeInVideo());
        
        // Start the video
        videoPlayer.Play();
        
        // Start checking for fade timing - SIMPLIFIED APPROACH
        StartCoroutine(StartFadeBeforeEnd());
    }
    
    private IEnumerator StartFadeBeforeEnd()
    {
        // Wait a moment for video to start
        yield return new WaitForSeconds(0.5f);
        
        // If we can't get video length, use a fixed time approach
        float videoTime = 0f;
        float targetFadeTime = 3f; // Start fading 3 seconds before end
        
        // Estimate video length by waiting for it to end
        while (videoPlayer.isPlaying && !isVideoFinished)
        {
            videoTime += Time.deltaTime;
            
            // If we've been playing for a while and video is still going, 
            // start fade based on frame count instead
            if (videoPlayer.frameCount > 0 && videoPlayer.frameRate > 0)
            {
                double totalTime = videoPlayer.frameCount / videoPlayer.frameRate;
                double timeLeft = totalTime - videoPlayer.time;
                
                // Start fade when 3 seconds left
                if (timeLeft <= 3.0 && !fadeStarted)
                {
                    fadeStarted = true;
                    StartCoroutine(FadeToBlackOverTime(3.0f));
                    break;
                }
            }
            
            // Fallback: if we can't calculate, start fade after a reasonable time
            // Adjust this based on your video length
            else if (videoTime > 1f && !fadeStarted) // Example: if video is ~8 seconds, start fade at 5 seconds
            {
                fadeStarted = true;
                StartCoroutine(FadeToBlackOverTime(1.0f));
                break;
            }
            
            yield return null;
        }
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
        
        if (fadeImage != null)
        {
            Destroy(fadeImage.gameObject);
            fadeImage = null;
        }
    }
    
    void OnVideoFinished(VideoPlayer vp)
    {
        if (!isVideoFinished)
        {
            isVideoFinished = true;
            Debug.Log("Video finished naturally");
            
            // If we're already in scary text phase, do nothing
            if (inScaryTextPhase) return;
            
            // If fade didn't complete, complete it quickly
            if (!fadeStarted)
            {
                fadeStarted = true;
                StartCoroutine(CompleteTransitionQuickly());
            }
            else
            {
                // Fade is in progress or completed, proceed to scary text
                StartCoroutine(PlayScaryTransition());
            }
        }
    }
    
    // SKIP BUTTON NOW WORKS IN ALL PHASES
    public void SkipIntro()
    {
        if (!isVideoFinished || inScaryTextPhase)
        {
            Debug.Log("Skip button pressed");
            isVideoFinished = true;
            inScaryTextPhase = false;
            fadeStarted = true;
            
            // Stop all coroutines
            StopAllCoroutines();
            
            // Stop the video
            if (videoPlayer != null && videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
            }
            
            // Hide scary text if it's showing
            if (scaryText != null)
            {
                scaryText.gameObject.SetActive(false);
            }
            
            // Hide skip button
            if (skipButton != null)
            {
                skipButton.gameObject.SetActive(false);
            }
            
            // Load game immediately
            LoadGameScene();
        }
    }
    
    private IEnumerator FadeToBlackOverTime(float fadeDuration)
    {
        CreateFadeOverlay();
        if (fadeImage == null) yield break;
        
        fadeImage.color = new Color(0, 0, 0, 0);
        
        // KEEP SKIP BUTTON VISIBLE AND WORKING DURING FADE
        if (skipButton != null) 
        {
            skipButton.gameObject.SetActive(true);
            // Make sure skip button stays on top of fade
            skipButton.transform.SetAsLastSibling();
        }
        
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeDuration && fadeImage != null && !isVideoFinished)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        
        // Ensure it's completely black if fade completed naturally
        if (fadeImage != null && !isVideoFinished)
        {
            fadeImage.color = Color.black;
        }
        
        Debug.Log("Fade to black completed");
        
        // Only proceed if we weren't skipped
        if (!isVideoFinished)
        {
            StartCoroutine(PlayScaryTransition());
        }
    }
    
    private IEnumerator CompleteTransitionQuickly()
    {
        // Do a quick fade to black
        yield return StartCoroutine(FadeToBlackOverTime(1.0f));
        
        if (!isVideoFinished)
        {
            StartCoroutine(PlayScaryTransition());
        }
    }
    
    private IEnumerator PlayScaryTransition()
    {
        if (isVideoFinished) yield break;
        
        inScaryTextPhase = true;
        Debug.Log("Starting scary text phase");
        
        // KEEP SKIP BUTTON VISIBLE AND WORKING DURING SCARY TEXT
        if (skipButton != null) 
        {
            skipButton.gameObject.SetActive(true);
            skipButton.transform.SetAsLastSibling();
        }
        
        // Show and shake the scary text
        yield return StartCoroutine(ShowShakingText());
        
        // Only load game if we weren't skipped during scary text
        if (inScaryTextPhase)
        {
            LoadGameScene();
        }
    }
    
    private IEnumerator ShowShakingText()
    {
        if (scaryText != null && !isVideoFinished)
        {
            // Set up the text
            scaryText.gameObject.SetActive(true);
            scaryText.color = Color.red;
            scaryText.text = "WARNING\nDANGER AHEAD\nPROCEED WITH CAUTION";
            scaryText.alignment = TextAlignmentOptions.Center;
            
            // Make sure text is above fade but below skip button
            scaryText.transform.SetSiblingIndex(transform.childCount - 2);
            
            // Get original position
            Vector3 originalPos = scaryText.transform.localPosition;
            
            float elapsedTime = 0f;
            
            while (elapsedTime < textDuration && inScaryTextPhase)
            {
                elapsedTime += Time.deltaTime;
                
                // Shake effect
                float shakeX = Random.Range(-1f, 1f) * shakeIntensity;
                float shakeY = Random.Range(-1f, 1f) * shakeIntensity;
                scaryText.transform.localPosition = originalPos + new Vector3(shakeX, shakeY, 0);
                
                // Pulsing effect
                float pulse = Mathf.PingPong(elapsedTime * 2f, 0.3f) + 0.7f;
                scaryText.color = new Color(1f, 0f, 0f, pulse);
                
                yield return null;
            }
            
            if (inScaryTextPhase)
            {
                // Reset position
                scaryText.transform.localPosition = originalPos;
            }
        }
        else
        {
            // If no text object, just wait (unless skipped)
            float elapsedTime = 0f;
            while (elapsedTime < textDuration && inScaryTextPhase)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
    
    void LoadGameScene()
    {
        Debug.Log("Loading game scene");
        SceneManager.LoadScene("SampleScene");
    }
    
    private void CreateFadeOverlay()
    {
        if (fadeImage != null) return;
        
        GameObject existingFade = GameObject.Find("FadeOverlay");
        if (existingFade != null)
        {
            fadeImage = existingFade.GetComponent<Image>();
            return;
        }

        GameObject fadeObject = new GameObject("FadeOverlay");
        Canvas canvas = fadeObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9998; // Lower than skip button
        
        fadeImage = fadeObject.AddComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0);
        
        RectTransform rect = fadeImage.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        DontDestroyOnLoad(fadeObject);
    }
    
    void OnDestroy()
    {
        // Clean up
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
}