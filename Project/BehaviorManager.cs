using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class BehaviorManager : Node
{
	[Export] public Movement Movement;
	[Export] public WalkAnimator Walk;
	[Export] public EyeAnimator Eyes;
	[Export] public MouthAnimator Mouth;
	[Export] private int SafeRandomIndices = 4;
	
	[Export] private Behavior[] Behaviors;
	[Export] private Behavior? Current;
	public Behavior? GetCurrent() => Current;
	
	private RandomNumberGenerator Rnd = new();

	public override void _Ready()
	{
		foreach (var b in Behaviors)
			b.Init(this);
	}

	public override void _Process(double delta)
	{	
		if (Current == null)
			Set(Behaviors.FirstOrDefault());
		
		// First check
		foreach (var b in Behaviors)
		{
			var n = b.Check(delta);
			if (n != null)
				Set(n);
		}
		
		// Then update current
		if (Current != null)
		{
			var n = Current?.Update(delta);
			if (n != null)
				Set(n);
		}
	}

	public void Set(Behavior In)
	{
		if (Current != null)
			Current.Exit();
		Current = In;
		Current.Enter();
	}

	public Behavior? Get<T>()
	{
		foreach (var b in  Behaviors)
			if (b.GetType() == typeof(T))
				return b;
		return null;
	}

	public Behavior GetRandom()
	{
		return Behaviors[Rnd.RandiRange(0, SafeRandomIndices - 1)];
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
						Set(Get<BehaviorWave>());
						break;
					case MouseButton.Left:
						Set(Get<BehaviorFly>());
						break;
				}
			}
			else
			{
				if (Current.GetType() == typeof(BehaviorFly))
					Set(Get<BehaviorLand>());
			}
		}
	}
}
