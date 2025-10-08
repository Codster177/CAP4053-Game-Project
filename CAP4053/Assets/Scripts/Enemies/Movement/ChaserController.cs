using UnityEngine;
using UnityEngine.AI;

public class ChaserController : EnemyController
{

    // Listens to when enemy death is called.
    void Start()
    {
        GameManager.OnGameStateChanged += DeathEnemyCommand;
    }

    public void SetPosition(bool goToPlayer, Vector3 alternatePosition)
    {
        if (!movementEnabled)
        {
            return;
        }
        if (goToPlayer)
        {
            navMeshAgent.SetDestination(GameManager.publicGameManager.GetPlayerLocation().position);
        }
        else
        {
            navMeshAgent.SetDestination(alternatePosition);
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
