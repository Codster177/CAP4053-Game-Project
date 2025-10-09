using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private SpawnSquare[] spawnSquares;
    [SerializeField] private GameObject[] enemies;

    // public void EnterRoom()
    // {

    // }

    [Serializable]
    private class SpawnSquare
    {
        public Vector2 location;
        public Vector2 size;

    }
    void OnDrawGizmos()
    {
        for (int i = 0; i < spawnSquares.Length; i++)
        {
            Gizmos.DrawWireCube(spawnSquares[i].location, spawnSquares[i].size);
        }
    }
}
