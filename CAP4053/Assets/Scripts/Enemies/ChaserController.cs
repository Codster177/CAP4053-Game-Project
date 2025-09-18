using UnityEngine;
using UnityEngine.AI;

public class ChaserController : MonoBehaviour
{
    [SerializeField] private Transform playerTrans;
    [SerializeField] private NavMeshAgent navMeshAgent;
    void Awake()
    {
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
    }
    void Update()
    {
        navMeshAgent.SetDestination(playerTrans.position);
    }
}
