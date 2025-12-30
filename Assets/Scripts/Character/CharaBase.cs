using Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色基础类
/// </summary>
public abstract class CharaBase : MonoBehaviour
{
    [Header("受击")]
    [SerializeField]
    [Tooltip("当前生命值")]
    protected float health = 100f;
    [SerializeField]
    [Tooltip("受击组件")]
    protected BeAttackable beAttackable;

    [Header("移动")]
    [SerializeField]
    [Tooltip("行走速度")]
    protected float walkSpeed = 5f;
    [SerializeField]
    [Tooltip("奔跑速度")]
    protected float runSpeed = 10f;

    [Header("攻击")]
    [SerializeField]
    [Tooltip("基础攻击力")]
    protected float baseAttackPower = 10f;

    /// <summary>
    /// 角色是否存活
    /// </summary>
    public bool IsAlive => health > 0f;
    /// <summary>
    /// 获取角色当前生命值
    /// </summary>
    public int Health => (int)health;

    private void Awake()
    {
        if(beAttackable != null)beAttackable.OnBeAttack += OnBeAttack;
    }

    protected abstract void OnBeAttack(CharaBase sourceChara, AttackType attackType, DamageType damageType, float damageValue);

    protected abstract void OnDied(CharaBase killer);
}
