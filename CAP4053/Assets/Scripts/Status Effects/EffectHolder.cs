using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EffectHolder : MonoBehaviour
{
    [SerializeField] private List<Effect> effectList;
    public void AddEffect(Effect newEffect)
    {
        effectList.Add(newEffect);
        newEffect.ApplyEffect(this);
    }
    public void RemoveEffectFromList(int index)
    {
        try
        {
            effectList.RemoveAt(index);
        }
        catch (IndexOutOfRangeException)
        {
            Debug.LogError($"{gameObject.name}: EffectHolder: effectList index out of bounds.");
            throw;
        }
    }
    public bool RemoveEffect(EffectType effectType)
    {
        for (int i = 0; i < effectList.Count; i++)
        {
            if (effectList[i].GetEffectType() == effectType)
            {
                effectList[i].RemoveEffect(this);
                return true;
            }
        }
        return false;
    }
    public void StartEffectCountdown(Effect effect, float countdownTime)
    {
        StartCoroutine(EffectCountdown(effect, countdownTime));
    }
    private IEnumerator EffectCountdown(Effect effect, float countdownTime)
    {
        yield return new WaitForSeconds(countdownTime);
        effect.RemoveEffect(this);
    }
}
