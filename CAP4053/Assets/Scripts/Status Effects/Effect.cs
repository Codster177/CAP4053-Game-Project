using System;
using System.Collections.Generic;
using UnityEditor;
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

public class Stun : Effect
{
    public static new EffectType effectType = EffectType.Stun;
    private Action<bool> stopMovement;
    private Action<bool> stopAttacks;

    public Stun(float setTime) : base(setTime)
    {
        return;
    }
    public override void ApplyEffect(EffectHolder effectHolder)
    {
        base.ApplyEffect(effectHolder);
        GetStopFunctions(effectHolder);
        stopMovement(false);
        stopAttacks(false);
    }
    public override void RemoveEffect(EffectHolder effectHolder)
    {
        base.RemoveEffect(effectHolder);
        stopMovement(true);
        stopAttacks(true);
    }

    private void GetStopFunctions(EffectHolder effectHolder)
    {
        if (effectHolder.gameObject.tag == "Player")
        {
            PlayerController playerController = null;
            playerController = effectHolder.GetComponent<PlayerController>();
            if (playerController == null)
            {
                Debug.LogError($"{effectHolder.gameObject.name}: Invincibility effect cannot find playerController.");
            }
            stopMovement = playerController.AllowMovement;
            stopAttacks = playerController.SetCanAttack;
        }
        else if (effectHolder.gameObject.tag == "Enemy")
        {
            EnemyController enemyController = null;
            enemyController = effectHolder.GetComponent<EnemyController>();
            if (enemyController == null)
            {
                Debug.LogError($"{effectHolder.gameObject.name}: Invincibility effect cannot find enemyHealth.");
            }
            stopMovement = enemyController.Allowmovement;
            stopAttacks = enemyController.SetCanAttack;
        }
        else
        {
            Debug.LogError($"{effectHolder.gameObject.name}: Invincibility cannot be applied onto this object.");
            return;
        }
    }
}
public class Knockback : Stun
{
    public static new EffectType effectType = EffectType.Knockback;
    private Rigidbody2D rigidbody = null;
    private Action<bool> stopMovement;
    private Action<bool> stopAttacks;
    private Vector3 direction;
    private float speed;

    public Knockback(float setTime, float speed, Vector3 direction) : base(setTime)
    {
        this.speed = speed;
        this.direction = direction;
        return;
    }
    public override void ApplyEffect(EffectHolder effectHolder)
    {
        base.ApplyEffect(effectHolder);
        rigidbody = effectHolder.GetComponent<Rigidbody2D>();
        if (rigidbody == null)
        {
            Debug.LogError($"{effectHolder.gameObject.name}: Knockback effect cannot find rigidbody.");
        }
        rigidbody.linearVelocity = direction * speed;
    }
    public override void RemoveEffect(EffectHolder effectHolder)
    {
        base.RemoveEffect(effectHolder);
        if (rigidbody != null)
        {
            rigidbody.linearVelocity = new Vector3(0f, 0f, 0f);
        }
    }
    public static Vector2 CalculateDir(Vector2 attackerPos, Vector2 victimPos)
    {
        Vector2 knockbackDir = attackerPos - victimPos;
        knockbackDir.Normalize();
        return knockbackDir;
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
    Stun,
    Knockback,
    Invincibility
}