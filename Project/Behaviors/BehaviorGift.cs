using Godot;
using System;

public partial class BehaviorGift : Behavior
{
    [Export] private Vector2 StartOffset = new(0, -1200);
    [Export] private Vector2 EndOffset = new(-900, 100);
    [Export] private Vector2 HandsOffset = new(0, 0);
    [Export] private float FadeInDuration = 0.2f;
    [Export] private string[] Texts;
    
    [Export] private double MoveDuration = 2.0;
    [Export] private double WaitDuration = 0.5;
    [Export] private double GiftChance = 0.2;
    
    private int GiftIndex = -1;
    private Vector2 GiftPos;
    private double Duration;
    
    public override void Enter()
    {
        var c = Character.Get();
        if (c.Rnd.Randf() > GiftChance)
        {
            GiftIndex = -1;
            return;
        }
        
        GiftIndex = c.Rnd.RandiRange(0, c.GiftWindows.Length - 1);
        var gift = c.GiftWindows[GiftIndex];
        if (gift.Visible)
        {
            GiftIndex = -1;
            return;
        }

        Duration = 0;
        gift.Visible = true;
        GiftPos = StartOffset;
        if (gift.GetChild(0) is Sprite2D sprite)
            sprite.SetModulate(new(1, 1, 1, 0));
    }

    public override Behavior Update(double InDelta)
    {
        if (GiftIndex == -1)
            return Get<BehaviorIdle>();
        
        var c = Character.Get();
        var gift = c.GiftWindows[GiftIndex] as GiftWindow;
        if (gift == null)
            return Get<BehaviorIdle>();

        Duration += InDelta;
        double lerp = Math.Clamp((Duration - WaitDuration) / MoveDuration, 0, 1);
        
        float xLerp = (float)(Math.Pow(lerp, 0.5));
        GiftPos.X = Mathf.Lerp(StartOffset.X, EndOffset.X, xLerp);
        
        float yLerp = (float)(Math.Pow(lerp, 3));
        GiftPos.Y = Mathf.Lerp(StartOffset.Y, EndOffset.Y, yLerp);
        
        // Lerp hands
        c.Hands.LerpTo(GiftPos + HandsOffset, GiftPos + HandsOffset, 20, InDelta);
        c.Eyes.CustomDir = GiftPos.Normalized();
        
        // Move window
        var screenPos = Character.WorldToScreen(GiftPos + c.Movement.GetPos());
        gift.SetPosition(new((int)screenPos.X, (int)screenPos.Y));
        
        // Color
        float alpha = Math.Clamp((float) (Duration - WaitDuration) / FadeInDuration, 0, 1);
        if (gift.GetChild(0) is Sprite2D sprite)
            sprite.SetModulate(new(1, 1, 1, alpha));

        if (Duration > MoveDuration + WaitDuration)
        {
            c.Talking.Say(Texts[c.Rnd.RandiRange(0, Texts.Length - 1)]);
            return Get<BehaviorIdle>();
        }
        return null;
    }
}
