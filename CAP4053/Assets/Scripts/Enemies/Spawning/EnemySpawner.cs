using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private SpawnSquare[] spawnSquares;
    [SerializeField] private EnemyProb[] enemies;
    [SerializeField] private int amountToSpawn;
    void Start()
    {
        if (enemies.Length == 0)
        {
            Destroy(this);
        }
        for (int i = 0; i < spawnSquares.Length; i++)
        {
            spawnSquares[i].AdjustToPosition(transform.position);
        }
    }
    public void SpawnEnemies()
    {
        for (int i = 0; i < amountToSpawn; i++)
        {
            SpawnEnemy();
        }
    }
    private void SpawnEnemy()
    {
        if (spawnSquares.Length == 0)
        {
            return;
        }
        SpawnSquare square = spawnSquares[UnityEngine.Random.Range(0, spawnSquares.Length)];
        if ((square.size.x == 0) || (square.size.y == 0))
        {
            return;
        }
        GameObject enemy = CalculateSpawnChance();
        GameObject spawnedEnemy = Instantiate(enemy, square.GetRandomPosition(), quaternion.identity);
        EnemyManager.publicEnemyManager.AddToList(spawnedEnemy);
    }
    private GameObject CalculateSpawnChance()
    {
        if (enemies.Length == 0)
        {
            return null;
        }
        if (enemies.Length == 1)
        {
            return enemies[0].enemyPrefab;
        }
        List<EnemyProb> calculatedProbabilties = new List<EnemyProb>(enemies);
        float finalTotal = 0;
        for (int i = 0; i < calculatedProbabilties.Count; i++)
        {
            if (calculatedProbabilties[i].spawnChance == 0f)
            {
                calculatedProbabilties.RemoveAt(i);
            }
            else
            {
                calculatedProbabilties[i].spawnChance = finalTotal + calculatedProbabilties[i].spawnChance;
                finalTotal = calculatedProbabilties[i].spawnChance;
            }
        }
        float randomVal = UnityEngine.Random.Range(0f, finalTotal);
        for (int i = 0; i < calculatedProbabilties.Count; i++)
        {
            if (randomVal < calculatedProbabilties[i].spawnChance)
            {
                return calculatedProbabilties[i].enemyPrefab;
            }
        }
        return null;
    }
    void OnDrawGizmos()
    {
        for (int i = 0; i < spawnSquares.Length; i++)
        {
            Gizmos.DrawWireCube(spawnSquares[i].location, spawnSquares[i].size);
        }
    }
    [Serializable]
    private class SpawnSquare
    {
        public Vector2 location;
        public Vector2 size;
        public Vector3 GetRandomPosition()
        {
            float minX = location.x - (size.x / 2);
            float maxX = location.x + (size.x / 2);
            float minY = location.y - (size.y / 2);
            float maxY = location.y + (size.y / 2);
            float randomX = UnityEngine.Random.Range(minX, maxX);
            float randomY = UnityEngine.Random.Range(minY, maxY);
            return new Vector3(randomX, randomY, 0f);
        }
        public void AdjustToPosition(Vector3 parentPosition)
        {
            location += new Vector2(parentPosition.x, parentPosition.y);
        }
    }
    [Serializable]
    private class EnemyProb
    {
        public GameObject enemyPrefab;
        public float spawnChance;
    }
}
