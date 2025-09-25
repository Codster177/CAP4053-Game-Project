using UnityEngine;
using UnityEngine.AI;

public class ChaserController : EnemyController
{
    // Listens to when enemy death is called.
    void Start()
    {
        GameManager.OnGameStateChanged += DeathEnemyCommand;
    }
    // Constantly sets the chaser enemy to pursue the player.
    void Update()
    {
        if (GameManager.publicGameManager.GetGameState() != GameState.Death)
        {
            navMeshAgent.SetDestination(GameManager.publicGameManager.GetPlayerLocation());
        }
    }
    // Called OnGameStateChanged. Sets location to current location once player is dead.
    void DeathEnemyCommand(GameState newGameState)
    {
        if (newGameState == GameState.Death)
        {
            navMeshAgent.SetDestination(transform.position);
        }
    }
}
