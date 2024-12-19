using Godot;
using System;

public partial class BehaviorWave : Behavior
{
    [Export] private Node2D HandR;
    [Export] private float WaveSpeed = 20.0f;
    [Export] private float WaveStrength = 70.0f;
    [Export] private float WaveDuration = 1.6f;
    [Export] private float RaiseHeight = 200.0f;
    private double Duration;
    
    public override void Enter()
    {
        base.Enter();

        if (Duration > 0)
            return;
        Manager.Walk.Correct();
        Manager.Eyes.CustomDir = Vector2.Up * 0.01f;
        Duration = 0;
    }

    public override void Exit()
    {
        base.Exit();
        Manager.Eyes.CustomDir = Vector2.Zero;
        Duration = 0;
    }

    public override Behavior Update(double InDelta)
    {
        Duration += InDelta;
        float wave = (float) Math.Sin(Duration * WaveSpeed) * WaveStrength;

        float part = (float) Duration / WaveDuration;
        float raise = (float)Math.Clamp(1.0 - Math.Pow((part - 0.5) * 2, 6), 0.0, 1.0);
        HandR.Position = new Vector2(wave * raise, -raise * RaiseHeight);

        if (Duration > WaveDuration)
            return Manager.Get<BehaviorIdle>();
        return null;
    }
}
