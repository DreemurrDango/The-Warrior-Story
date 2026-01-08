using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskName("定向移动")]
public class Move : Action
{
    public SharedVector2 moveDirection;
    public SharedFloat speed;

    private Rigidbody2D rb;
    public override void OnAwake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Running;
    }

    public override void OnFixedUpdate()
    {
        rb.position += speed.Value * Time.fixedDeltaTime * moveDirection.Value.normalized;
    }
}
