using UnityEngine;

public class ShooterController : EnemyController
{
    [Header("Shooting Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float attackRange = 6f; // How close to get before stopping
    [SerializeField] private float timeBetweenShots = 1.5f;
    [SerializeField] private Transform firePoint; // Optional: Where the bullet spawns (gun tip)

    private float nextShotTime;

    // We use 'new' to hide the parent Update so we can add our own logic
    new void Update()
    {
        if (!movementEnabled) return;

        // Get Player location from your GameManager
        Transform player = GameManager.publicGameManager.GetPlayerLocation();
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            // STATE 1: CLOSE ENOUGH - STOP AND SHOOT

            // Stop moving
            navMeshAgent.ResetPath();
            navMeshAgent.velocity = Vector2.zero;

            // Handle facing direction manually since we aren't moving
            FacePlayer(player.position);

            // Shoot if cooldown is ready
            if (Time.time >= nextShotTime)
            {
                Shoot(player.position);
                nextShotTime = Time.time + timeBetweenShots;
            }
        }
        else
        {
            // STATE 2: TOO FAR - CHASE PLAYER

            navMeshAgent.SetDestination(player.position);

            // Call the base Update to handle the sprite flipping while moving
            base.Update();
        }
    }

    private void FacePlayer(Vector3 playerPos)
    {
        // Simple flip logic based on X position
        if (playerPos.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    private void Shoot(Vector3 targetPos)
    {
        if (projectilePrefab == null) return;

        // Determine where to spawn the bullet (use current position if no firePoint set)
        Vector3 spawnLoc = firePoint != null ? firePoint.position : transform.position;

        // Calculate rotation towards the player
        Vector3 direction = targetPos - spawnLoc;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Instantiate(projectilePrefab, spawnLoc, rotation);
    }
}