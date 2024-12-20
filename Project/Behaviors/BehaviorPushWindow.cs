using Godot;
using System;
using NewGameProject;

public partial class BehaviorPushWindow : Behavior
{
    [Export] private float MinDist = 50.0f;
    [Export] private float PushDuration = 1.0f;
    [Export] private Vector2 Push = Vector2.Right * 50.0f;
    
    private IntPtr WindowHandle = IntPtr.Zero;
    private Vector2 TargetPos;
    private double Duration;
    
    public override void Enter()
    {
        Character.Get().Walk.Correct();
        Duration = 0;
    }

    public override void Exit()
    {
        Duration = 0;
    }

    public override Behavior Update(double InDelta)
    {
        var c = Character.Get();
        var pos = c.Movement.GetPos();
        
        if (WindowHandle == IntPtr.Zero || TargetPos == Vector2.Zero)
        {
            WindowHandle = ProcessUtility.GetRandomWindow();
            if (WindowHandle == IntPtr.Zero)
                return base.Update(InDelta);
            
            // Start by walking there
            var p = ProcessUtility.GetWindowSide(WindowHandle);
            if (p == Vector2.Zero)
                return base.Update(InDelta);
            TargetPos = Character.ScreenToWorld(p);
            c.Eyes.CustomDir = (TargetPos - pos).Normalized();
        }
        
        // If too far away, walk towards
        if (TargetPos.DistanceTo(pos) > MinDist)
        {
            var walk = Get<BehaviorWalk>();
            walk.Target = TargetPos;
            walk.NextBehavior = this;
            walk.FollowMouse = false;
            return walk;
        }
        
        ProcessUtility.PushWindow(WindowHandle, Vector2I.Right);

        Duration += InDelta;
        if (Duration > PushDuration)
        {
            WindowHandle = IntPtr.Zero;
            TargetPos = Vector2.Zero;
            return Get<BehaviorIdle>();
        }
        return null;
    }
}