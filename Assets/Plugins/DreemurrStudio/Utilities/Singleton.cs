using UnityEngine;

/// <summary>
/// 单例模式基础代码
/// </summary>
/// <typeparam name="T">单例模式类</typeparam>
public class Singleton<T> : MonoBehaviour where T:Singleton<T>
{
    /// <summary>
    /// 单例实例
    /// </summary>
    private static T _instance;
    /// <summary>
    /// 获取单例实例
    /// </summary>
    public static T Instance => _instance ??= FindFirstObjectByType<T>();

    /// <summary>
    /// 确保单例，若继承应当放在最前
    /// </summary>
    protected virtual void Awake()
    {
        if (_instance && _instance != this) Destroy(this);
        else _instance = (T)this;
    }
    protected virtual void OnDestroy() { if (_instance == this) _instance = null; }
}