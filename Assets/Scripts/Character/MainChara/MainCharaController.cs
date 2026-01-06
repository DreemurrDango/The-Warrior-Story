using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class MainCharaController : MonoBehaviour
{
    private static int AnimParam_HorizontalInput = Animator.StringToHash("horizontalInput");
    private static int AnimParam_VerticalVelocity = Animator.StringToHash("verticalVelocity");
    private static int AnimParam_InAir = Animator.StringToHash("inAir");
    private static int AnimParam_InCrouch = Animator.StringToHash("inCrouch");
    private static int AnimParam_InAttack = Animator.StringToHash("inAttack");
    private static int AnimParam_DoCriticalHit = Animator.StringToHash("doCritical");

    [Header("组件引用")]
    [SerializeField]
    private Animator animator;
    [SerializeField]
    [Tooltip("玩家输入组件")]
    private PlayerInput playerInput;
    [SerializeField]
    [Tooltip("角色刚体组件")]
    private Rigidbody2D rigidbody2D;
    [SerializeField]
    [Tooltip("角色正常状态下的碰撞盒")]
    private Collider2D normalCollider2D;
    [SerializeField]
    [Tooltip("下蹲时使用固定碰撞盒")]
    private Collider2D crouchCollider2D;
    [SerializeField]
    [Tooltip("地面检测组件")]
    private OnGroundCheck onGroundCheck;

    [Header("数据配置")]
    [SerializeField]
    [Tooltip("最大移动速度")]
    private float maxRunSpeed = 5f;
    [SerializeField]
    [Tooltip("最大行走速度")]
    private float maxWalkSpeed = 2.5f;
    [SerializeField]
    [Tooltip("加速到最大速度所需时间")]
    private float timeToMaxSpeed = 0.2f;
    [SerializeField]
    [Tooltip("跳跃力度")]
    private float jumpForce = 300f;
    [SerializeField]
    [Tooltip("最大连续跳跃次数")]
    private int maxJumpCount = 2;
    [SerializeField]
    [Tooltip("跳跃预备时间")]
    private float jumpPrepareTime = 0.1f;

    [Header("攻击配置")]
    [SerializeField]
    [Tooltip("攻击效果列表")]
    private AttackEffect[] attackEffects;
    [SerializeField]
    [Tooltip("暴击率")]
    [Range(0.01f, 1f)]
    private float criticalHitRate = 0.1f;

    [ReadOnly, SerializeField]
    /// <summary>
    /// 输入的方向向量
    /// </summary>
    private Vector2 inputDirection;
    /// <summary>
    /// 当前是否在保持行走
    /// </summary>
    private bool inKeepWalking = false;
    /// <summary>
    /// 当前速度
    /// </summary>
    private float currentXSpeed = 0f;
    /// <summary>
    /// 当前可用的连续跳跃次数
    /// </summary>
    private float leftJumpTimes = 0;
    /// <summary>
    /// 输入锁定计时器
    /// </summary>
    private float t_lockInput = 0f;
    /// <summary>
    /// 此次攻击是否为暴击
    /// </summary>
    private bool isCriticalHit = false;

    /// <summary>
    /// 获取当前是否在地面上
    /// </summary>
    public bool OnGround => onGroundCheck.OnGround;
    /// <summary>
    /// 获取当前是否在下蹲
    /// </summary>
    public bool InCrouch => inputDirection.y < -0.5f;
    /// <summary>
    /// 是否正在攻击
    /// </summary>
    public bool InAttack { get; private set; }
    /// <summary>
    /// 是否处于输入锁定状态
    /// </summary>
    public bool InInputLocking => t_lockInput > 0f;

    /// <summary>
    /// 获取当前最大水平速度
    /// </summary>
    public float MaxHorizontalSpeed => inKeepWalking ? maxWalkSpeed : maxRunSpeed;

    private void Awake()
    {
        if (playerInput == null) throw new Exception("PlayerInput组件未绑定");
        playerInput.actions["Jump"].started += OnJumpStarted;
        playerInput.actions["KeepWalk"].started += ctx => inKeepWalking = true;
        playerInput.actions["KeepWalk"].canceled += ctx => inKeepWalking = false;
        playerInput.actions["Fire"].started += ctx => InAttack = true;
        playerInput.actions["Fire"].canceled += ctx => InAttack = false;
        if (onGroundCheck == null) throw new Exception("OnGroundCheck组件未绑定");
        onGroundCheck.OnLandGround += OnLandGround;
    }

    /// <summary>
    /// 着陆时调用
    /// </summary>
    private void OnLandGround()
    {
        leftJumpTimes = maxJumpCount;
    }

    /// <summary>
    /// 跳跃按钮被按下时调用
    /// </summary>
    /// <param name="context"></param>
    private void OnJumpStarted(InputAction.CallbackContext context)
    {
        if (leftJumpTimes <= 0 || InInputLocking) return;
        // 延迟跳跃
        if (onGroundCheck.OnGround && jumpPrepareTime > 0)
            StartCoroutine(JumpAfterDelay(jumpPrepareTime));
        else DoJump();

        IEnumerator JumpAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            DoJump();
        }

        void DoJump()
        {
            rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            //animator.SetBool(AnimParam_InJumping, true);
            leftJumpTimes--;
        }
    }

    /// <summary>
    /// 上次暴击检测时间戳
    /// </summary>
    private float t_lastCriticalCheck = 0f;
    private void Update()
    {
        if (InInputLocking) t_lockInput -= Time.deltaTime;
        // 处理下蹲状态
        normalCollider2D.enabled = !InCrouch;
        crouchCollider2D.enabled = InCrouch;
        // 暴击检测
        if (Time.time > t_lastCriticalCheck + 1f)
        {
            isCriticalHit = UnityEngine.Random.value < criticalHitRate;
            t_lastCriticalCheck = Time.time;
        }
        // 更新动画参数
        var horizontalInput = inKeepWalking ? inputDirection.x / 2 : inputDirection.x;
        animator.SetFloat(AnimParam_HorizontalInput, Mathf.Abs(horizontalInput));
        animator.SetFloat(AnimParam_VerticalVelocity, rigidbody2D.velocity.y);
        animator.SetBool(AnimParam_InAir, !OnGround);
        animator.SetBool(AnimParam_InCrouch, InCrouch);
        animator.SetBool(AnimParam_InAttack, !InInputLocking && InAttack);
        animator.SetBool(AnimParam_DoCriticalHit, isCriticalHit);
    }

    private void FixedUpdate()
    {
        if (InInputLocking) return;
        // 读取输入方向，计算中间变量
        inputDirection = playerInput.actions["Move"].ReadValue<Vector2>();
        var normalizedInput = inputDirection.normalized;
        float inputX = normalizedInput.x;
        // 更新角色朝向
        if (inputX != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(inputX) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        if (InCrouch || InAttack)
        {
            // 下蹲或攻击时不可移动，并且水平速度归零
            rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
            currentXSpeed = 0;
        }
        else HandleHorizontalMove();

        void HandleHorizontalMove()
        {
            // 处理水平移动,计算目标速度
            float targetSpeed = inputDirection.magnitude * MaxHorizontalSpeed;
            // 平滑调整当前速度
            currentXSpeed = Mathf.MoveTowards(currentXSpeed, targetSpeed, (MaxHorizontalSpeed / timeToMaxSpeed) * Time.fixedDeltaTime);
            // 计算移动向量
            var moveVector = new Vector2(inputX * currentXSpeed, rigidbody2D.velocity.y);
            // 应用移动
            rigidbody2D.velocity = moveVector;
        }
    }

    #region 输入锁定
    /// <summary>
    /// 锁定输入一段时间，若未指定则永久锁定
    /// </summary>
    /// <param name="lockTime">要持续锁定的时间，若忽略则为一个极长的时间，可视为永久锁定</param>
    public void LockInput(float lockTime = 9999f) => t_lockInput = lockTime;
    /// <summary>
    /// 取消锁定输入
    /// </summary>
    public void UnlockInput() => t_lockInput = 0f;
    #endregion
}
