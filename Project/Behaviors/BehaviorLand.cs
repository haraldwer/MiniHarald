using Godot;
using System;

public partial class BehaviorLand : Behavior
{
	[Export] private double LandDuration = 0.2; 
	
	private double Duration;
	
	public override void Enter()
	{
		var c = Character.Get();
		c.Eyes.CustomDir = Vector2.Down;
		c.Walk.IsEnabled = false;
		c.Mouth.Set(MouthAnimator.MouthType.OPEN);
		Duration = 0;
	}

	public override void Exit()
	{
		var c = Character.Get();
		c.Eyes.CustomDir = Vector2.Zero;
		c.Walk.IsEnabled = true;
		c.Mouth.Set(MouthAnimator.MouthType.SMILE);
		Duration = 0;
	}
	
	public override Behavior Update(double InDelta)
	{
		Character.Get().Hands.LerpTo(
			Vector2.Left * 100 + Vector2.Up * 200, 
			Vector2.Right * 100 + Vector2.Up * 200, 
			20, InDelta);
		
		Duration += InDelta;
		return Duration > LandDuration ? Get<BehaviorIdle>() : null;
	}
}