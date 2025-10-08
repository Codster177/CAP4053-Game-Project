using System.Collections;
using NUnit.Framework;
using UnityEngine;

public class EnemyCombater : MonoBehaviour
{
    protected EnemyController controller;
    [SerializeField] protected float cooldownTime;
    protected bool isBeingHit = false, canAttack = true;

    // Sets the controller that the combater uses to a specific controller.
    public void SetController(EnemyController newController)
    {
        controller = newController;
    }
    public void SetCanAttack(bool newState)
    {
        canAttack = newState;
    }

    // Hits the player, knocks them back, and does damage to them.
    protected void HitWithKnockback(GameObject EntityGO, float damageAmount, Vector2 knockbackDir, float knockbackTime, bool hitWhileDash)
    {
        // Checks if the player is being hit. Returns if the player is currently being hit.
        Debug.Log($"isBeingHit: {isBeingHit}, canAttack: {canAttack}");
        if (isBeingHit || !canAttack)
        {
            return;
        }
        // Gets player componenets.
        EffectHolder playerEH = EntityGO.GetComponent<EffectHolder>();
        PlayerController playerCon = EntityGO.GetComponent<PlayerController>();

        // Deals damage, knocks the player back, and starts the coroutine to reset the player to normal.
        StartCoroutine(AttackCooldown());
        playerCon.DealDamageToPlayer(damageAmount, hitWhileDash);
        playerEH.AddEffect(new Knockback(knockbackTime, 20, knockbackDir));
    }

    private IEnumerator AttackCooldown()
    {
        isBeingHit = true;
        yield return new WaitForSeconds(cooldownTime);
        isBeingHit = false;
    }
}
