using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager publicEnemyManager;
    [SerializeField] private EnemyStatManager enemyStatManager;
    private List<GameObject> enemyList = new List<GameObject>();

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
        enemyStatManager.SetAllStartingStats();
    }
    public void IncreaseEnemyDifficulty()
    {
        enemyStatManager.IncreaseAllStats();
    }
}