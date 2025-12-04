using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] protected EnemyCombater enemyCombater;
    protected bool movementEnabled = true;
    protected NavMeshAgent navMeshAgent;
    protected SpriteRenderer spriteRenderer;
    protected EnemyHealth enemyHealth;

    // Sets the enemy controller up with the NavMesh and needed parameters.
    protected void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        //checks for navmesh
        if (navMeshAgent != null)
        {
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();

        //looks for combater
        if (enemyCombater == null)
        {
            enemyCombater = GetComponent<EnemyCombater>();
        }

        if (enemyHealth == null)
        {
            enemyHealth = GetComponent<EnemyHealth>();
        }

        //sets up combater
        if (enemyCombater != null)
        {
            enemyCombater.SetController(this);
        }
    }
    protected void Start()
    {
        GameManager.OnGameStateChanged += DeathEnemyCommand;
    }
    protected void Update()
    {
        if (GetDirection().x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (GetDirection().x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            Vector3 playerPos = GameManager.publicGameManager.GetPlayerLocation().position;
            if (transform.position.x > playerPos.x)
            {
                spriteRenderer.flipX = true;
            }
            else if (transform.position.x < playerPos.x)
            {
                spriteRenderer.flipX = false;
            }
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
    public virtual void RecieveDamage()
    {
        return;
    }
    protected void DeathEnemyCommand(GameState newGameState)
    {
        if (newGameState == GameState.Death)
        {
            navMeshAgent.SetDestination(transform.position);
        }
    }

    //needed so it stops calling the script after being destroyed
    protected void OnDestroy()
    {
        if (GameManager.publicGameManager != null)
        {
            GameManager.OnGameStateChanged -= DeathEnemyCommand;
        }
    }
}