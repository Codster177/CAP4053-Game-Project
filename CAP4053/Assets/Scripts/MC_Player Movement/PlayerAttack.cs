using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float[] comboResetTime; // the combo time window
    [SerializeField] private int damage = 10;
    [SerializeField] private List<GameObject> enemiesInRange = new List<GameObject>();
    private PlayerController playerController;
    private Coroutine comboResetCoroutine;
    private int comboStep = 0;
    private bool isAttacking = false, canAttack = true;

    void Update()
    {
        if (!canAttack)
        {
            return;
        }
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
        if (isAttacking)
        {
            return;
        }

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

    public void SetCanAttack(bool newState)
    {
        canAttack = newState;
    }

    IEnumerator AttackCoroutine()
    {
        isAttacking = true;

        // sync with animation length
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < enemiesInRange.Count; i++)
        {
            EnemyHealth enemyHealth = enemiesInRange[i].GetComponent<EnemyHealth>();
            EffectHolder effectHolder = enemiesInRange[i].GetComponent<EffectHolder>();
            if (effectHolder != null)
            {
                Vector2 knockbackDir = Knockback.CalculateDir(enemiesInRange[i].transform.position, transform.position);
                effectHolder.AddEffect(new Knockback(0.1f, 20, knockbackDir));
            }
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }

        isAttacking = false;
    }

    IEnumerator ResetComboAfterDelay()
    {
        yield return new WaitForSeconds(comboResetTime[comboStep - 1]);
        comboStep = 0;
        animator.SetInteger("ComboStep", 0);
    }
    public void SetPlayerController(PlayerController playerController)
    {
        this.playerController = playerController;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (!other.CompareTag("Enemy")) return;

        if (!enemiesInRange.Contains(other.gameObject))
        {
            enemiesInRange.Add(other.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        if (enemiesInRange.Contains(other.gameObject))
        {
            enemiesInRange.Remove(other.gameObject);
        }

    }
}
