using System;
using Godot;

public partial class WalkAnimator : Node
{
    [Export] private Node2D Left;
    [Export] private Node2D Right;
    [Export] private Node2D Body;
    [Export] private Node2D HandLeft;
    [Export] private Node2D HandRight;
    [Export] private double StepDuration = 0.2;
    [Export] private double StepDistance = 100;
    [Export] private double StepHeight = -50;
    [Export] private double BounceHeight = -50;
    [Export] private double HandBounceHeight = 20;

    class FootState
    {
        public Vector2 Pos;
        public Vector2 StepStartPos;
        public Vector2 StepTargetPos;
        public bool Moving;
        public double AirTime;
    }

    private Vector2 PrevPos;
    private FootState LeftState = new();
    private FootState RightState = new();

    public bool IsEnabled = true;
    
    public override void _Process(double delta)
    {
        base._Process(delta);
        ApplyMovement();
        UpdateState(LeftState, delta);
        UpdateState(RightState, delta);
        
        Left.Position = LeftState.Pos;
        Right.Position = RightState.Pos;

        float bodyBounce = (float) (BounceHeight * Math.Abs(GetBounce()));
        Body.Position = new Vector2(0.0f, bodyBounce);
    }

    public void Set(Vector2 InOffset)
    {
        LeftState.Moving = false;
        LeftState.AirTime = 0;
        LeftState.Pos = InOffset;
        LeftState.StepStartPos = Vector2.Zero;
        LeftState.StepTargetPos = Vector2.Zero;
        RightState.Moving = false;
        RightState.AirTime = 0;
        RightState.Pos = InOffset;
        RightState.StepStartPos = Vector2.Zero;
        RightState.StepTargetPos = Vector2.Zero;
        Left.Position = InOffset;
        Right.Position = InOffset;
    }

    public void Wiggle()
    {
        Set(Vector2.Zero);
    }
    
    public void Correct()
    {
        // TODO: Force move both to target

        if (LeftState.Pos.Length() > StepDistance * 0.3 || LeftState.Moving)
        {
            LeftState.Moving = true;
            LeftState.AirTime = 0;
            LeftState.StepStartPos = LeftState.Pos;
            LeftState.StepTargetPos = Vector2.Zero;
        }

        if (RightState.Pos.Length() > StepDistance * 0.3 || RightState.Moving)
        {
            RightState.Moving = true;
            RightState.AirTime = 0;
            RightState.StepStartPos = RightState.Pos;
            RightState.StepTargetPos = Vector2.Zero;
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

    void ApplyMovement()
    {
        var newPos = Character.Get().Movement.GetPos();
        var diff = newPos - PrevPos;
        if (!LeftState.Moving)
            LeftState.Pos -= diff;
        if (!RightState.Moving)
            RightState.Pos -= diff;
        PrevPos = newPos;
    }
    
    void UpdateState(FootState InState, double InDelta)
    {
        if (!IsEnabled)
            return; 
        
        if (!InState.Moving)
        {
            if (!Standing())
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
