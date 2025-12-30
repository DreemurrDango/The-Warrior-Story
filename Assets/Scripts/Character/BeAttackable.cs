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
    /// <summary>
    /// 受到攻击时触发的事件
    /// </summary>
    public event Action<CharaBase,AttackType,DamageType,float> OnBeAttack;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log($"{transform.parent.name}的受击盒检测到{collision.name}进入");
        var ae = collision.GetComponent<AttackEffect>();
        if (ae == null) return;
        OnBeAttack?.Invoke(ae.sourceChara, ae.attackType, ae.damageType, ae.baseDamageValue);
    }
}
