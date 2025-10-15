using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameStartFade : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1.5f;
    
    void Start()
    {
        GameObject fadeObject = GameObject.Find("FadeOverlay");
        
        if (fadeObject != null)
        {
            StartCoroutine(FadeIn(fadeObject));
        }
        else
        {
            CreateAndFadeIn();
        }
    }
    
    private IEnumerator FadeIn(GameObject fadeObject)
    {
        Image fadeImage = fadeObject.GetComponent<Image>();
        
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        
        Destroy(fadeObject);
    }
    
    private void CreateAndFadeIn()
    {
        GameObject fadeObject = new GameObject("FadeOverlay");
        Canvas canvas = fadeObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;
        
        Image fadeImage = fadeObject.AddComponent<Image>();
        fadeImage.color = Color.black; 
        
        RectTransform rect = fadeImage.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        StartCoroutine(FadeIn(fadeObject));
    }
}