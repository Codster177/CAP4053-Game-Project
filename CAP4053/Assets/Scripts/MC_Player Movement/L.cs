using System.Collections.Generic;
using UnityEngine;

public class L : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (!other.CompareTag("Enemy")) return;

        EnemyHealth health = other.GetComponentInParent<EnemyHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
            Debug.Log("Dealt damage to " + other.name);
        }
    }
}
