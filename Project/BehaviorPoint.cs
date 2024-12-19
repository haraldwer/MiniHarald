using Godot;
using System;

public partial class BehaviorPoint : Behavior
{
    [Export] private Node2D HandR;
    [Export] private float PointDuration = 2.0f;
    [Export] private float PointDistance = 200.0f;
    
    public Vector2 Direction;
    private RandomNumberGenerator Rnd = new();
    private double Duration; 
    
    public override void Enter()
    {
        base.Enter();
        
        Direction = new Vector2(
            Rnd.RandfRange(-1, 1),
            Rnd.RandfRange(-1, 1)
        ).Normalized();
        Duration = 0;

        Manager.Eyes.CustomDir = Direction;
    }

    public override void Exit()
    {
        base.Exit();
        
        Manager.Eyes.CustomDir = Vector2.Zero;
    }

    public override Behavior Update(double InDelta)
    {
        Duration += InDelta;
        float part = (float) Duration / PointDuration;
        float raise = (float)Math.Clamp(1.0 - Math.Pow((part - 0.5) * 2, 6) + Math.Cos(part * 30.0) * 0.1, 0.0, 1.0);
        HandR.Position = Vector2.Up * 50.0f + raise * Direction * PointDistance; 
        if (Duration > PointDuration)
            return Manager.Get<BehaviorIdle>();
        return null;
    }
}
