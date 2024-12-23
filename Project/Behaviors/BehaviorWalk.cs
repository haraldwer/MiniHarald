using Godot;
using System;
using NewGameProject;

public partial class BehaviorWalk : Behavior
{
    [Export] private float Speed = 800.0f; 
    [Export] private float MinDist = 900.0f; 
    [Export] private float MaxDist = 1600.0f; 
    [Export] private float CursorDist = 400.0f; 
    [Export] private float CursorChance = 0.5f; 
    
    public Vector2 Target;
    public  bool FollowMouse;
    public Behavior? NextBehavior;
    
    public override void Enter()
    {
        FollowMouse = false;
        if (Target != Vector2.Zero) 
            return;

        var c = Character.Get();
        
        FollowMouse = c.Rnd.Randf() < CursorChance;
        if (FollowMouse) 
            return;
        
        // Select a random point
        var p = c.Movement.GetPos();

        Vector2 dir = new Vector2(
            c.Rnd.RandfRange(-1, 1),
            c.Rnd.RandfRange(-1, 1)
        ).Normalized();
        Target = p + dir * c.Rnd.RandfRange(MinDist, MaxDist);
    }

    public override void Exit()
    {
        base.Exit();
        Target = Vector2.Zero;
        Character.Get().Eyes.CustomDir = Vector2.Zero;
        FollowMouse = false;
        NextBehavior = null;
    }

    public override Behavior Update(double InDelta)
    {
        var c = Character.Get();
        var p = c.Movement.GetPos();
        
        if (FollowMouse)
        {
            // Walk to cursor
            var mp = Character.WorldToChar(Character.ScreenToWorld(Character.MousePos()));
            var md = mp - p;
            float dist = Math.Max(md.Length() - CursorDist, 0);
            Target = p + md.Normalized() * dist;
        }
        
        var diff = Target - p;
        if (diff.Length() < 10.0)
        {
            if (NextBehavior != null)
                return NextBehavior;
            return Get<BehaviorIdle>();
        }

        var dir = diff.Normalized();
        p += dir * Speed * (float)InDelta;
        c.Movement.SetPos(p);
        c.Hands.Default(InDelta);
        c.Eyes.CustomDir = dir;
        return null;
    }
}
