using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ChaserCombater : EnemyCombater
{
    [SerializeField] private float damageAmount, enemyKnockback, knockbackTime;
    [SerializeField] private bool hitWhileDash;
    // Recognizes the player when encountering them in the enemies trigger collider.
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Checks tag to be player.
        if (collision.tag == "Player")
        {
            // Calculates the knockback direction, and sends the data to the EnemyCombater to knockback the player.
            Vector2 knockbackDir = collision.transform.position - controller.transform.position;
            knockbackDir.Normalize();
            knockbackDir = knockbackDir * enemyKnockback;
            HitWithKnockback(collision.gameObject, damageAmount, knockbackDir, knockbackTime, hitWhileDash);
        }
        else
        {
            return;
        }
    }
}
