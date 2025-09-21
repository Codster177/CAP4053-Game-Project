using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ChaserCombater : EnemyCombater
{
    [SerializeField] private float damageAmount, enemyKnockback, knockbackTime;
    [SerializeField] private bool hitWhileDash;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
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
