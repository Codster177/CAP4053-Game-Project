using System.Collections;
using NUnit.Framework;
using UnityEngine;

public class EnemyCombater : MonoBehaviour
{
    protected EnemyController controller;
    private bool isBeingHit = false;

    public void SetController(EnemyController newController)
    {
        controller = newController;
    }
    protected void HitWithKnockback(GameObject EntityGO, float damageAmount, Vector2 knockbackDir, float knockbackTime, bool hitWhileDash)
    {
        if (isBeingHit)
        {
            return;
        }
        Rigidbody2D playerRB = EntityGO.GetComponent<Rigidbody2D>();
        PlayerController playerCon = EntityGO.GetComponent<PlayerController>();

        if (!playerCon.CanEnemyHit(hitWhileDash))
        {
            return;
        }

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
