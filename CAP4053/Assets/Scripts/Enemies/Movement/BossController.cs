using System.Collections;
using UnityEngine;

public class BossController : ChaserController
{
    public EnemyAnimator enemyAnimator;

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
            return;
        }
        // Left off here ^^
    }

}