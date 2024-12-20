using Godot;
using System;

public partial class Behavior : Node
{
	[Export] public bool IsNormal = true;

	public virtual Behavior? Update(double InDelta)
	{
		return Get<BehaviorIdle>();
	}

	public virtual Behavior? Check(double InDelta)
	{
		return null;
	}

	public virtual void Exit()
	{
	}

	public virtual void Enter()
	{
	}

	public T? Get<T>()
	{
		return Character.Get().Behavior.Get<T>();
	}
}
