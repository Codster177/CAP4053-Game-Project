using UnityEngine;
using TMPro;
using System.Collections;

public class TypewriterDialogue : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI dialogueText;
    
    [Header("Dialogue Settings")]
    [SerializeField] private string[] dialogueLines;
    [SerializeField] private float typewriterSpeed = 0.05f;

    private bool isTyping = false;
    private bool nextPressed = false;

    void Start()
    {
        dialogueBox.SetActive(false);
        StartCoroutine(ShowDialogueSequence());
    }
    
    private IEnumerator ShowDialogueSequence()
    {
        yield return new WaitForSeconds(1f);
        dialogueBox.SetActive(true);

        foreach (string line in dialogueLines)
        {
            yield return StartCoroutine(TypeLine(line));

            nextPressed = false;
            yield return new WaitUntil(() => nextPressed);
        }

        dialogueBox.SetActive(false);
        Debug.Log("Dialogue finished!");
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        isTyping = false;
    }

    void Update()
    {
        if (dialogueBox.activeSelf && !isTyping)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
            {
                nextPressed = true;
            }
        }
    }
}
