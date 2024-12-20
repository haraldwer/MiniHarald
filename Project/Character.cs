using System;
using Godot;

public partial class Character : Node2D
{
    [Export] public Label Label;
    [Export] public TextWindow TextWindow;
    [Export] public Movement Movement;
    [Export] public Talking Talking;
    [Export] public BehaviorManager Behavior;
    [Export] public EyeAnimator Eyes;
    [Export] public MouthAnimator Mouth;
    [Export] public WalkAnimator Walk;
    [Export] public HandAnimator Hands;
    [Export] public Sprite2D Shirt;
    
    private static Character? Instance;
    private RandomNumberGenerator Rnd = new();
    
    Character()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        base._Ready();

        var c = Shirt.SelfModulate;
        Shirt.SetSelfModulate(Color.FromHsv(Rnd.Randf(), c.S, c.V));
    }

    public static Character Get() => Instance;
    
    public static float ViewportScale()
    {
        if (Instance != null)
            return Instance.GlobalScale.Length();
        return 1.0f;
    }

    public static Vector2 ScreenToWorld(Vector2 InScreen)
    {
        return InScreen / ViewportScale();
    }
    
    public static Vector2 WorldToChar(Vector2 InPos)
    {
        return InPos - Instance.Position / ViewportScale();
    }
    
    public static Vector2 CharToWorld(Vector2 InPos)
    {
        return InPos * ViewportScale() + Instance.Position;
    }
    
    public static Vector2 WorldToScreen(Vector2 InPos)
    {
        return InPos * ViewportScale();
    }
    
    public static Vector2 MousePos()
    {
        if (Instance != null)
            return WindowPos() + Instance.GetWindow().GetMousePosition();
        return Vector2.Zero;
    }

    public static Vector2 WindowPos()
    {
        if (Instance != null)
            return Instance.GetWindow().GetPosition();
        return Vector2.Zero;
    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.Pressed)
            {
                switch (mouseEvent.ButtonIndex)
                {
                    case MouseButton.Right:
                        Behavior.Set(Behavior.Get<BehaviorWave>());
                        break;
                    case MouseButton.Left:
                        Behavior.Set(Behavior.Get<BehaviorFly>());
                        break;
                }
            }
            else
            {
                if (Behavior.GetCurrent()?.GetType() == typeof(BehaviorFly))
                    Behavior.Set(Behavior.Get<BehaviorLand>());
            }
        }
    }
}
