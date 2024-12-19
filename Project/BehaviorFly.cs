using Godot;
using System;

public partial class BehaviorFly : Behavior
{
	public override void Enter()
	{
		base.Enter();
		Manager.Walk.IsEnabled = false;
		Manager.Mouth.Set(MouthAnimator.MouthType.OPEN);
	}

	public override void Exit()
	{
		base.Exit();
		Manager.Walk.IsEnabled = true;
		Manager.Mouth.Set(MouthAnimator.MouthType.SMILE);
	}

	public override Behavior Update(double InDelta)
	{
		return null;
	}
}