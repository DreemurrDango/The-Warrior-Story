using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Enums;

[TaskName("前方是否可行走")]
[TaskDescription("判断前方是否可行走")]
public class DoCanFrontWalk : Conditional
{
    public TerrainEdgeCheck edgeCheck;

    public override TaskStatus OnUpdate()
	{
        return edgeCheck.OnGround ? TaskStatus.Success : TaskStatus.Failure;
	}
}