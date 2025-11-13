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
    public float fadeStartBeforeEnd = 1f;
    
    private Image fadeImage;
    private bool isVideoFinished = false;
    private bool fadeStarted = false;
    private bool inScaryTextPhase = false;
    private CanvasGroup scaryTextCanvasGroup;
    
    void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;
        
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipIntro);
            skipButton.gameObject.SetActive(true);
        }
        
        if (scaryText != null)
        {
            scaryText.gameObject.SetActive(false);
            scaryTextCanvasGroup = scaryText.GetComponent<CanvasGroup>();
            if (scaryTextCanvasGroup == null)
                scaryTextCanvasGroup = scaryText.gameObject.AddComponent<CanvasGroup>();
        }
        
        GameObject existingFade = GameObject.Find("FadeOverlay");
        if (existingFade != null)
        {
            Destroy(existingFade);
        }
        
        StartCoroutine(FadeInVideo());
        videoPlayer.Play();
        StartCoroutine(StartFadeBeforeEnd());
    }
    
    private IEnumerator StartFadeBeforeEnd()
    {
        yield return new WaitForSeconds(0.5f);
        
        while (videoPlayer.isPlaying && !isVideoFinished)
        {
            if (videoPlayer.frameCount > 0 && videoPlayer.frameRate > 0)
            {
                double totalTime = videoPlayer.frameCount / videoPlayer.frameRate;
                double currentTime = videoPlayer.time;
                double timeLeft = totalTime - currentTime;
                
                if (timeLeft <= fadeStartBeforeEnd && !fadeStarted)
                {
                    fadeStarted = true;
                    StartCoroutine(FadeToBlackOverTime(fadeStartBeforeEnd));
                    break;
                }
            }
            
           
            else if (videoPlayer.time > 10f && !fadeStarted) 
            {
                fadeStarted = true;
                StartCoroutine(FadeToBlackOverTime(1f));
                break;
            }
            
            yield return new WaitForSeconds(0.1f);
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
            
            if (inScaryTextPhase) return;
            
            if (!fadeStarted)
            {
                fadeStarted = true;
                StartCoroutine(CompleteTransitionQuickly());
            }
            else
            {
                StartCoroutine(PlayScaryTransition());
            }
        }
    }
    
    public void SkipIntro()
    {
        if (!isVideoFinished || inScaryTextPhase)
        {
            Debug.Log("Skip button pressed");
            isVideoFinished = true;
            inScaryTextPhase = false;
            fadeStarted = true;
            
            StopAllCoroutines();
            
            if (videoPlayer != null && videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
            }
            
            if (scaryText != null)
            {
                scaryText.gameObject.SetActive(false);
            }
            
            if (skipButton != null)
            {
                skipButton.gameObject.SetActive(false);
            }
            
            LoadGameScene();
        }
    }
    
    private IEnumerator FadeToBlackOverTime(float fadeDuration)
    {
        CreateFadeOverlay();
        if (fadeImage == null) yield break;
        
        fadeImage.color = new Color(0, 0, 0, 0);
        
        if (skipButton != null) 
        {
            skipButton.gameObject.SetActive(true);
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
        
        if (fadeImage != null && !isVideoFinished)
        {
            fadeImage.color = Color.black;
        }
        
        Debug.Log("Fade to black completed");
        
        if (!isVideoFinished)
        {
            StartCoroutine(PlayScaryTransition());
        }
    }
    
    private IEnumerator CompleteTransitionQuickly()
    {
        yield return StartCoroutine(FadeToBlackOverTime(1.0f));
        
        if (!isVideoFinished)
        {
            StartCoroutine(PlayScaryTransition());
        }
    }
    
    private IEnumerator PlayScaryTransition()
{
    inScaryTextPhase = true;
    Debug.Log("Starting scary text phase");

    EnsureBlackBackground();

    if (skipButton != null) 
    {
        skipButton.gameObject.SetActive(true);
        skipButton.transform.SetAsLastSibling();
    }

    yield return StartCoroutine(ShowShakingText());

    if (inScaryTextPhase)
    {
        LoadGameScene();
    }
}

    
    private void EnsureBlackBackground()
    {
        CreateFadeOverlay();
        if (fadeImage != null)
        {
            fadeImage.color = Color.black;
            fadeImage.transform.SetSiblingIndex(0);
        }
    }
    
    private IEnumerator ShowShakingText()
{
    if (scaryText == null) yield break;

    scaryText.gameObject.SetActive(true);
    scaryText.color = Color.red;
    scaryText.text = GetRandomScaryText();
    scaryText.alignment = TextAlignmentOptions.Center;

    RectTransform rect = scaryText.rectTransform;

    float elapsed = 0f;

    while (elapsed < textDuration && inScaryTextPhase)
    {
        elapsed += Time.deltaTime;

        float shakeX = Random.Range(-shakeIntensity, shakeIntensity);
        float shakeY = Random.Range(-shakeIntensity, shakeIntensity);
        rect.anchoredPosition = new Vector2(shakeX, shakeY);

        float alpha = Mathf.PingPong(Time.time * 4f, 0.5f) + 0.5f;
        scaryText.color = new Color(1, 0, 0, alpha);

        if (Random.Range(0f, 1f) < 0.03f)
        {
            scaryText.text = GetRandomScaryText();
        }

        yield return null;
    }

    rect.anchoredPosition = Vector2.zero;
}

    
    private string GetRandomScaryText()
    {
        string[] scaryTexts = {
            "WAKE UP",
            "YOU FAILED US\n YOU FAILED EVERYONE",
            "WAKE UP\n WAKE UP\n WAKE UP",
            "IT'S TOO LATE\nYOU WILL NEVER AMOUNT TO ANYTHING",
            "WAKE UP BEFORE IT'S TOO LATE",
            "YOU COULDN'T SAVE ANYONE",
            "WHY ARE YOU HERE",
            "DON'T LISTEN TO THEM AND WAKE THE FUCK UP!!!"
        };
        
        return scaryTexts[Random.Range(0, scaryTexts.Length)];
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
        canvas.sortingOrder = 100; 
        
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
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
}