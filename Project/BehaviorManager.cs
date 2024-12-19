using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class BehaviorManager : Node
{
	[Export]
	public Behavior[] Behaviors;
	[Export]
	public Behavior? Current;

	private RandomNumberGenerator Rnd = new();
	
	public override void _Process(double delta)
	{
		// Maybe select random
		Current ??= Behaviors[Rnd.RandiRange(0, Behaviors.Length - 1)];
		
		// First check
		foreach (var b in Behaviors)
		{
			var n = b.Check();
			if (n != null)
				Current = n;
		}
		
		// Then update current
		if (Current != null)
		{
			var n = Current?.Update();
			if (n != null)
				Current = n; 
		}
	}

	public void Set(Behavior In)
	{
		if (Current != null)
			Current.Exit();
		Current = In;
		Current.Enter();
	}
	
	
}
