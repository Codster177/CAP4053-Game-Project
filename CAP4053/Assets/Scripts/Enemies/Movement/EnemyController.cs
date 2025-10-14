using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] protected EnemyCombater enemyCombater;
    protected bool movementEnabled = true;
    protected NavMeshAgent navMeshAgent;

    // Sets the enemy controller up with the NavMesh and needed parameters.
    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        enemyCombater.SetController(this);
    }
    // Returns the velocity of the enemy.
    public Vector2 GetDirection()
    {
        return navMeshAgent.velocity;
    }
    public void Allowmovement(bool movementBool)
    {
        navMeshAgent.enabled = movementBool;
        movementEnabled = movementBool;
    }
    public void SetCanAttack(bool newState)
    {
        enemyCombater.SetCanAttack(newState);
    }
    public EnemyCombater GetCombater()
    {
        return enemyCombater;
    }
}
