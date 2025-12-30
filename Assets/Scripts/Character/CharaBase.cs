using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharaBase : MonoBehaviour
{
    [SerializeField]
    [Tooltip("当前生命值")]
    protected float health = 100f;
    [SerializeField]
    [Tooltip("行走速度")]
    protected float walkSpeed = 5f;
    [SerializeField]
    [Tooltip("奔跑速度")]
    protected float runSpeed = 10f;
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
    /// <summary>
    /// 角色死亡时事件
    /// </summary>
    public Action OnCharaDeath;
}
