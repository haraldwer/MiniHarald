using Godot;
using System;

public partial class BehaviorIdle : Behavior
{
    [Export] private float MaxDuration = 5.0f; 
    [Export] private float MinDuration = 1.0f;

    private RandomNumberGenerator Rnd = new();
    private double Countdown;
    
    public override void Enter()
    {
        base.Enter();
        Countdown = Rnd.RandfRange(MinDuration, MaxDuration);
        Character.Get().Walk.Correct();
    }

    public override Behavior Update(double InDelta)
    {
        Character.Get().Hands.Idle(InDelta);
        
        Countdown -= InDelta;
        if (Countdown < 0)
            return Character.Get().Behavior.GetRandom();
            //return Get<BehaviorGift>();
        return null;
    }
}
