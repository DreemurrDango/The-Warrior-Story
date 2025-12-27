using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundCheck : MonoBehaviour
{
    private bool onGround = false;
    /// <summary>
    /// 当前是否在地面上
    /// </summary>
    public bool OnGround => onGround;

    /// <summary>
    /// 落地事件
    /// </summary>
    public Action OnLandGround;
    /// <summary>
    /// 离开地面事件
    /// </summary>
    public Action OnLeaveGround;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"触碰到了{collision.gameObject}");
        if (collision.CompareTag("Ground"))
        {
            onGround = true;
            OnLandGround?.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log($"离开了{collision.gameObject}");
        if (collision.CompareTag("Ground"))
        {
            onGround = false;
            OnLeaveGround?.Invoke();
        }
    }
}
