using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager publicEnemyManager;
    private List<GameObject> enemyList = new List<GameObject>();

    // Temp:
    private int baseHealth = 30;
    private float baseDamage = 5, baseAttackSpeed = 1;

    void Awake()
    {
        publicEnemyManager = this;
    }
    void Start()
    {
        SetEnemyDifficulty();
    }
    public void AddToList(GameObject enemy)
    {
        enemyList.Add(enemy);
    }
    public void RemoveEnemy(GameObject enemy)
    {
        enemyList.Remove(enemy);
    }
    public List<GameObject> GetEnemyList()
    {
        return enemyList;
    }
    public void ClearList()
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i] != null)
            {
                Destroy(enemyList[i].gameObject);
            }
        }
        enemyList.Clear();
        // System.GC.Collect();
    }
    private void SetEnemyDifficulty()
    {
        GameObject enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemies/Enemy 1_ Chaser.prefab");
        EnemyHealth health = enemyPrefab.GetComponent<EnemyHealth>();
        EnemyController controller = enemyPrefab.GetComponent<EnemyController>();
        ChaserCombater combater = controller.GetCombater() as ChaserCombater;

        health.SetMaxHP(baseHealth);
        combater.SetEnemyStats(baseDamage, baseAttackSpeed);
    }
    public void IncreaseEnemyDifficulty()
    {
        GameObject enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemies/Enemy 1_ Chaser.prefab");
        EnemyHealth health = enemyPrefab.GetComponent<EnemyHealth>();
        EnemyController controller = enemyPrefab.GetComponent<EnemyController>();
        ChaserCombater combater = controller.GetCombater() as ChaserCombater;

        health.SetMaxHP((int)(health.GetMaxHP() * 1.1));
        List<float> combatStats = combater.GetEnemyStats();
        combater.SetEnemyStats(combatStats[0] * 1.1f, combatStats[1] * 0.95f);
    }
}
