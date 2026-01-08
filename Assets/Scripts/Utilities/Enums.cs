using UnityEngine;

namespace Enums
{
    /// <summary>
    /// 攻击类型
    /// </summary>
    public enum AttackType
    {
        normal,
        explode,
        COUNT
    }

    /// <summary>
    /// 造成的伤害类型
    /// </summary>
    public enum DamageType
    {
        物理,
        魔法,
        真实,
        COUNT
    }

    /// <summary>
    /// 方位
    /// </summary>
    public enum Direction
    {
        Left,
        Right,
        COUNT
    }
}