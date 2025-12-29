using Godot;
using System;

public partial class WalkAnimator : Node
{
	[Export] public Node2D Left;
	[Export] public Node2D Right;
	[Export] public Node2D LeftBack;
	[Export] public Node2D RightBack;

	[Export] public Node2D Body;
	[Export] public Node2D Head;
	[Export] public Node2D Tail;
	[Export] private Node2D HandLeft;
	[Export] private Node2D HandRight;
	[Export] private double StepDuration = 0.2;
	[Export] private double StepDistance = 100;
	[Export] private double StepHeight = -50;
	[Export] private double BounceHeight = -50;
	[Export] private double HandBounceHeight = 20;
	[Export] private Vector2 BackOffset = new Vector2();
	[Export] private Vector2 HeadOffset = new Vector2();
	[Export] private Vector2 TailOffset = new Vector2();


	class FootState
	{
		public Vector2 Pos;
		public Vector2 StepStartPos;
		public Vector2 StepTargetPos;
		public bool Moving;
		public double AirTime;
	}

	private Vector2 PrevPos;
	private Vector2 WalkDir;

	private FootState LeftState = new();
	private FootState LeftBackState = new();
	private FootState RightState = new();
	private FootState RightBackState = new();

	public bool IsEnabled = true;
	
	public override void _Process(double delta)
	{
		base._Process(delta);
		ApplyMovement();

		UpdateState(LeftState, false, delta);
		UpdateState(RightState, false, delta);
		UpdateState(RightBackState, true, delta);
		UpdateState(LeftBackState, true, delta);
		
		Left.Position = LeftState.Pos - WalkDir * BackOffset;
		Right.Position = RightState.Pos - WalkDir * BackOffset;
		Head.Position = -WalkDir * HeadOffset;
		if (Tail != null)
			Tail.Position = WalkDir * TailOffset;
		
		if (LeftBack != null)
			LeftBack.Position = LeftBackState.Pos + WalkDir * BackOffset;
		if (RightBack != null)
			RightBack.Position = RightBackState.Pos + WalkDir * BackOffset;

		float bodyBounce = (float) (BounceHeight * Math.Abs(GetBounce()));
		Body.Position = new Vector2(0.0f, bodyBounce);
	}

	public void Set(Vector2 InOffset)
	{
		SetStateOffset(InOffset, LeftState);
		SetStateOffset(InOffset, RightState);
		SetStateOffset(InOffset, RightBackState);
		SetStateOffset(InOffset, LeftBackState);

		Left.Position = InOffset;
		Right.Position = InOffset;
	}

	private void SetStateOffset(Vector2 InOffset, FootState InState)
	{
		InState.Moving = false;
		InState.AirTime = 0;
		InState.Pos = InOffset;
		InState.StepStartPos = Vector2.Zero;
		InState.StepTargetPos = Vector2.Zero;
	}

	public void Wiggle()
	{
		Set(Vector2.Zero);
	}
	
	public void Correct()
	{
		CorrectState(LeftState);
		CorrectState(RightState);
		CorrectState(RightBackState);
		CorrectState(LeftBackState);
	}

	private void CorrectState(FootState InState)
	{
		if (InState.Pos.Length() > StepDistance * 0.3 || InState.Moving)
		{
			InState.Moving = true;
			InState.AirTime = 0;
			InState.StepStartPos = InState.Pos;
			InState.StepTargetPos = Vector2.Zero;
		}
	}

	double GetBounce()
	{
		if (LeftState.Moving)
			return GetStepHeight(LeftState.AirTime / StepDuration);
		if (RightState.Moving)
			return GetStepHeight(RightState.AirTime / StepDuration);
		return 0.0;
	}

	double GetStepHeight(double InPart)
	{
		return Math.Clamp(1 - Math.Pow(InPart * 2 - 1, 2), 0.0, 1.0);
	}
	
	bool Standing()
	{
		return !LeftState.Moving && !RightState.Moving;
	}

	bool BackStanding()
	{
		return !LeftBackState.Moving && !RightBackState.Moving;
	}

	void ApplyMovement()
	{
		var newPos = Character.Get().Movement.GetPos();
		var diff = newPos - PrevPos;
		ApplyStateMovement(LeftState, diff);
		ApplyStateMovement(RightState, diff);
		ApplyStateMovement(RightBackState, diff);
		ApplyStateMovement(LeftBackState, diff);

		WalkDir.X = (float)Mathf.Lerp(WalkDir.X, diff.Normalized().X, 0.1);
		WalkDir.Y = (float)Mathf.Lerp(WalkDir.Y, diff.Normalized().Y, 0.1);

		PrevPos = newPos;
	}

	private void ApplyStateMovement(FootState InState, Vector2 InDiff)
	{
		if (!InState.Moving) InState.Pos -= InDiff;
	}

	void UpdateState(FootState InState, bool InBack, double InDelta)
	{
		if (!IsEnabled)
			return; 
		
		if (!InState.Moving)
		{
			if (!(InBack ? BackStanding() : Standing()))
				return;
			if (InState.Pos.Length() < StepDistance * 1.1f)
				return;
			
			InState.Moving = true;
			InState.AirTime = 0;
			InState.StepStartPos = InState.Pos;
			InState.StepTargetPos = -InState.Pos;
		}

		InState.AirTime += InDelta;
		double part = InState.AirTime / StepDuration;
		InState.Pos.X = (float) Mathf.Lerp(InState.StepStartPos.X, InState.StepTargetPos.X, part);
		InState.Pos.Y = (float) Mathf.Lerp(InState.StepStartPos.Y, InState.StepTargetPos.Y, part);
		
		double height = GetStepHeight(part) * StepHeight;
		InState.Pos.Y += (float)height;
		
		// End step
		if (part >= 0.99)
		{
			InState.Moving = false;
			InState.Pos = InState.StepTargetPos;
			if (InState.Pos.Length() > StepDistance)
				InState.Pos = InState.Pos.Normalized() * (float)StepDistance;
			InState.AirTime = 0.0;
			InState.StepStartPos = Vector2.Zero;
		}
	}
}
