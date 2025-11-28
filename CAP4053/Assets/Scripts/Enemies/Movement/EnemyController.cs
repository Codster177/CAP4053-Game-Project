using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] protected EnemyCombater enemyCombater;
    protected bool movementEnabled = true;
    protected NavMeshAgent navMeshAgent;
    protected SpriteRenderer spriteRenderer;

    // Sets the enemy controller up with the NavMesh and needed parameters.
    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;

        spriteRenderer = GetComponent<SpriteRenderer>();

        enemyCombater.SetController(this);
    }
    void Start()
    {
        GameManager.OnGameStateChanged += DeathEnemyCommand;
    }
    void Update()
    {
        if (GetDirection().x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (GetDirection().x > 0)
        {
            spriteRenderer.flipX = false;
        }
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
    void DeathEnemyCommand(GameState newGameState)
    {
        if (newGameState == GameState.Death)
        {
            navMeshAgent.SetDestination(transform.position);
        }
    }
}