using Godot;
using System;

public partial class Behavior : Node
{
	protected BehaviorManager Manager;

	public void Init(BehaviorManager InManager)
	{
		Manager = InManager;
	}
	
	public virtual Behavior? Update(double InDelta)
	{
		return Manager.Get<BehaviorIdle>();
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
}
