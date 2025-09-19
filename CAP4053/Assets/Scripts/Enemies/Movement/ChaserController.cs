using UnityEngine;
using UnityEngine.AI;

public class ChaserController : EnemyController
{
    void Start()
    {
        GameManager.OnGameStateChanged += DeathEnemyCommand;
    }
    void Update()
    {
        if (GameManager.publicGameManager.GetGameState() != GameState.Death)
        {
            navMeshAgent.SetDestination(GameManager.publicGameManager.GetPlayerLocation());
        }
    }
    void DeathEnemyCommand(GameState newGameState)
    {
        if (newGameState == GameState.Death)
        {
            navMeshAgent.SetDestination(transform.position);
        }
    }
}
