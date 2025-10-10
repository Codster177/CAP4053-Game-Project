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
    [SerializeField] private List<int> animationQueue = new List<int>();
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
            // Debug.Log("You weakling... (get light attacked)");
        }

        //heavy attack
        if (Input.GetButtonDown("HeavyAttack"))
        {
            HandleHeavyAttack();
            Debug.Log("Get heavy attacked... weirdo");
        }
    }

    private void HandleLightAttack()
    {
        if (isAttacking)
        {
            return;
        }
        if (animationQueue.Count != 0)
        {
            if (animationQueue[animationQueue.Count - 1] == 3)
            {
                animationQueue.Add(1);
            }
            else
            {
                animationQueue.Add(animationQueue[animationQueue.Count - 1] + 1);
            }
        }
        else
        {
            animationQueue.Add(1);
            StartCoroutine(TriggerAnimation());
        }

        // clamp (whatever that means) combo btw 1 and 3

        // animator.SetInteger("ComboStep", comboStep);
        // animator.SetTrigger("LightAttack");

        // lock player so they cant spam cancel animations
        // StartCoroutine(AttackCoroutine());

        // reset combo timer
        if (comboResetCoroutine != null)
        {
            StopCoroutine(comboResetCoroutine);
        }

        comboResetCoroutine = StartCoroutine(ResetComboAfterDelay());
    }

    private void HandleHeavyAttack()
    {
        if (isAttacking) return;

        animator.SetTrigger("HeavyAttack");

        // lock player so they cant spam cancel animations
        StartCoroutine(AttackCoroutine());
    }

    public IEnumerator TriggerAnimation()
    {
        if (animationQueue.Count > 0)
        {
            if (animationQueue.Count != 1 && animationQueue.Count != 0)
            {
                animationQueue.RemoveAt(0);
            }
            animator.SetInteger("ComboStep", animationQueue[0]);
            StartCoroutine(AttackCoroutine());
            yield return new WaitForSeconds(comboResetTime[animationQueue[0] - 1]);
        }
    }

    public void SetCanAttack(bool newState)
    {
        canAttack = newState;
    }

    IEnumerator AttackCoroutine()
    {
        isAttacking = true;

        // sync with animation length
        yield return new WaitForSeconds(0.05f);
        for (int i = 0; i < enemiesInRange.Count; i++)
        {
            EnemyHealth enemyHealth = enemiesInRange[i].GetComponent<EnemyHealth>();
            EffectHolder effectHolder = enemiesInRange[i].GetComponent<EffectHolder>();
            if (effectHolder != null)
            {
                Vector2 knockbackDir = Knockback.CalculateDir(enemiesInRange[i].transform.position, transform.position);
                // Debug.Log(knockbackDir);
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
        int frontOfList;
        if (animationQueue.Count != 0)
        {
            frontOfList = animationQueue[0];
            yield return new WaitForSeconds(comboResetTime[frontOfList - 1]);
            animator.SetInteger("ComboStep", 0);
            animationQueue.Clear();
        }
        else
        {
            yield break;
        }
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
