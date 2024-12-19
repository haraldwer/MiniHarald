using Godot;
using System;

public partial class BehaviorLand : Behavior
{
	public override void Enter()
	{
		Manager.Eyes.CustomDir = Vector2.Down;
		Manager.Walk.IsEnabled = false;
		Manager.Mouth.Set(MouthAnimator.MouthType.OPEN);
	}

	public override void Exit()
	{
		Manager.Eyes.CustomDir = Vector2.Zero;
		Manager.Walk.IsEnabled = true;
		Manager.Mouth.Set(MouthAnimator.MouthType.SMILE);
	}

	private double Duration;
	
	public override Behavior Update(double InDelta)
	{
		Duration += InDelta;
		if (Duration > 0.5)
			return Manager.Get<BehaviorIdle>();
		return null;
	}
}