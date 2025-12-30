using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家主角角色行为类
/// </summary>
public class MainChara : CharaBase
{
    /// <summary>
    /// 受到伤害动画参数哈希值
    /// </summary>
    private static int AnimParam_GetHurt = Animator.StringToHash("getHurt");
    /// <summary>
    /// 是否处于受伤保护状态动画参数哈希值
    /// </summary>
    private static int AnimParam_InProtection = Animator.StringToHash("inProtection");
    /// <summary>
    /// 是否已死亡动画参数哈希值
    /// </summary>
    private static int AnimParam_IsDead = Animator.StringToHash("isDead");

    [SerializeField]
    [Tooltip("受到伤害后的保护时间")]
    private float hurtProtectTime = 1.0f;
    [SerializeField]
    [Tooltip("受到伤害后输入锁定时间")]
    private float afterHurtInputLockTime = 0.2f;
    [SerializeField]
    [Tooltip("是否处于无敌状态")]
    private bool invincible = false;
    [SerializeField]
    [Tooltip("受到伤害时的反作用力")]
    private Vector2 getHurtForce;

    [Header("组件引用")]
    [SerializeField]
    [Tooltip("主角控制器组件")]
    private MainCharaController controller;
    [SerializeField]
    [Tooltip("动画组件")]
    private Animator animator;
    [SerializeField]
    [Tooltip("2D刚体组件")]
    private Rigidbody2D rigidbody2D;

    /// <summary>
    /// 受伤保护剩余时间
    /// </summary>
    private float t_hurtProtect = 0f;

    /// <summary>
    /// 是否处于受伤保护状态
    /// </summary>
    public bool CanGetHurt => !invincible && t_hurtProtect <= 0f;

    private void Update()
    {
        if (t_hurtProtect > 0f) t_hurtProtect -= Time.deltaTime;
        animator.SetBool(AnimParam_InProtection,!invincible && t_hurtProtect > 0f);
        animator.SetBool(AnimParam_IsDead, health <= 0f);
    }

    /// <summary>
    /// 主角受到攻击时的处理
    /// </summary>
    /// <param name="sourceChara">伤害来源角色</param>
    /// <param name="attackType">攻击类型</param>
    /// <param name="damageType">伤害类型</param>
    /// <param name="damageValue">伤害数值</param>
    protected override void OnBeAttack(CharaBase sourceChara, AttackType attackType, DamageType damageType, float damageValue)
    {
        if(!CanGetHurt || !IsAlive) return;
        health -= damageValue;
        EventHandler.CallOnCharaTakeDamage(this, true, sourceChara, damageType, damageValue);
        Debug.Log($"{name}受到来自{(sourceChara ? sourceChara.name : "(无角色)")}的{damageValue}点{damageType}伤害");
        // 锁定输入
        controller.LockInput(afterHurtInputLockTime);
        // 检测是否死亡
        if (health <= 0f) OnDied(sourceChara);
        else
        {
            // 施加受伤后退力
            var force = getHurtForce;
            if (sourceChara != null)
            {
                //rigidbody2D.velocity = Vector2.zero;
                var dir = transform.position - sourceChara.transform.position;
                if (Mathf.Abs(dir.x) < 0.1f) force.x = 0f;
                else
                {
                    force.x *= dir.x > 0f ? 1 : -1;
                    force.y = 0f;
                }
            }
            else force.x = 0f;
            rigidbody2D.AddForce(force, ForceMode2D.Impulse);
            // 播放受伤动画
            animator.SetTrigger(AnimParam_GetHurt);
            // 重置受伤保护时间
            t_hurtProtect = hurtProtectTime;
        }        
    }

    /// <summary>
    /// 主角死亡时的处理
    /// </summary>
    /// <param name="killer">最后造成伤害的角色</param>
    protected override void OnDied(CharaBase killer)
    {
        var v = rigidbody2D.velocity;
        v.x = 0f;
        rigidbody2D.velocity = v;
        controller.LockInput();
        EventHandler.CallOnCharaDied(this, true, killer);
    }
}
