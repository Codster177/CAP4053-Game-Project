using System.Collections;
using NUnit.Framework;
using UnityEngine;

public class EnemyCombater : MonoBehaviour
{
    protected EnemyController controller;
    private bool isBeingHit = false;

    // Sets the controller that the combater uses to a specific controller.
    public void SetController(EnemyController newController)
    {
        controller = newController;
    }

    // Hits the player, knocks them back, and does damage to them.
    protected void HitWithKnockback(GameObject EntityGO, float damageAmount, Vector2 knockbackDir, float knockbackTime, bool hitWhileDash)
    {
        // Checks if the player is being hit. Returns if the player is currently being hit.
        if (isBeingHit)
        {
            return;
        }
        // Gets player componenets.
        Rigidbody2D playerRB = EntityGO.GetComponent<Rigidbody2D>();
        PlayerController playerCon = EntityGO.GetComponent<PlayerController>();

        // Checks if the player is in a condition where they should not be hit. Returns if they can't be hit.
        if (!playerCon.CanEnemyHit(hitWhileDash))
        {
            return;
        }

        // Deals damage, knocks the player back, and starts the coroutine to reset the player to normal.
        GameManager.publicGameManager.DealDamage(damageAmount);
        isBeingHit = true;
        StartCoroutine(KnockbackCoroutine(playerRB, playerCon, knockbackTime));
        Debug.Log(knockbackDir);
        playerRB.AddForce(knockbackDir, ForceMode2D.Impulse);
    }
    private IEnumerator KnockbackCoroutine(Rigidbody2D playerRB, PlayerController playerCon, float knockbackTime)
    {
        playerCon.AllowMovement(false);
        yield return new WaitForSeconds(knockbackTime);
        if (GameManager.publicGameManager.GetGameState() != GameState.Death)
        {
            playerCon.AllowMovement(true);
        }
        isBeingHit = false;
        playerRB.linearVelocity = new Vector2(0f, 0f);
    }
}
