using UnityEngine;
using System.Collections;

public class TimedFader : MonoBehaviour
{
    public CanvasGroup uiElement;   
    public float waitTime = 15f;    //how long to wait before fading
    public float fadeSpeed = 2f;    //how many seconds the fade takes

    void Start()
    {
        //timer starts when game begins (i want to edit this so that it shows up when the text dialouge is finished)
        StartCoroutine(FadeOutRoutine());
    }

    IEnumerator FadeOutRoutine()
    {
        
        yield return new WaitForSeconds(waitTime);

        
        float counter = 0f;
        while (counter < fadeSpeed)
        {
            counter += Time.deltaTime;
            
            uiElement.alpha = Mathf.Lerp(1f, 0f, counter / fadeSpeed);

          
            yield return null;
        }

       
        uiElement.alpha = 0;
        uiElement.gameObject.SetActive(false);
    }
}