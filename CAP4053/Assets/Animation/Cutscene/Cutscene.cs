using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class TutorialCutscene : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject clickIndicator;
    
    [Header("Characters")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator catAnimator;
    [SerializeField] private Transform catStartPosition;
    [SerializeField] private Transform catEndPosition;
    
    [Header("Cutscene Settings")]
    [SerializeField] private string[] dialogueLines;
    [SerializeField] private float typewriterSpeed = 0.05f;
    [SerializeField] private float fadeOutSpeed = 1f;
    
    [Header("Timing Settings")]
    [SerializeField] private float wakeUpDuration = 2f;
    [SerializeField] private float catConfuseDuration = 1f;
    [SerializeField] private float catWalkDuration = 2f;
    [SerializeField] private float mergeDuration = 1.5f;

    [Header("Bobble Settings")]
    [SerializeField] private float bobbleHeight = 10f;
    [SerializeField] private float bobbleSpeed = 2f;

    private int currentLine = 0;
    private bool isTyping = false;
    private bool dialogueActive = false;
    private bool cutscenePlaying = false;
    private CanvasGroup canvasGroup;
    private Vector2 originalIndicatorPosition;
    private PlayerController playerController;
    private MonoBehaviour playerAttackScript;
    private bool mergePlayed = false; // track merge once

    void Start()
    {
        Debug.Log("Cutscene Start() called");
        
        canvasGroup = dialogueBox.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = dialogueBox.AddComponent<CanvasGroup>();
        
        playerController = FindObjectOfType<PlayerController>();
        playerAttackScript = FindObjectOfType<PlayerAttack>();
        
        if (clickIndicator != null)
        {
            clickIndicator.SetActive(false);
            originalIndicatorPosition = clickIndicator.GetComponent<RectTransform>().anchoredPosition;
        }
        
        dialogueBox.SetActive(false);
        
        if (playerAnimator != null)
            playerAnimator.Play("Waking_up");
        
        if (catAnimator != null)
            catAnimator.Play("Idle");
        
        StartCoroutine(StartCutsceneAfterDelay(1f));
    }
    
    private IEnumerator StartCutsceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCutscene();
    }
    
    void Update()
    {
        if (cutscenePlaying && Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }
    
    public void StartCutscene()
    {
        if (dialogueLines.Length == 0)
        {
            Debug.LogError("No dialogue lines set in TutorialCutscene!");
            return;
        }
        
        cutscenePlaying = true;
        dialogueActive = true;
        currentLine = 0;
        canvasGroup.alpha = 1f;
        
        DisablePlayerControls();
        StartCoroutine(CutsceneSequence());
    }
    
    private IEnumerator CutsceneSequence()
    {
        Debug.Log("Cutscene sequence started");
        
        // Player wake up animation
        if (playerAnimator != null)
        {
            playerAnimator.Play("Waking_up");
            yield return new WaitForSeconds(wakeUpDuration);
            playerAnimator.Play("Idle");
        }
        
        // Dialogue
        for (int i = 0; i <= 3 && i < dialogueLines.Length; i++)
        {
            yield return StartCoroutine(ShowDialogueLine(i));
        }
        
        // Cat confused
        yield return StartCoroutine(HideDialogue());
        if (catAnimator != null)
        {
            catAnimator.Play("confuse");
            yield return new WaitForSeconds(catConfuseDuration);
            catAnimator.Play("Idle");
        }
        
        // Continue dialogue lines
        for (int i = 4; i <= 8 && i < dialogueLines.Length; i++)
        {
            yield return StartCoroutine(ShowDialogueLine(i));
        }
        
        // Cat walks to player
        yield return StartCoroutine(HideDialogue());
        yield return StartCoroutine(MoveCatToPlayer());
        
        // Merge
        yield return StartCoroutine(PlayMergeAnimation());
        
        // Final dialogue
        for (int i = 9; i <= 25 && i < dialogueLines.Length; i++)
        {
            yield return StartCoroutine(ShowDialogueLine(i));
        }
        
        StartCoroutine(EndCutscene());
    }

private IEnumerator PlayMergeAnimation()
{
    if (mergePlayed) yield break;
    mergePlayed = true;

    if (catAnimator != null)
    {
        SpriteRenderer catSprite = catAnimator.GetComponent<SpriteRenderer>();
        if (catSprite != null)
        {
            float fadeTime = 0.4f; 
            float fadeElapsed = 0f;
            Color original = catSprite.color;

            catAnimator.Play("Idle");

            bool insertStarted = false;

            while (fadeElapsed < fadeTime)
            {
                fadeElapsed += Time.deltaTime;
                float t = fadeElapsed / fadeTime;
                catSprite.color = new Color(original.r, original.g, original.b, 1f - t);

                if (!insertStarted && t >= 0.5f)
                {
                    insertStarted = true;
                    if (playerAnimator != null)
                    {
                        playerAnimator.Play("Insert");
                    }
                }

                yield return null;
            }

            catSprite.color = new Color(original.r, original.g, original.b, 0f);
            catAnimator.gameObject.SetActive(false);
        }
        else
        {
            catAnimator.gameObject.SetActive(false);
        }
    }

    if (playerAnimator != null)
    {
        yield return new WaitForSeconds(mergeDuration);
        playerAnimator.Play("Idle");
    }
}

    private IEnumerator ShowDialogueLine(int lineIndex)
    {
        dialogueBox.SetActive(true);
        currentLine = lineIndex;
        
        if (currentLine < dialogueLines.Length)
        {
            yield return StartCoroutine(TypeLine(dialogueLines[currentLine]));
        }
        
        yield return WaitForClick();
    }

    private IEnumerator HideDialogue()
    {
        dialogueBox.SetActive(false);
        yield return new WaitForSeconds(0.3f);
    }

    private IEnumerator WaitForClick()
    {
        while (isTyping)
            yield return null;

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        yield return null;
    }

    private IEnumerator MoveCatToPlayer()
    {
        if (catAnimator == null) yield break;
        
        float elapsedTime = 0f;
        Vector3 startPos = catStartPosition.position;
        Vector3 endPos = catEndPosition.position;
        
        catAnimator.Play("walk");

        while (elapsedTime < catWalkDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / catWalkDuration;
            catAnimator.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        catAnimator.transform.position = endPos;
        catAnimator.Play("Idle");
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;

        if (clickIndicator != null)
        {
            clickIndicator.SetActive(false);
            StopCoroutine("BobbleIndicator");
        }

        dialogueText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            if (Input.GetMouseButton(0))
            {
                dialogueText.text = line;
                break;
            }
            yield return new WaitForSeconds(typewriterSpeed);
        }

        if (clickIndicator != null)
        {
            clickIndicator.SetActive(true);
            StartCoroutine(BobbleIndicator());
        }

        isTyping = false;
    }

    private IEnumerator BobbleIndicator()
    {
        RectTransform indicatorRect = clickIndicator.GetComponent<RectTransform>();
        
        while (clickIndicator.activeInHierarchy)
        {
            float bobble = Mathf.Sin(Time.time * bobbleSpeed) * bobbleHeight;
            indicatorRect.anchoredPosition = originalIndicatorPosition + new Vector2(0, bobble);
            yield return null;
        }

        indicatorRect.anchoredPosition = originalIndicatorPosition;
    }

    private void HandleClick()
    {
        if (!dialogueActive) return;

        if (isTyping)
        {
            StopCoroutine("TypeLine");
            dialogueText.text = dialogueLines[currentLine];

            if (clickIndicator != null)
            {
                clickIndicator.SetActive(true);
                StartCoroutine(BobbleIndicator());
            }

            isTyping = false;
        }
    }

    private IEnumerator EndCutscene()
    {
        dialogueActive = false;
        cutscenePlaying = false;
        
        float elapsedTime = 0f;
        float startAlpha = canvasGroup.alpha;
        
        while (elapsedTime < fadeOutSpeed)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeOutSpeed);
            yield return null;
        }
        
        dialogueBox.SetActive(false);
        EnablePlayerControls();
        OnCutsceneComplete();
    }

    private void DisablePlayerControls()
    {
        if (playerController != null)
            playerController.enabled = false;
        
        if (playerAttackScript != null)
            playerAttackScript.enabled = false;
        
        Rigidbody2D playerRb = FindObjectOfType<Rigidbody2D>();
        if (playerRb != null)
            playerRb.linearVelocity = Vector2.zero;
    }
    
    private void EnablePlayerControls()
    {
        if (playerController != null)
            playerController.enabled = true;
        
        if (playerAttackScript != null)
            playerAttackScript.enabled = true;
    }

    private void OnCutsceneComplete()
    {
        Debug.Log("Tutorial cutscene finished fuckers");
    }
}
