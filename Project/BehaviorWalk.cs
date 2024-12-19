using Godot;
using System;

public partial class BehaviorWalk : Behavior
{
    [Export] private float Speed = 800.0f; 
    
    public Vector2 Target;
    private RandomNumberGenerator Rnd = new();
    
    public override void Enter()
    {
        // Select a random point
        var p = Manager.Movement.GetPos();

        Vector2 dir = new Vector2(
            Rnd.RandfRange(-1, 1),
            Rnd.RandfRange(-1, 1)
        ).Normalized();
        Target = p + dir * 1000.0f;
    }

    public override Behavior Update(double InDelta)
    {
        var p = Manager.Movement.GetPos();
        var diff = Target - p;
        if (diff.Length() < 10.0)
            return Manager.Get<BehaviorIdle>();

        var dir = diff.Normalized();
        p += dir * Speed * (float)InDelta;
        Manager.Movement.SetPos(p);
        return null;
    }
}
