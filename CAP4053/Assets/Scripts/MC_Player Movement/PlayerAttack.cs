using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private float comboResetTime = 1f; // the combo time window
    private int comboStep = 0;
    private bool isAttacking = false;
    private Coroutine comboResetCoroutine;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        //light attack
        if (Input.GetButtonDown("LightAttack"))
        {
            HandleLightAttack();
            Debug.Log("You weakling... (get light attacked)");
        }

        //heavy attack
        if (Input.GetButtonDown("HeavyAttack"))
        {
            Debug.Log("Get heavy attacked... weirdo");
        }
    }

    private void HandleLightAttack()
    {
        if (isAttacking) return;

        comboStep++;

        // clamp (whatever that means) combo btw 1 and 3
        if (comboStep > 3) comboStep = 1;

        animator.SetInteger("ComboStep", comboStep);
        animator.SetTrigger("LightAttack");

        // lock player so they cant spam cancel animations
        StartCoroutine(AttackCoroutine());

        // reset combo timer
        if (comboResetCoroutine != null)
        {
            StopCoroutine(comboResetCoroutine);
        }

        comboResetCoroutine = StartCoroutine(ResetComboAfterDelay());
    }

    IEnumerator AttackCoroutine()
    {
        isAttacking = true;

        // sync with animation length
        yield return new WaitForSeconds(0.4f);

        isAttacking = false;
    }

    IEnumerator ResetComboAfterDelay()
    {
        yield return new WaitForSeconds(comboResetTime);
        comboStep = 0;
        animator.SetInteger("ComboStep", 0);
    }
}
