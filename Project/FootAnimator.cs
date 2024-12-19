using System;
using Godot;

public partial class FootAnimator : Node
{
    [Export] private Movement Movement;
    [Export] private Node2D Left;
    [Export] private Node2D Right;
    [Export] private double StepDuration = 0.2;
    [Export] private double StepDistance = 100;
    [Export] private double StepHeight = -50;

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
    
    public override void _Process(double delta)
    {
        base._Process(delta);
        ApplyMovement();
        UpdateState(LeftState, delta);
        UpdateState(RightState, delta);
        Left.Position = LeftState.Pos;
        Right.Position = RightState.Pos;
    }

    void Correct()
    {
        // TODO: Force move both to target
    }

    double GetBounce()
    {
        if (LeftState.Moving)
            return LeftState.AirTime / StepDuration;
        if (RightState.Moving)
            return RightState.AirTime / StepDuration;
        return 0.0;
    }
    
    bool Standing()
    {
        return !LeftState.Moving && !RightState.Moving;
    }

    void ApplyMovement()
    {
        var newPos = Movement.GetPos();
        var diff = newPos - PrevPos;
        if (!LeftState.Moving)
            LeftState.Pos -= diff;
        if (!RightState.Moving)
            RightState.Pos -= diff;
        PrevPos = newPos;
    }
    
    void UpdateState(FootState InState, double InDelta)
    {
        if (!InState.Moving)
        {
            if (!Standing())
                return;
            if (InState.Pos.Length() < StepDistance)
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
        
        double height = Math.Clamp(1 - Math.Pow(part * 2 - 1, 2), 0.0, 1.0) * StepHeight;
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
