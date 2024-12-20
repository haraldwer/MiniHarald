using Godot;
using System;

public partial class BehaviorWave : Behavior
{
    [Export] private float WaveSpeed = 20.0f;
    [Export] private float WaveStrength = 70.0f;
    [Export] private float WaveDuration = 1.6f;
    [Export] private float RaiseHeight = 200.0f;
    [Export] private float GreetingChance = 0.2f;
    [Export] private string[] Greetings;
    private double Duration;
    
    private RandomNumberGenerator Rnd = new();
    
    public override void Enter()
    {
        base.Enter();

        if (Duration > 0)
            return;
        var c = Character.Get();
        c.Walk.Correct();
        c.Eyes.CustomDir = Vector2.Up * 0.1f;
        Duration = 0;

        if (Rnd.Randf() > GreetingChance)
            c.Talking.Say(Greetings[Rnd.RandiRange(0, Greetings.Length - 1)]);
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
        float wave = (float) Math.Sin(Duration * WaveSpeed) * WaveStrength;
        
        var c = Character.Get();
        c.Hands.Default(InDelta);
        float part = (float) Duration / WaveDuration;
        float raise = (float)Math.Clamp(1.0 - Math.Pow((part - 0.5) * 2, 6), 0.0, 1.0);
        c.Hands.HandR.Position = new Vector2(wave * raise, -raise * RaiseHeight);

        if (Duration > WaveDuration)
            return Get<BehaviorIdle>();
        return null;
    }
}
