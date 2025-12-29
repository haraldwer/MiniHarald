using Godot;
using System.Linq;

public partial class BehaviorManager : Node
{
	[Export] private Behavior[] Behaviors;
	private Behavior? Current;
	public Behavior? GetCurrent() => Current;

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
		GD.Print("Behavior: " + Current.GetType().Name);
		Current.Enter();
	}

	public T? Get<T>()
	{
		foreach (var b in  Behaviors)
			if (b is T t)
				return t;
		return default(T);
	}

	public Behavior GetRandom()
	{
		while (true)
		{
			var rand = Behaviors[Character.Get().Rnd.RandiRange(0, Behaviors.Length - 1)];
			if (rand.IsNormal)
				return rand;
		}
	}
}
