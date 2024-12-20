using Godot;
using System;

public partial class BehaviorFly : Behavior
{
	[Export] private float Height = 200;
	[Export] private Vector2 TargetOffset = Vector2.Down * 800;
	[Export] private float OffLerpSpeed = 10.0f;

	[Export] private float WaveSpeed = 20;
	[Export] private float WaveDistance = 100;
	[Export] private float WaveHandHeight = 100;
	
	private Vector2 Offset;
	private double Duration;
	
	public override void Enter()
	{
		var c = Character.Get();
		c.Walk.IsEnabled = false;
		c.Mouth.Set(MouthAnimator.MouthType.OPEN);
		c.Eyes.CustomDir = Vector2.Down;
		Offset = Vector2.Zero;
	}

	public override void Exit()
	{
		var c = Character.Get();
		c.Walk.IsEnabled = true;
		c.Mouth.Set(MouthAnimator.MouthType.SMILE);
	}

	public override Behavior Update(double InDelta)
	{
		var c = Character.Get();
		var mp = Character.MousePos();
		var wp = Character.ScreenToWorld(mp);
		var cp = Character.WorldToChar(wp);

		if (Offset == Vector2.Zero)
			Offset = c.Movement.GetPos() - cp;

		Offset = new Vector2(
			Mathf.Lerp(Offset.X, TargetOffset.X, OffLerpSpeed * (float)InDelta),
			Mathf.Lerp(Offset.Y, TargetOffset.Y, OffLerpSpeed * (float)InDelta)
		);
		
		c.Movement.SetPos(cp + Offset);
		c.Walk.Wiggle();

		Duration += InDelta;
		Vector2 TargetHandPos = new(-WaveHandHeight, (float) Math.Sin(Duration * WaveSpeed) * WaveDistance);
		c.Hands.LerpTo(TargetHandPos, -TargetHandPos, 20, InDelta);

		float h = c.Movement.GetHeight();
		h = Mathf.Lerp(h, Height, 10 * (float)InDelta);
		c.Movement.SetHeight(h);
		
		return null;
	}
}