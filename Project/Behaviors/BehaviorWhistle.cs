using System;
using Godot;
using NewGameProject;

public partial class BehaviorWhistle : Behavior
{
    [Export] private double MaxDuration = 8;
    [Export] private double FadeDuration = 0.5;
    [Export] private double NoteSpeed = 2;
    [Export] private float NoteStrength = 60f;
    [Export] private Vector2 NoteOffset = new(0, -50);
    [Export] private double MouthSpeed = 5;
    [Export] private float MouthStrength = 20f;
    [Export] private Sprite2D[] Notes;
    [Export] private Node2D Mouth;
    [Export] private string[] Texts;
    
    public override void Enter()
    {
        if (!ProcessUtility.IsPlayingMusic())
            return;
        
        foreach (var n in Notes)
        {
            n.Visible = true;
            var c = Color.FromHsv(Character.Get().Rnd.Randf(), 1.0f, 0.5f);
            n.SetModulate(new (c.R, c.G, c.B, 0));
        }
    }

    public override void Exit()
    {
        Character.Get().Mouth.Set(MouthAnimator.MouthType.SMILE);
        Duration = 0;
        foreach (var n in Notes)
            n.Visible = false;
        Mouth.Position = Vector2.Zero;
    }

    private double Duration;
    
    public override Behavior Update(double InDelta)
    {
        if (!ProcessUtility.IsPlayingMusic())
            return Get<BehaviorIdle>();

        var c = Character.Get();
        
        Duration += InDelta;
        if (Duration > MaxDuration)
        {
            c.Talking.Say(Texts[c.Rnd.RandiRange(0, Texts.Length - 1)]);
            return Get<BehaviorIdle>();
        }
        
        c.Hands.Idle(InDelta);
        c.Mouth.Set(MouthAnimator.MouthType.WHISTLE);

        double fadeIn = Duration / FadeDuration;
        double fadeOut = (MaxDuration - Duration) / FadeDuration;
        double fade = Math.Min(fadeIn, fadeOut);

        for (int i = 0; i < Notes.Length; i++)
        {
            double t = Duration * NoteSpeed + i * 4.5;
            float x = (float)Math.Sin(t);
            float y = (float)Math.Sin(t * 2);
            Notes[i].Position = new Vector2(x, y) * NoteStrength + NoteOffset * (float)fade;
            var color = Notes[i].Modulate;
            color.A = (float) fade;
            Notes[i].SetModulate(color);
        }
        
        double mt = Duration * MouthSpeed;
        float mx = (float)Math.Sin(mt);
        float my = (float)Math.Sin(mt * 2);
        Mouth.Position = new Vector2(mx, my) * MouthStrength; 
        
        return null;
    }
}
