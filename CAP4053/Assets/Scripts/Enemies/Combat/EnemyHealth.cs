using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 30;
    private EnemyController enemyController;
    private bool canBeHit = true;
    private int currentHP;

    //starts enemy at max hp (duh)
    void Awake()
    {
        currentHP = maxHP;
        //added this so enemies could be attacked again
        if (enemyController == null)
        {
            enemyController = GetComponent<EnemyController>();
        }
    }

    public void SetEnemyController(EnemyController newEnemyController)
    {
        enemyController = newEnemyController;
    }
    public void SetMaxHP(int newMax)
    {
        maxHP = newMax;
    }
    public int GetMaxHP()
    {
        return maxHP;
    }
    public int GetCurrentHP()
    {
        return currentHP;
    }
    public void TakeDamage(int amount)
    {
        if (!canBeHit)
        {
            return;
        }
        enemyController.RecieveDamage();
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
        EnemyManager.publicEnemyManager.RemoveEnemy(gameObject);
        Destroy(gameObject);
    }
}
