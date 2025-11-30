using System.Collections;
using UnityEngine;

public class BossCombater : ChaserCombater
{
    protected BossController bossController;

    protected new void Start()
    {
        base.Start();
        bossController = chaserController as BossController;
    }
    protected new void Update()
    {
        if (currentAttack == null && inRange.Count > 0)
        {
            currentAttack = StartCoroutine(StartAttack());
        }
        else
        {
            if (currentAttack == null)
            {
                bossController.SetPosition(true, new Vector3());
            }
        }
        return;
    }
    protected new IEnumerator StartAttack()
    {
        bossController.enemyAnimator.ChangeAnimation("attack_1");
        bossController.SetPosition(false, transform.position);
        yield return new WaitForSeconds(attackSpeed);
        for (int i = 0; i < inRange.Count; i++)
        {
            Vector2 knockbackDir = Knockback.CalculateDir(inRange[i].transform.position, controller.transform.position);
            HitWithKnockback(inRange[i], damageAmount, knockbackDir, enemyKnockback, knockbackTime, hitWhileDash);
        }
        currentAttack = null;
    }

}
