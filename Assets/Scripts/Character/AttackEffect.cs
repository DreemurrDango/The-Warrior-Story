using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

/// <summary>
/// 发出的攻击效果判定组件
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class AttackEffect : MonoBehaviour
{
    [Tooltip("攻击类型")]
    public AttackType attackType;
    [Tooltip("造成的伤害类型")]
    public DamageType damageType;
    [Tooltip("基础伤害值")]
    public float baseDamageValue;
    [Tooltip("攻击来源角色，置为空时为无差别攻击，可伤害发出者")]
    public CharaBase sourceChara;

    /// <summary>
    /// 攻击判定区域
    /// </summary>
    private Collider2D attackAreaTrigger;
}
