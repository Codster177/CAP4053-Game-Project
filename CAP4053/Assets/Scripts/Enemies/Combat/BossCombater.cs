using System.Collections;
using UnityEngine;

public class BossCombater : ChaserCombater
{
    protected BossController bossController;
    [SerializeField] protected GameObject projectilePrefab;

    protected new void Start()
    {
        base.Start();
        bossController = chaserController as BossController;
    }
    protected new void Update()
    {
        DecideAttack(bossController.GetAttackState());
    }
    protected void DecideAttack(int attackState)
    {
        if (currentAttack != null)
        {
            return;
        }
        if (attackState == 1)
        {
            if (inRange.Count > 0)
            {
                currentAttack = StartCoroutine(StartChaserAttack());
            }
            else
            {
                bossController.SetPosition(true, new Vector3());
            }
        }
        if (attackState == 3)
        {
            currentAttack = StartCoroutine(StartShooterAttack());
        }
    }
    protected IEnumerator StartChaserAttack()
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

    protected IEnumerator StartShooterAttack()
    {
        bossController.enemyAnimator.ChangeAnimation("attack_2");

        while (bossController.GetAttackState() == 3)
        {
            Shoot(GameManager.publicGameManager.GetPlayerLocation().position);
            yield return new WaitForSeconds(attackSpeed / 3);
        }
    }

    private void Shoot(Vector3 targetPos)
    {
        if (projectilePrefab == null) return;

        // Determine where to spawn the bullet (use current position if no firePoint set)
        Vector3 spawnLoc = transform.position;

        // Calculate rotation towards the player
        Vector3 direction = targetPos - spawnLoc;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Instantiate(projectilePrefab, spawnLoc, rotation);
    }

}
