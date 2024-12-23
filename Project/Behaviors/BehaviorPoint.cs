using Godot;
using System;

public partial class BehaviorPoint : Behavior
{
    [Export] private float PointDuration = 2.0f;
    [Export] private float PointDistance = 200.0f;
    [Export] private float RaiseOffset = 90.0f;
    
    public Vector2 Direction;
    private double Duration; 
    
    public override void Enter()
    {
        base.Enter();
        
        Direction = new Vector2(
            Character.Get().Rnd.RandfRange(-1, 1),
            Character.Get().Rnd.RandfRange(-1, 1)
        ).Normalized();
        Duration = 0;

        Character.Get().Eyes.CustomDir = Direction;
    }

    public override void Exit()
    {
        base.Exit();
        
        Character.Get().Eyes.CustomDir = Vector2.Zero;
    }

    public override Behavior Update(double InDelta)
    {
        Duration += InDelta;
        float part = (float) Duration / PointDuration;
        float raise = (float)Math.Clamp(1.0 - Math.Pow((part - 0.5) * 2, 6) + Math.Cos(part * 30.0) * 0.1, 0.0, 1.0);
        
        Character.Get().Hands.LerpTo(
            Vector2.Zero, 
            Vector2.Up * RaiseOffset + raise * Direction * PointDistance,
            20,
            InDelta
        );
        
        if (Duration > PointDuration)
            return Get<BehaviorIdle>();
        return null;
    }
}
