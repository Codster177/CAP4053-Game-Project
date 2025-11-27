using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatManager : MonoBehaviour
{
    public List<EnemyStatController> enemyStatControllers = new List<EnemyStatController>() {
        new ChaserStatController()};

    public void SetAllStartingStats()
    {
        for (int i = 0; i < enemyStatControllers.Count; i++)
        {
            enemyStatControllers[i].SetStartingStats();
        }
    }
    public void IncreaseAllStats()
    {
        for (int i = 0; i < enemyStatControllers.Count; i++)
        {
            enemyStatControllers[i].UpdateStats();
        }
    }
}

[Serializable]
public class ChaserStatController : EnemyStatController
{
    public ChaserStatController() : base()
    {
        this.enemyName = "Chaser";
        this.floatStats = new List<EnemyStat<float>>() {
            new EnemyStat<float>("baseDamage"), 
            new EnemyStat<float>("baseAttackSpeed")};
    }
    public void SetStartingStats()
    {
        base.SetStartingStats();
        EnemyController controller = prefab.GetComponent<EnemyController>();

        ChaserCombater combater = controller.GetCombater() as ChaserCombater;
        EnemyStat<float> baseDamage = GetFloatStat("baseDamage");
        EnemyStat<float> baseAttackSpeed = GetFloatStat("baseAttackSpeed");
        baseDamage.Awake();
        baseAttackSpeed.Awake();
        combater.SetEnemyStats(baseDamage.startStat, baseAttackSpeed.startStat);
    }

    public void UpdateStats()
    {
        base.UpdateStats();
        EnemyController controller = prefab.GetComponent<EnemyController>();

        ChaserCombater combater = controller.GetCombater() as ChaserCombater;
        EnemyStat<float> baseDamage = GetFloatStat("baseDamage");
        EnemyStat<float> baseAttackSpeed = GetFloatStat("baseAttackSpeed");

        baseDamage.currentStat = baseDamage.currentStat * baseDamage.updateStatBy;
        baseAttackSpeed.currentStat = baseAttackSpeed.currentStat * baseAttackSpeed.updateStatBy;

        combater.SetEnemyStats(baseDamage.currentStat, baseAttackSpeed.currentStat);
    }
}

[Serializable]
public class EnemyStatController
{
    public string enemyName;
    public GameObject prefab;
    public EnemyStat<int> health;
    public List<EnemyStat<float>> floatStats;

    public EnemyStatController()
    {
        this.health = new EnemyStat<int>("health");
    }
    public void SetStartingStats()
    {
        EnemyHealth enemyHealth = prefab.GetComponent<EnemyHealth>();

        health.Awake();
        enemyHealth.SetMaxHP(health.startStat);
    }
    public void UpdateStats()
    {
        EnemyHealth enemyHealth = prefab.GetComponent<EnemyHealth>();

        health.currentStat = ((int)(health.currentStat * health.updateStatBy));
        enemyHealth.SetMaxHP(health.currentStat);
    }
    public EnemyStat<float> GetFloatStat(string statName)
    {
        for (int i = 0; i < floatStats.Count; i++)
        {
            if (floatStats[i].statName == statName)
            {
                return floatStats[i];
            }
        }
        return null;
    }
}

[Serializable]
public class EnemyStat<T>
{
    public string statName;
    public T currentStat;
    public T startStat;
    public float updateStatBy;

    public EnemyStat(string statName)
    {
        this.statName = statName;
    }
    public void Awake()
    {
        this.currentStat = this.startStat;
    }
}