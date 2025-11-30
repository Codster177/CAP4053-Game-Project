using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyUIManager : MonoBehaviour
{
    public static MyUIManager publicUIManager;
    [SerializeField] private Animator healthbarAnimator;
    [SerializeField] private Animator dashAnimator;
    [SerializeField] private HealthbarScript healthbarScript;
    [SerializeField] private GameObject deathScreen;
    private Coroutine healthBarCoroutine = null;

    void Awake()
    {
        publicUIManager = this;
    }
    void Start()
    {
        GameManager.OnGameStateChanged += ActivateDeath;
    }
    public void HealthbarDamageAnim(float damageTaken)
    {
        float startHealth = GameManager.publicGameManager.GetPlayerHealth();
        float targetHealth = startHealth - damageTaken;
        if (healthBarCoroutine != null)
        {
            StopCoroutine(healthBarCoroutine);
            float tempHealth = healthbarScript.GetTempHealth();
            healthBarCoroutine = StartCoroutine(healthbarScript.BarTakeDamage(tempHealth, targetHealth, SetCoroutineToNull));
        }
        else
        {
            healthBarCoroutine = StartCoroutine(healthbarScript.BarTakeDamage(startHealth, targetHealth, SetCoroutineToNull));
        }
    }
    public void DashbarAnim()
    {
        dashAnimator.SetInteger("State", 1);
        StartCoroutine(DashCoroutine());
    }

    public void SetCoroutineToNull()
    {
        healthBarCoroutine = null;
    }

    private IEnumerator DashCoroutine()
    {
        yield return new WaitForSeconds(0.01f);
        dashAnimator.SetInteger("State", 0);
    }
    public void ActivateDeath(GameState state)
    {
        if (state == GameState.Death)
        {
            deathScreen.SetActive(true);
        }
    }

}
