using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : CharaBase
{
    protected override void OnBeAttack(CharaBase sourceChara, AttackType attackType, DamageType damageType, float damageValue)
    {
        //TODO: 敌人受到攻击时的处理逻辑
    }

    protected override void OnDied(CharaBase killer)
    {
    }
}
