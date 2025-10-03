using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
[Serializable]
public class Effect
{
    public static EffectType effectType = EffectType.NullType;
    protected int effectId;
    protected float effectTime;
    public Effect(float setTime = -1)
    {
        effectTime = setTime;
    }
    public virtual void ApplyEffect(EffectHolder effectHolder)
    {
        if (effectTime != -1)
        {
            effectHolder.StartEffectCountdown(this, effectTime);
        }
        return;
    }
    public virtual void RemoveEffect(EffectHolder effectHolder)
    {
        effectHolder.RemoveEffectFromList(effectId);
        return;
    }
    public void setId(int id)
    {
        effectId = id;
    }
    public EffectType GetEffectType()
    {
        return effectType;
    }
}
public class Knockback : Effect
{
    public static new EffectType effectType = EffectType.Knockback;
    Vector3 direction = new Vector3();

    public Knockback(float setTime) : base(setTime)
    {
        return;
    }
    public override void ApplyEffect(EffectHolder effectHolder)
    {
        base.ApplyEffect(effectHolder);
        Rigidbody2D rigidbody = null;
        rigidbody = effectHolder.GetComponent<Rigidbody2D>();
        if (rigidbody == null)
        {
            Debug.LogError($"{effectHolder.gameObject.name}: Knockback effect cannot find rigidbody.");
        }
    }
}

public class Invincibility : Effect
{
    public static new EffectType effectType = EffectType.Invincibility;
    private Action<bool> setCanHit = null;
    public Invincibility(float setTime) : base(setTime)
    {
        return;
    }
    public override void ApplyEffect(EffectHolder effectHolder)
    {
        base.ApplyEffect(effectHolder);
        GetObjectType(effectHolder);
        if (setCanHit != null)
        {
            setCanHit(false);
        }
    }
    public override void RemoveEffect(EffectHolder effectHolder)
    {
        base.RemoveEffect(effectHolder);
        if (setCanHit != null)
        {
            setCanHit(true);
        }
    }
    private void GetObjectType(EffectHolder effectHolder)
    {
        if (effectHolder.gameObject.tag == "Player")
        {
            PlayerController playerController = null;
            playerController = effectHolder.GetComponent<PlayerController>();
            if (playerController == null)
            {
                Debug.LogError($"{effectHolder.gameObject.name}: Invincibility effect cannot find playerController.");
            }
            setCanHit = playerController.SetCanBeHit;
        }
        else if (effectHolder.gameObject.tag == "Enemy")
        {
            EnemyHealth enemyHealth = null;
            enemyHealth = effectHolder.GetComponent<EnemyHealth>();
            if (enemyHealth == null)
            {
                Debug.LogError($"{effectHolder.gameObject.name}: Invincibility effect cannot find enemyHealth.");
            }
            setCanHit = enemyHealth.SetCanBeHit;
        }
        else
        {
            Debug.LogError($"{effectHolder.gameObject.name}: Invincibility cannot be applied onto this object.");
            return;
        }
    }
}
public enum EffectType
{
    NullType,
    Knockback,
    Invincibility
}