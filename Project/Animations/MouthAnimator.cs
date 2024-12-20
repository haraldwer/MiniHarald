using Godot;
using System;

public partial class MouthAnimator : Node
{
	[Export] private Node2D[] Mouths;

	public enum MouthType
	{
		SMILE = 0,
		OPEN,
		TEETH,
		WHISTLE,
		COUNT
	}

	public void Set(MouthType InType)
	{
		for (int i = 0; i < Mouths.Length; i++)
			Mouths[i].Visible = i == (int) InType;
	}
}
