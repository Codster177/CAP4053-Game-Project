using UnityEngine;
using UnityEngine.AI;

public class ChaserController : EnemyController
{
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
}
