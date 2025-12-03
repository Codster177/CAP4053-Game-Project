using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shrine : MonoBehaviour
{
    [SerializeField] private Animator shrineAnim, textAnim;
    private bool playerInRange = false, shrineActive = true;

    void Update()
    {
        if (playerInRange)
        {
            if (shrineActive && Input.GetButtonDown("UseShrine"))
            {
                ActivateShrine();
            }
        }
    }

    private void PlayerInRange(bool enabled)
    {
        if (enabled)
        {
            if (shrineActive)
            {
                playerInRange = true;
                textAnim.SetInteger("State", 1);
            }
        }
        else
        {
            playerInRange = false;
            textAnim.SetInteger("State", 0);
        }
    }
    private void ActivateShrine()
    {
        shrineActive = false;
        playerInRange = false;

        textAnim.SetInteger("State", 2);
        shrineAnim.SetInteger("State", 1);

        GameManager.publicGameManager.HealFull();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            PlayerInRange(true);
        }
    }
    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            PlayerInRange(false);
        }
    }
}
