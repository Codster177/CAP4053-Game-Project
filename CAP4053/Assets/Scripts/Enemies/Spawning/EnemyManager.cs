using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager publicEnemyManager;
    private List<GameObject> enemyList = new List<GameObject>();

    void Awake()
    {
        publicEnemyManager = this;
    }
    public void AddToList(GameObject enemy)
    {
        enemyList.Add(enemy);
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
    }
}
