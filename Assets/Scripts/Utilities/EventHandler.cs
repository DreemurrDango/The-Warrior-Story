using System;
using DataCollection;

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
    /// 发生事件
    /// </summary>
    public static event Action<int, bool, string> OnAction;
    /// <summary>
    /// 触发事件
    /// </summary>
    public static void CallOnAction(int argu1, bool argu2, string argu3)
        => OnAction?.Invoke(argu1, argu2, argu3);
    #endregion
}
