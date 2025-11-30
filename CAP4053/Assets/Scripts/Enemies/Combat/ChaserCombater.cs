using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChaserCombater : EnemyCombater
{
    protected ChaserController chaserController;
    [SerializeField] protected List<GameObject> inRange;
    [SerializeField] protected float damageAmount, attackSpeed, enemyKnockback, knockbackTime;
    [SerializeField] protected bool hitWhileDash;
    protected Coroutine currentAttack;

    protected void Start()
    {
        chaserController = controller as ChaserController;
        if (controller == null)
        {
            Debug.Log($"Chaser Controller = null");
        }
    }
    // Recognizes the player when encountering them in the enemies trigger collider.
    protected void Update()
    {
        ChaserAttack();
    }
    protected void ChaserAttack()
    {
        if (currentAttack == null)
        {
            currentAttack = StartCoroutine(StartAttack());
        }
        if (inRange.Count != 0)
        {
            chaserController.SetPosition(false, transform.position);
        }
        else
        {
            chaserController.SetPosition(true, new Vector3());
        }
    }
    public List<float> GetEnemyStats()
    {
        return new List<float>() { damageAmount, attackSpeed };
    }
    public void SetEnemyStats(float damageAmount, float attackSpeed)
    {
        this.damageAmount = damageAmount;
        this.attackSpeed = attackSpeed;
    }
    protected IEnumerator StartAttack()
    {
        yield return new WaitForSeconds(attackSpeed);
        for (int i = 0; i < inRange.Count; i++)
        {
            Vector2 knockbackDir = Knockback.CalculateDir(inRange[i].transform.position, controller.transform.position);
            HitWithKnockback(inRange[i], damageAmount, knockbackDir, enemyKnockback, knockbackTime, hitWhileDash);
        }
        currentAttack = null;
    }
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        // Checks tag to be player.
        if (collision.tag == "Player")
        {
            // Calculates the knockback direction, and sends the data to the EnemyCombater to knockback the player.
            if (!inRange.Contains(collision.gameObject))
            {
                inRange.Add(collision.gameObject);
            }
        }
        else
        {
            return;
        }
    }
    protected void OnTriggerExit2D(Collider2D collision)
    {
        // Checks tag to be player.
        if (collision.tag == "Player")
        {
            // Calculates the knockback direction, and sends the data to the EnemyCombater to knockback the player.
            if (inRange.Contains(collision.gameObject))
            {
                inRange.Remove(collision.gameObject);
            }
            if (currentAttack != null)
            {
                StopCoroutine(currentAttack);
                currentAttack = null;
            }
        }
        else
        {
            return;
        }
    }
}
