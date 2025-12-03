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
    
    [Header("Character Profiles")]
    [SerializeField] private Image leftCharacterImage;
    [SerializeField] private Image rightCharacterImage;
    [SerializeField] private TextMeshProUGUI leftNameText;
    [SerializeField] private TextMeshProUGUI rightNameText;
    [SerializeField] private GameObject leftCharacterPanel;
    [SerializeField] private GameObject rightCharacterPanel;
    
    [Header("Character Sprites")]
    [SerializeField] private Sprite playerSprite;
    [SerializeField] private Sprite catSprite;
    [SerializeField] private Sprite bossSprite; // Add other NPC sprites as needed
    
    [Header("Characters")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator catAnimator;
    [SerializeField] private Transform catStartPosition;
    [SerializeField] private Transform catEndPosition;
    
    [Header("Cutscene Settings")]
    [SerializeField] private string[] dialogueLines;
    [SerializeField] private float fadeOutSpeed = 1f;
    
    [Header("Timing Settings")]
    [SerializeField] private float wakeUpDuration = 2f;
    [SerializeField] private float catConfuseDuration = 1f;
    [SerializeField] private float catWalkDuration = 2f;
    [SerializeField] private float mergeDuration = 1.5f;

    [Header("Bobble Settings")]
    [SerializeField] private float bobbleHeight = 10f;
    [SerializeField] private float bobbleSpeed = 2f;

    // Static variable that persists during gameplay but resets when game restarts
    private static bool _cutsceneCompleted = false;
    
    private int currentLine = 0;
    private bool dialogueActive = false;
    private bool cutscenePlaying = false;
    private CanvasGroup canvasGroup;
    private Vector2 originalIndicatorPosition;
    private PlayerController playerController;
    private MonoBehaviour playerAttackScript;
    private bool mergePlayed = false;

    // Reference to the cat GameObject so we can hide it
    private GameObject catObject;
    private Vector3 catOriginalPosition;
    
    private bool[] isPlayerSpeaking = {
        true,  // Line 0: Player
        true,  // Line 1: Player
        false, // Line 2: NPC (Cat)
        true,  // Line 3: Player
        false, // Line 4: NPC (Cat)
        true,  // Line 5: Player
        true,  // Line 6: Player
        false, // Line 7: NPC (Cat)
        false, // Line 8: NPC (Cat)
        true,  // Line 9: Player
        false, // Line 10: NPC (Cat)
        false, // Line 11: NPC (Cat)
        true,  // Line 12: Player
        false, // Line 13: NPC (Cat)
        false, // Line 14: NPC (Cat)
        false, // Line 15: NPC (Cat)
        true,  // Line 16: Player
        false, // Line 17: NPC (Cat)
        true,  // Line 18: Player
        false, // Line 19: NPC (Cat)
        false, // Line 20: NPC (Cat)
        true,  // Line 21: Player
        false, // Line 22: NPC (Cat)
        false, // Line 23: NPC (Cat)
        false, // Line 24: NPC (Cat),
    };

    void Awake()
    {
        // Store reference to cat object early
        if (catAnimator != null)
        {
            catObject = catAnimator.gameObject;
            catOriginalPosition = catObject.transform.position;
        }
    }

    void Start()
    {
        Debug.Log("Cutscene Start() called. Cutscene completed: " + _cutsceneCompleted);
        
        // Check if cutscene has already been completed in this play session
        if (_cutsceneCompleted)
        {
            Debug.Log("Cutscene already completed this session, skipping...");
            
            // IMPORTANT: Hide the cat immediately
            if (catObject != null)
            {
                catObject.SetActive(false);
                Debug.Log("Cat hidden because cutscene was already completed");
            }
            
            // Clean up any cutscene objects that might be active
            CleanupCutsceneObjects();
            
            // Ensure player controls are enabled
            EnablePlayerControlsImmediately();
            
            // Disable this script to prevent it from running again
            this.enabled = false;
            return;
        }
        
        InitializeCutscene();
    }
    
    private void InitializeCutscene()
    {
        canvasGroup = dialogueBox.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = dialogueBox.AddComponent<CanvasGroup>();
        
        playerController = FindObjectOfType<PlayerController>();
        playerAttackScript = FindObjectOfType<PlayerAttack>();
        
        // Make sure cat is at start position and active
        if (catObject != null)
        {
            catObject.SetActive(true);
            
            // Reset to original position
            if (catStartPosition != null)
            {
                catObject.transform.position = catStartPosition.position;
            }
            else
            {
                catObject.transform.position = catOriginalPosition;
            }
            
            // Reset cat sprite alpha if it was faded out
            SpriteRenderer catSpriteRenderer = catObject.GetComponent<SpriteRenderer>();
            if (catSpriteRenderer != null)
            {
                catSpriteRenderer.color = new Color(catSpriteRenderer.color.r, catSpriteRenderer.color.g, catSpriteRenderer.color.b, 1f);
            }
            
            // Reset cat animator to idle
            if (catAnimator != null)
            {
                catAnimator.Play("Idle");
                catAnimator.Rebind();
                catAnimator.Update(0f);
            }
            
            Debug.Log("Cat initialized for cutscene");
        }
        
        // Reset merge flag
        mergePlayed = false;
        
        if (clickIndicator != null)
        {
            clickIndicator.SetActive(false);
            originalIndicatorPosition = clickIndicator.GetComponent<RectTransform>().anchoredPosition;
        }
        
        // Initialize character panels
        if (leftCharacterPanel != null) leftCharacterPanel.SetActive(false);
        if (rightCharacterPanel != null) rightCharacterPanel.SetActive(false);
        
        dialogueBox.SetActive(false);
        
        if (playerAnimator != null)
        {
            playerAnimator.Play("Waking_up");
            playerAnimator.Rebind();
            playerAnimator.Update(0f);
        }
        
        StartCoroutine(StartCutsceneAfterDelay(1f));
    }
    
    private void CleanupCutsceneObjects()
    {
        Debug.Log("Cleaning up cutscene objects");
        
        // Hide dialogue box and panels
        if (dialogueBox != null) 
        {
            dialogueBox.SetActive(false);
            Debug.Log("Dialogue box hidden");
        }
        
        if (leftCharacterPanel != null) 
        {
            leftCharacterPanel.SetActive(false);
            Debug.Log("Left panel hidden");
        }
        
        if (rightCharacterPanel != null) 
        {
            rightCharacterPanel.SetActive(false);
            Debug.Log("Right panel hidden");
        }
        
        if (clickIndicator != null) 
        {
            clickIndicator.SetActive(false);
            Debug.Log("Click indicator hidden");
        }
        
        // Make sure player is in idle state
        if (playerAnimator != null)
        {
            playerAnimator.Play("Idle");
            Debug.Log("Player set to idle");
        }
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
        
        // Optional debug: Press F5 to reset cutscene
        if (Input.GetKeyDown(KeyCode.F5))
        {
            ResetCutsceneState();
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
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
        for (int i = 9; i < dialogueLines.Length && i <= 25; i++)
        {
            yield return StartCoroutine(ShowDialogueLine(i));
        }
        
        StartCoroutine(EndCutscene());
    }

    private IEnumerator ShowDialogueLine(int lineIndex)
    {
        dialogueBox.SetActive(true);
        currentLine = lineIndex;
        
        // Set up character profile based on who's speaking
        SetupCharacterProfile(lineIndex);
        
        // Display full text immediately
        if (currentLine < dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[currentLine];
            
            // Show click indicator immediately
            if (clickIndicator != null)
            {
                clickIndicator.SetActive(true);
                StartCoroutine(BobbleIndicator());
            }
        }
        
        yield return WaitForClick();
    }

    private void SetupCharacterProfile(int lineIndex)
    {
        // Hide both panels first
        if (leftCharacterPanel != null) leftCharacterPanel.SetActive(false);
        if (rightCharacterPanel != null) rightCharacterPanel.SetActive(false);
        
        if (lineIndex < isPlayerSpeaking.Length && isPlayerSpeaking[lineIndex])
        {
            // Player speaking - show on right side
            if (rightCharacterPanel != null)
            {
                rightCharacterPanel.SetActive(true);
                if (rightCharacterImage != null && playerSprite != null)
                    rightCharacterImage.sprite = playerSprite;
                if (rightNameText != null)
                    rightNameText.text = "Laeseo";
            }
        }
        else
        {
            // NPC speaking - show on left side
            if (leftCharacterPanel != null)
            {
                leftCharacterPanel.SetActive(true);
                if (leftCharacterImage != null)
                {
                    // Determine which NPC sprite to use
                    if (lineIndex <= 8) // First part - cat speaking
                    {
                        leftCharacterImage.sprite = catSprite;
                        if (leftNameText != null)
                            leftNameText.text = "???";
                    }
                    else // Later parts - could be different NPC
                    {
                        leftCharacterImage.sprite = catSprite; // Use cat or other NPC sprite
                        if (leftNameText != null)
                            leftNameText.text = "???";
                    }
                }
            }
        }
    }

    private IEnumerator PlayMergeAnimation()
    {
        if (mergePlayed) yield break;
        mergePlayed = true;

        if (catAnimator != null)
        {
            SpriteRenderer catSpriteRenderer = catAnimator.GetComponent<SpriteRenderer>();
            if (catSpriteRenderer != null)
            {
                float fadeTime = 0.4f; 
                float fadeElapsed = 0f;
                Color original = catSpriteRenderer.color;

                catAnimator.Play("Idle");

                bool insertStarted = false;

                while (fadeElapsed < fadeTime)
                {
                    fadeElapsed += Time.deltaTime;
                    float t = fadeElapsed / fadeTime;
                    catSpriteRenderer.color = new Color(original.r, original.g, original.b, 1f - t);

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

                catSpriteRenderer.color = new Color(original.r, original.g, original.b, 0f);
                
                // IMPORTANT: Disable the cat object after fade
                catAnimator.gameObject.SetActive(false);
                Debug.Log("Cat disabled after merge animation");
            }
            else
            {
                // If no sprite renderer, just disable the object
                catAnimator.gameObject.SetActive(false);
                Debug.Log("Cat disabled (no sprite renderer found)");
            }
        }

        if (playerAnimator != null)
        {
            yield return new WaitForSeconds(mergeDuration);
            playerAnimator.Play("Idle");
        }
    }

    private IEnumerator HideDialogue()
    {
        // Hide character panels when dialogue is hidden
        if (leftCharacterPanel != null) leftCharacterPanel.SetActive(false);
        if (rightCharacterPanel != null) rightCharacterPanel.SetActive(false);
        
        // Hide click indicator
        if (clickIndicator != null)
            clickIndicator.SetActive(false);
            
        dialogueBox.SetActive(false);
        yield return new WaitForSeconds(0.3f);
    }

    private IEnumerator WaitForClick()
    {
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
        
        // No typing to interrupt, so clicks will just advance the dialogue
        // The waiting is handled in WaitForClick() coroutine
    }

    private IEnumerator EndCutscene()
    {
        dialogueActive = false;
        cutscenePlaying = false;
        
        // Hide character panels
        if (leftCharacterPanel != null) leftCharacterPanel.SetActive(false);
        if (rightCharacterPanel != null) rightCharacterPanel.SetActive(false);
        
        // Hide click indicator
        if (clickIndicator != null)
            clickIndicator.SetActive(false);
            
        float elapsedTime = 0f;
        float startAlpha = canvasGroup.alpha;
        
        while (elapsedTime < fadeOutSpeed)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeOutSpeed);
            yield return null;
        }
        
        dialogueBox.SetActive(false);
        
        // FINAL STEP: Make absolutely sure the cat is hidden
        if (catObject != null)
        {
            catObject.SetActive(false);
            Debug.Log("Cat finally hidden at end of cutscene");
        }
        
        // Mark cutscene as completed for this play session
        _cutsceneCompleted = true;
        
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
    
    private void EnablePlayerControlsImmediately()
    {
        // Find components if they're not already found
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();
        
        if (playerAttackScript == null)
            playerAttackScript = FindObjectOfType<PlayerAttack>();
        
        // Enable them
        if (playerController != null)
            playerController.enabled = true;
        
        if (playerAttackScript != null)
            playerAttackScript.enabled = true;
    }

    private void OnCutsceneComplete()
    {
        Debug.Log("Tutorial cutscene finished");
    }
    
    // Static method to manually reset the cutscene (for debugging/testing)
    public static void ResetCutsceneState()
    {
        _cutsceneCompleted = false;
        Debug.Log("Cutscene state reset - will play on next start");
    }
}