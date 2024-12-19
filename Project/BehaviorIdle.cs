using Godot;
using System;

public partial class BehaviorIdle : Behavior
{
    private double Duration;
    
    public override void Enter()
    {
        base.Enter();
        Duration = 0.0;
        Manager.Walk.Correct();
    }

    public override Behavior Update(double InDelta)
    {
        Duration += InDelta;
        if (Duration > 1.0)
            return Manager.GetRandom();
        return null;
    }
}
