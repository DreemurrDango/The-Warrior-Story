using Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可被攻击的对象判定组件
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class BeAttackable : MonoBehaviour
{
    public Action<CharaBase,AttackType,DamageType,float> OnBeAttack;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        AttackEffect attackEffect = collision.GetComponent<AttackEffect>();
        if (attackEffect != null)
        {
            CharaBase sourceChara = attackEffect.sourceChara;
            AttackType attackType = attackEffect.attackType;
            DamageType damageType = attackEffect.damageType;
            float damageValue = attackEffect.baseDamageValue;
            OnBeAttack?.Invoke(sourceChara, attackType, damageType, damageValue);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        
    }
}
