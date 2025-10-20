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
    [SerializeField] private float catRiseDuration = 1f;
    [SerializeField] private float catWalkDuration = 2f;
    [SerializeField] private float insertDuration = 1.5f;

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

    void Start()
    {
        canvasGroup = dialogueBox.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = dialogueBox.AddComponent<CanvasGroup>();
        
        // Find player scripts
        playerController = FindObjectOfType<PlayerController>();
        playerAttackScript = FindObjectOfType<PlayerAttack>();
        
        if (clickIndicator != null)
        {
            clickIndicator.SetActive(false);
            originalIndicatorPosition = clickIndicator.GetComponent<RectTransform>().anchoredPosition;
        }
        
        dialogueBox.SetActive(false);
        
        // Set initial animations
        playerAnimator.Play("Waking_up");
        catAnimator.Play("Cat_sit");
        
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
        if (dialogueLines.Length == 0) return;
        
        cutscenePlaying = true;
        dialogueActive = true;
        currentLine = 0;
        canvasGroup.alpha = 1f;
        
        // Disable player controls
        DisablePlayerControls();
        
        // Start the cutscene sequence
        StartCoroutine(CutsceneSequence());
    }
    
    private IEnumerator CutsceneSequence()
    {
        // Step 1: Player wake up animation
        Debug.Log("Step 1: Player waking up");
        playerAnimator.Play("Waking_up");
        yield return new WaitForSeconds(wakeUpDuration);
        
        // Step 2: Player goes to idle after waking
        playerAnimator.Play("Idle");
        
        // Step 3: First dialogue line (Cat speaks while sitting)
        Debug.Log("Step 3: First dialogue");
        dialogueBox.SetActive(true);
        yield return StartCoroutine(TypeLine(dialogueLines[0]));
        
        // Wait for click to continue
        while (isTyping || !Input.GetMouseButtonDown(0))
            yield return null;
        
        // Step 4: Cat confused reaction
        Debug.Log("Step 4: Cat confused");
        catAnimator.Play("Cat_confuse");
        yield return new WaitForSeconds(1f);
        
        // Step 5: Second dialogue line
        yield return StartCoroutine(TypeLine(dialogueLines[1]));
        
        // Wait for click to continue
        while (isTyping || !Input.GetMouseButtonDown(0))
            yield return null;
        
        // Step 6: Cat rises
        Debug.Log("Step 6: Cat rising");
        catAnimator.Play("Cat_rise");
        yield return new WaitForSeconds(catRiseDuration);
        
        // Step 7: Third dialogue line
        yield return StartCoroutine(TypeLine(dialogueLines[2]));
        
        // Wait for click to continue
        while (isTyping || !Input.GetMouseButtonDown(0))
            yield return null;
        
        // Step 8: Cat walks to player
        Debug.Log("Step 8: Cat walking to player");
        yield return StartCoroutine(MoveCatToPlayer());
        
        // Step 9: Fourth dialogue line
        yield return StartCoroutine(TypeLine(dialogueLines[3]));
        
        // Wait for click to continue
        while (isTyping || !Input.GetMouseButtonDown(0))
            yield return null;
        
        // Step 10: Cat insert animation
        Debug.Log("Step 10: Cat inserting into player");
        catAnimator.Play("Insert");
        yield return new WaitForSeconds(insertDuration);
        
        // Cutscene complete
        StartCoroutine(EndCutscene());
    }
    
    private IEnumerator MoveCatToPlayer()
    {
        float elapsedTime = 0f;
        Vector3 startPos = catStartPosition.position;
        Vector3 endPos = catEndPosition.position;
        
        // Start walking animation
        catAnimator.Play("Cat_walk");
        
        while (elapsedTime < catWalkDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / catWalkDuration;
            catAnimator.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        
        // Go back to idle after walking
        catAnimator.Play("Cat_idle");
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
            StopAllCoroutines();
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
        
        // Fade out dialogue box
        float elapsedTime = 0f;
        float startAlpha = canvasGroup.alpha;
        
        while (elapsedTime < fadeOutSpeed)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeOutSpeed);
            yield return null;
        }
        
        dialogueBox.SetActive(false);
        
        // Re-enable player controls
        EnablePlayerControls();
        
        // Hide cat after insertion
        catAnimator.gameObject.SetActive(false);
        
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
        Debug.Log("Tutorial cutscene complete - game begins!");
        // Add any additional game start logic here
    }
}