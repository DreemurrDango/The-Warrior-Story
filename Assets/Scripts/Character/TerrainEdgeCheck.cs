using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using System;
using Sirenix.OdinInspector;

public class TerrainEdgeCheck : MonoBehaviour
{
    [Tooltip("检测地形边缘的方位")]
    public Direction edgeDirection;

    [ShowInInspector]
    /// <summary>
    /// 获取是否仍在地面上
    /// </summary>
    public bool OnGround { get; private set; } = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Ground")) return;
        OnGround = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Ground")) return;
        OnGround = false;
    }
}
