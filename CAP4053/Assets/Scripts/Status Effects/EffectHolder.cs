using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EffectHolder : MonoBehaviour
{
    [SerializeField] private List<Effect> effectList = new List<Effect>();
    public void AddEffect(Effect newEffect)
    {
        if (effectList != null)
        {
            effectList.Add(newEffect);
            newEffect.ApplyEffect(this);
        }
    }
    public void RemoveEffectFromList(int index)
    {
        try
        {
            if (effectList != null)
            {
                effectList.RemoveAt(index);
            }
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
    public void RemoveAllEffects()
    {
        for (int i = 0; i < effectList.Count; i++)
        {
            effectList[i].RemoveEffect(this);
        }
    }
    public void DisableEffects()
    {
        effectList = null;
    }
    public void StartEffectCountdown(Effect effect, float countdownTime)
    {
        StartCoroutine(EffectCountdown(effect, countdownTime));
    }
    private IEnumerator EffectCountdown(Effect effect, float countdownTime)
    {
        yield return new WaitForSeconds(countdownTime);
        if (effectList.Contains(effect))
        {
            effect.RemoveEffect(this);
        }
    }
}
