using Godot;
using System;
using NewGameProject;

public partial class BehaviorSit : Behavior
{
	[Export] private float MinDist = 50.0f;
	[Export] private float MinSitDuration = 5.0f;
	[Export] private float MaxSitDuration = 15.0f;
	
	[Export] private float FeetSpeed = 5.0f;
	[Export] private Vector2 FeetStrength = new(70, 30);
	[Export] private Vector2 FeetOffset = new(0, 70);

	[Export] private Vector2 HandOffset = new(0, 100);
	[Export] private Vector2 TargetOffset = new(0, -230);
	[Export] private bool UseWindow = true;
	
	private IntPtr WindowHandle = IntPtr.Zero;
	private float SidePos;
	private double Countdown;
	private bool Seated;
	
	public override void Enter()
	{
		Character.Get().Walk.Correct();
	}

	public override void Exit()
	{
		Seated = false;
		var c = Character.Get();
		c.Eyes.CustomDir = Vector2.Zero;
		c.Movement.Shadow.Visible = true;
		c.Walk.IsEnabled = true;
		c.Walk.Correct();
	}

	public override Behavior Update(double InDelta)
	{
		var c = Character.Get();
		var pos = c.Movement.GetPos();

		var targetPos = pos;

		if (UseWindow)
		{
			if (WindowHandle == IntPtr.Zero)
			{
				WindowHandle = ProcessUtility.GetRandomWindow();
				if (WindowHandle == IntPtr.Zero)
					return Get<BehaviorIdle>();
			
				// Start by walking there
				SidePos = c.Rnd.RandfRange(0.2f, 0.8f);
				Countdown = c.Rnd.RandfRange(MinSitDuration, MaxSitDuration);
			}
		
			var p = ProcessUtility.GetWindowTop(WindowHandle, SidePos);
			if (p == Vector2.Zero)
				return Get<BehaviorIdle>();
			targetPos = Character.WorldToChar(Character.ScreenToWorld(p));
			targetPos += TargetOffset;

			if (targetPos.Y < -530)
			{
				WindowHandle = IntPtr.Zero;
				return Get<BehaviorIdle>();
			}

			GD.Print(targetPos);
		
			// If too far away
			// If not yet seated, walk towards
			if (targetPos.DistanceTo(pos) > MinDist && !Seated)
			{
				// Look towards
				c.Eyes.CustomDir = (targetPos - pos).Normalized();
			
				var walk = Get<BehaviorWalk>();
				walk.Target = targetPos;
				walk.NextBehavior = this;
				walk.FollowMouse = false;
				return walk;
			}
		}
		else if (Countdown <= 0)
		{
			Countdown = c.Rnd.RandfRange(MinSitDuration, MaxSitDuration);
		}

		if (!Seated)
		{
			Seated = true;
			if (c.Rnd.Randf() < 0.2 && UseWindow)
				c.Talking.Say("Fin utsikt!");
		}
		
		c.Movement.SetPos(targetPos); // Teleport
		if (UseWindow)
			c.Movement.Shadow.Visible = false;
		c.Walk.IsEnabled = false;
		c.Eyes.CustomDir = Vector2.Zero;
		c.Hands?.LerpTo(HandOffset, HandOffset, 10, InDelta);

		if (UseWindow)
		{
			double t = Countdown * FeetSpeed;
			float lx = (float)Math.Sin(t);
			float ly = (float)Math.Sin(t * 0.5);
			c.Walk.Left.Position = c.Walk.Left.Position.Lerp(new Vector2(lx, ly) * FeetStrength + FeetOffset, 10 * (float)InDelta);
			float rx = (float)Math.Sin(-t);
			float ry = (float)Math.Sin(-t * 0.5);
			c.Walk.Right.Position = c.Walk.Right.Position.Lerp(new Vector2(rx, ry) * FeetStrength + FeetOffset, 10 * (float)InDelta);
		}
		else
		{
			c.Walk.Head.Position = new Vector2(0, 100);
			c.Walk.Body.Position = new Vector2(0, 100);
			c.Walk.Left.Position = new Vector2(0, 0);
			c.Walk.Right.Position = new Vector2(0, 0);
			c.Walk.LeftBack.Position = new Vector2(0, 0);
			c.Walk.RightBack.Position = new Vector2(0, 0);
			c.Walk.Tail.Position = new Vector2(
				(float)Math.Sin(Countdown * 2) * 20 + 150,
				(float)Math.Sin(Countdown * 1.567) * 50 - 800);
		}

		GD.Print(Countdown);

		Countdown -= InDelta;
		if (Countdown < 0)
		{
			WindowHandle = IntPtr.Zero;
			return Get<BehaviorIdle>();
		}
		return null;
	}
}
