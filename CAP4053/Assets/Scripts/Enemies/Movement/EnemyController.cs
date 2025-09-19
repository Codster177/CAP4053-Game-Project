using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] protected EnemyCombater enemyCombater;
    protected NavMeshAgent navMeshAgent;
    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        enemyCombater.SetController(this);
    }

    public Vector2 GetDirection()
    {
        return navMeshAgent.velocity;
    }
}
