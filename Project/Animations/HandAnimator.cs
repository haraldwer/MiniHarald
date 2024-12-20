using Godot;
using System;

public partial class HandAnimator : Node
{
    [Export] public Node2D HandL; 
    [Export] public Node2D HandR;

    private double Duration;
    
    public void Default(double InDelta)
    {
        Duration += InDelta;
        LerpTo(Vector2.Zero, Vector2.Zero, 4.0f, InDelta);
    }

    public void Idle(double InDelta)
    {
        Duration += InDelta;

        var l = Off(Duration);
        var r = Off(-Duration);
        LerpTo(l, r, 6.0f, InDelta);
    }
    
    Vector2 Off(double InDuration)
    {
        double w = Mathf.Sin(InDuration * 3);
        float x = (float)Mathf.Cos(w + Math.PI / 2);
        float y = (float)Mathf.Sin(w + Math.PI / 2);
        var p = new Vector2(x * 100, y * 50 - 30);
        return p;
    }

    public void LerpTo(Vector2 InL, Vector2 InR, float InSpeed, double InDelta)
    {
        HandL.Position = new(
            Mathf.Lerp(HandL.Position.X, InL.X, InSpeed * (float)InDelta),
            Mathf.Lerp(HandL.Position.Y, InL.Y, InSpeed * (float)InDelta)
        );
        HandR.Position = new(
            Mathf.Lerp(HandR.Position.X, InR.X, InSpeed * (float)InDelta),
            Mathf.Lerp(HandR.Position.Y, InR.Y, InSpeed * (float)InDelta)
        );
    }
    
    
}