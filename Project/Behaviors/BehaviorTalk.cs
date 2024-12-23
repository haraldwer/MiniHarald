using Godot;
using System;

public partial class BehaviorTalk : Behavior
{
    [Export] private string[] Talks;
    [Export] private double TalkDuration;
    private double Duration;
    
    public override void Enter()
    {
        base.Enter();

        if (Duration > 0)
            return;
        var c = Character.Get();
        c.Walk.Correct();
        c.Eyes.CustomDir = Vector2.Up * 0.1f;
        Duration = 0;

        c.Talking.Say(Talks[c.Rnd.RandiRange(0, Talks.Length - 1)]);
    }

    public override void Exit()
    {
        base.Exit();
        Character.Get().Eyes.CustomDir = Vector2.Zero;
        Duration = 0;
    }

    public override Behavior Update(double InDelta)
    {
        Duration += InDelta;
        Character.Get().Hands.LerpTo(
            Vector2.Zero, 
            Vector2.Up * 200,
            8, InDelta
        );
        
        if (Duration > TalkDuration)
            return Get<BehaviorIdle>();
        return null;
    }
}
