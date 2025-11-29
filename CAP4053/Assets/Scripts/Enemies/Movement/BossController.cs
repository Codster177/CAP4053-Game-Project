using UnityEngine;

public class BossController : ChaserController
{
    [SerializeField] private Animator bossAnimator;
    private BossAnimationState animationState;
    private EnemyHealth bossHealth;

    void Awake()
    {
        bossHealth = GetComponent<EnemyHealth>();   
    }

    private void DecideAction()
    {
        float playerHealth = GameManager.publicGameManager.GetPlayerHealth();
        float currentHealth = bossHealth.GetCurrentHP();

        if (playerHealth > 50 && currentHealth > 50)
        {
            return;
        }
        // Left off here ^^
    }

    public void SetAnimationState(BossAnimationState newAnimationState)
    {
        animationState = newAnimationState;
        bossAnimator.SetInteger("State", (int)animationState);
    }

}
public enum BossAnimationState
{
    Idle,
    Walking,
    Hurt,
    LightAttack,
    SpinAttack,
    LightningAttack,
    Death,
    FlowerAvailable,
    FlowerGone
}