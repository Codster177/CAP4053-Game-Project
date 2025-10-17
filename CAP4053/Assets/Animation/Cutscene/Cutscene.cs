using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class TypewriterDialogueWithBobble : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject clickIndicator;
    
    [Header("Dialogue Settings")]
    [SerializeField] private string[] dialogueLines;
    [SerializeField] private float typewriterSpeed = 0.05f;
    [SerializeField] private float fadeOutSpeed = 1f;
    
    [Header("Bobble Settings")]
    [SerializeField] private float bobbleHeight = 10f;
    [SerializeField] private float bobbleSpeed = 2f;

    private int currentLine = 0;
    private bool isTyping = false;
    private bool dialogueActive = false;
    private CanvasGroup canvasGroup;
    private Vector2 originalIndicatorPosition;
    private PlayerController playerController;
    private MonoBehaviour playerAttackScript;

    void Start()
    {
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
        StartCoroutine(StartDialogueAfterDelay(1f));
    }
    
    private IEnumerator StartDialogueAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartDialogue();
    }
    
    void Update()
    {
        if (dialogueActive && Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }
    
    public void StartDialogue()
    {
        if (dialogueLines.Length == 0) return;
        
        dialogueActive = true;
        currentLine = 0;
        canvasGroup.alpha = 1f;
        dialogueBox.SetActive(true);
        
        DisablePlayerControls();
        
        StartCoroutine(TypeLine(dialogueLines[currentLine]));
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
        else
        {
            if (clickIndicator != null)
            {
                clickIndicator.SetActive(false);
                StopCoroutine("BobbleIndicator");
            }
            
            currentLine++;
            
            if (currentLine < dialogueLines.Length)
            {
                StartCoroutine(TypeLine(dialogueLines[currentLine]));
            }
            else
            {
                StartCoroutine(FadeOutAndEnd());
            }
        }
    }
    
    private IEnumerator FadeOutAndEnd()
    {
        dialogueActive = false;
        
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
        
        OnDialogueComplete();
    }
    
    private void DisablePlayerControls()
    {
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        
        if (playerAttackScript != null)
        {
            playerAttackScript.enabled = false;
        }
        
        Rigidbody2D playerRb = FindObjectOfType<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
        }
    }
    
    private void EnablePlayerControls()
    {
        if (playerController != null)
        {
            playerController.enabled = true;
        }
        
        if (playerAttackScript != null)
        {
            playerAttackScript.enabled = true;
        }
    }
    
    private void OnDialogueComplete()
    {
        Debug.Log("Dialogue complete");
    }
}