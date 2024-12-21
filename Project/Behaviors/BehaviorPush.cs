using Godot;
using System;
using NewGameProject;

public partial class BehaviorPush : Behavior
{
    [Export] private float MinDist = 50.0f;
    [Export] private float MinPushDuration = 2.0f;
    [Export] private float MaxPushDuration = 5.0f;
    [Export] private Vector2 Push = Vector2.Right * 50.0f;

    [Export] private Vector2 LeftOffset = new(700, -400);
    [Export] private Vector2 RightOffset = new(50, -200);
    [Export] private Vector2 BodyOffset = new(250, -100);
    
    private IntPtr WindowHandle = IntPtr.Zero;
    private bool Left;
    private float SidePos;
    private double Countdown;
    private RandomNumberGenerator Rnd = new();
    
    private Vector2 PushOffset;
    private Vector2I WindowOffset;
    
    public override void Enter()
    {
        Character.Get().Walk.Correct();
    }

    public override void Exit()
    {
    }

    public override Behavior Update(double InDelta)
    {
        var c = Character.Get();
        var pos = c.Movement.GetPos();
        
        if (WindowHandle == IntPtr.Zero)
        {
            WindowHandle = ProcessUtility.GetRandomWindow();
            if (WindowHandle == IntPtr.Zero)
                return base.Update(InDelta);
            
            // Start by walking there
            Left = Rnd.Randf() > 0.5;
            SidePos = Rnd.RandfRange(0.2f, 0.8f);
            Countdown = Rnd.RandfRange(MinPushDuration, MaxPushDuration);
        }
        
        var p = ProcessUtility.GetWindowSide(WindowHandle, Left, SidePos);
        if (p == Vector2.Zero)
            return base.Update(InDelta);
            
        var targetPos = Character.WorldToChar(Character.ScreenToWorld(p));
        
        // Offset position
        Vector2 cOff = (Left ? BodyOffset * new Vector2(-1, 1) : BodyOffset);
        Vector2 cTarget = targetPos + cOff; 
        
        c.Eyes.CustomDir = (targetPos - pos).Normalized();
        
        // If too far away, walk towards
        if (cTarget.DistanceTo(pos) > MinDist)
        {
            var walk = Get<BehaviorWalk>();
            walk.Target = cTarget;
            walk.NextBehavior = this;
            walk.FollowMouse = false;
            return walk;
        }

        c.Hands.LerpTo(
            Left ? LeftOffset : (RightOffset * new Vector2(-1, 1)), 
            Left ? RightOffset : (LeftOffset * new Vector2(-1, 1)),
            10, 
            InDelta);

        // Move window
        PushOffset += Push * (float) InDelta * (Left ? 1 : -1); 
        Vector2I diff = new Vector2I((int)PushOffset.X, (int)PushOffset.Y) - WindowOffset;
        ProcessUtility.PushWindow(WindowHandle, diff);
        WindowOffset += diff;

        // Move character
        var worldDiff = Character.ScreenToWorld(diff);
        c.Movement.SetPos(pos + worldDiff);

        Countdown -= InDelta;
        if (Countdown < 0)
        {
            WindowHandle = IntPtr.Zero;
            if (Rnd.Randf() < 0.2)
                c.Talking.Say("Mycket bättre såhär.");
            return Get<BehaviorIdle>();
        }
        return null;
    }
}