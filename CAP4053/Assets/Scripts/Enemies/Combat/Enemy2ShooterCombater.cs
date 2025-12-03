using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2ShooterCombater : EnemyCombater
{
    [Header("Contact Damage Settings")]
    [SerializeField] protected float damageAmount = 1f;
    [SerializeField] protected float enemyKnockback = 5f;
    [SerializeField] protected float knockbackTime = 0.2f;
    [SerializeField] protected bool hitWhileDash = false;

    // We do NOT need to look for ChaserController here, avoiding the crash.

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        // If the player physically touches the enemy, hurt them
        if (collision.CompareTag("Player"))
        {
            AttackPlayer(collision.gameObject);
        }
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        // Backup in case you use Physics Colliders instead of Triggers
        if (collision.gameObject.CompareTag("Player"))
        {
            AttackPlayer(collision.gameObject);
        }
    }

    private void AttackPlayer(GameObject player)
    {
        // Calculate knockback direction away from the enemy
        Vector2 knockbackDir = Knockback.CalculateDir(player.transform.position, transform.position);

        // This function exists in your base EnemyCombater class
        HitWithKnockback(player, damageAmount, knockbackDir, enemyKnockback, knockbackTime, hitWhileDash);
    }

    // REQUIRED: These satisfy the requirements of the parent "EnemyCombater" script
    public List<float> GetEnemyStats()
    {
        // Return damage, and 0 for attack speed since the Controller handles shooting speed
        return new List<float>() { damageAmount, 0f };
    }

    public void SetEnemyStats(float damageAmount, float attackSpeed)
    {
        this.damageAmount = damageAmount;
        // We ignore attackSpeed here because ShooterController handles the fire rate
    }
}