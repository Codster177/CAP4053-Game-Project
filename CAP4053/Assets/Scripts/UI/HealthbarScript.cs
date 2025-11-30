using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class HealthbarScript : MonoBehaviour
{
    [SerializeField] private Animator healthBarAnim;
    [SerializeField] private RectTransform healthBarTransform;
    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private float animationTime;
    private float currentTime, tempHealth;
    void Start()
    {
        tempHealth = GameManager.publicGameManager.GetPlayerHealth();
    }

    public IEnumerator BarTakeDamage(float curHealth, float targetHealth, Action setCoroutineToNull)
    {
        healthBarAnim.SetInteger("State", 1);
        float curLocation = ConvertHealthToLoc(curHealth);
        float targetLocation = ConvertHealthToLoc(targetHealth);
        float distance = curLocation - targetLocation;
        currentTime = 0f;
        while (currentTime < animationTime)
        {
            if (currentTime != 0f)
            {
                healthBarAnim.SetInteger("State", 0);
            }
            currentTime += Time.deltaTime;
            float curveVal = animationCurve.Evaluate(currentTime);
            float newLocation = curLocation - (distance * curveVal);
            tempHealth = ConvertLocToHealth(newLocation);
            healthBarTransform.anchoredPosition = new Vector2(newLocation, healthBarTransform.anchoredPosition.y);
            yield return null;
        }
        setCoroutineToNull();
    }
    public float GetTempHealth()
    {
        return tempHealth;
    }

    private float ConvertHealthToLoc(float health)
    {
        return (health * 5.4f) - 540f;
    }
    private float ConvertLocToHealth(float location)
    {
        return (location + 540f) / 5.4f;
    }
    private float ConvertLocToCurve(float location)
    {
        return (location + 540f) / 540f;
    }
    private float ConvertCurveToLoc(float curveVal)
    {
        return (curveVal * 540f) - 540f;
    }
}
