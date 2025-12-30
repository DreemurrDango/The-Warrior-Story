using System;
using DataCollection;
using Enums;

/// <summary>
/// 事件转发中心
/// 使用此类可实现耦合度较低的通信模式
/// </summary>
public static class EventHandler
{
    #region 请求

    /// <summary>
    /// 请求执行某个操作
    /// </summary>
    public static event Action<int, bool, string> DoRequest;
    /// <summary>
    /// 发出执行某个操作的请求
    /// </summary>
    /// <param name="argu1"></param>
    /// <param name="argu2"></param>
    /// <param name="argu3"></param>
    public static void CallDoRequest(int argu1, bool argu2, string argu3)
        => DoRequest?.Invoke(argu1, argu2, argu3);
    #endregion

    #region 事件
    /// <summary>
    /// 角色受到伤害
    /// </summary>
    public static event Action<CharaBase, bool, CharaBase,DamageType,float> OnCharaTakeDamage;
    /// <summary>
    /// 触发角色受伤事件
    /// </summary>
    /// <param name="huntChara">受伤者角色</param>
    /// <param name="isMainChara">受伤者是否是主角</param>
    /// <param name="hurterChara">伤害来源角色</param>
    /// <param name="damageType">伤害类型</param>
    /// <param name="damageValue">伤害值</param>
    public static void CallOnCharaTakeDamage(CharaBase huntChara, bool isMainChara, CharaBase hurterChara, DamageType damageType, float damageValue)
        => OnCharaTakeDamage?.Invoke(huntChara, isMainChara, hurterChara, damageType, damageValue);

    /// <summary>
    /// 角色死亡事件
    /// </summary>
    public static event Action<CharaBase, bool, CharaBase> OnCharaDied;
    /// <summary>
    /// 触发角色死亡事件
    /// </summary>
    /// <param name="deadChara">死亡的角色</param>
    /// <param name="isMainChara">死亡的角色是否是主角</param>
    /// <param name="killerChara">最后造成伤害的角色</param>
    public static void CallOnCharaDied(CharaBase deadChara, bool isMainChara, CharaBase killerChara)
        => OnCharaDied?.Invoke(deadChara, isMainChara, killerChara);
    #endregion
}
