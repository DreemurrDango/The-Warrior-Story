using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskName("转向")]
[TaskDescription("让角色转向指定方向，或是输入方向的相反方向，只会进行水平转向")]
public class TurnAround : Action
{
	public SharedVector2 direction;
	public bool invert = true;
	public SharedVector2 result;

	public override TaskStatus OnUpdate()
	{
		Vector2 dir = direction.Value.normalized;
		if (invert) dir.x *= -1;
		result.Value = dir;
		transform.localScale = new Vector3(dir.x >= 0 ? 1 : -1, 1, 1);
        return TaskStatus.Success;
	}
}