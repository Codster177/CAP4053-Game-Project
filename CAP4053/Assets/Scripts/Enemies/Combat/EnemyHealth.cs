using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 30;
    private bool canBeHit = true;
    private int currentHP;

    //starts enemy at max hp (duh)
    void Awake() => currentHP = maxHP;

    public void TakeDamage(int amount)
    {
        if (!canBeHit)
        {
            return;
        }
        currentHP -= amount;
        Debug.Log($"took dmg and current HP: {currentHP}/{maxHP}");

        if (currentHP < 0) Die();
    }
    public void SetCanBeHit(bool allowHit)
    {
        canBeHit = allowHit;
    }

    private void Die()
    {
        Debug.Log("deaded");
        Destroy(gameObject);
    }
}
