using System.Collections;
using UnityEngine;

public class BossController : ChaserController
{
    public EnemyAnimator enemyAnimator;
    protected int attackState = 0;
    private Vector3 randomLocation;
    new void Update()
    {
        base.Update();
        DecideAction();
    }
    public override void RecieveDamage()
    {
        Debug.Log("Damaged!");
        enemyAnimator.ChangeAnimation("hurt", true);
        StartCoroutine(RecieveDamageWait());
    }
    public int GetAttackState()
    {
        return attackState;
    }
    private bool CheckHurtAnimation()
    {
        return enemyAnimator.CheckCurrentAnimation() != "hurt";
    }
    private IEnumerator RecieveDamageWait()
    {
        enemyCombater.SetCanAttack(false);
        yield return new WaitUntil(CheckHurtAnimation);
        enemyCombater.SetCanAttack(true);
    }
    private void DecideAction()
    {
        float playerHealth = GameManager.publicGameManager.GetPlayerHealth();
        float currentHealth = enemyHealth.GetCurrentHP();

        if (GetDirection().x != 0 || GetDirection().y != 0)
        {
            enemyAnimator.ChangeAnimation("walking");
        }
        else
        {
            enemyAnimator.ChangeAnimation("idle");
        }

        if (playerHealth > 50 && currentHealth > 50)
        {
            attackState = 1;
        }
        else if (currentHealth <= 50 && attackState != 2 && attackState != 3)
        {
            attackState = 2;
        }

        if (attackState == 2)
        {
            if (Vector3.Distance(transform.position, GameManager.publicGameManager.GetPlayerLocation().position) > 14f)
            {
                SetPosition(true, new Vector3());
            }
            else
            {
                SetPosition(false, transform.position);
                StartCoroutine(SecondAttackCoroutine());
            }
        }

        Debug.Log($"Attack State: {attackState}");
        // Left off here ^^
    }
    private IEnumerator SecondAttackCoroutine()
    {
        attackState = 3;
        yield return new WaitForSeconds(1.3f);
        attackState = 2;
    }

    public new void SetPosition(bool goToPlayer, Vector3 alternatePosition)
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