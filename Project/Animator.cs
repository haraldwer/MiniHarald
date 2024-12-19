using Godot;
using System;

public partial class Animator : Node
{
	[Export]
	public Animation? Current = null;
	
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		if (Current != null)
			Current.Update();
	}

	void Set(Animation In)
	{
		if (Current != null)
			Current.Exit();
		Current = In;
		Current.Enter();
	}	
}
