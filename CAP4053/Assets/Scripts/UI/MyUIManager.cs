using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MyUIManager : MonoBehaviour
{
    public static MyUIManager publicUIManager;
    [SerializeField] private Animator healthbarAnimator;
    [SerializeField] private Animator dashAnimator;
    [SerializeField] private HealthbarScript healthbarScript;
    private Coroutine healthBarCoroutine = null;

    void Start()
    {
        publicUIManager = this;
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
        Debug.Log("Dashbar Animation!");
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

}
