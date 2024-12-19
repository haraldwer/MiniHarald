using Godot;
using System;

public partial class Movement : Node
{
	private Vector2 Pos;
	private float Height = 0;
	
	public override void _Ready()
	{
		base._Ready();
		Pos = GetWindow().Position;
		Pos /= Character.ViewportScale();
	}
	
	public void SetHeight(float InHeight)
	{
		Height = InHeight;
		SetPos(GetPos());
	}
	
	public void SetPos(Vector2 InPos)
	{
		Pos = InPos;
		Vector2 p = Pos * (float)Character.ViewportScale();
		GetWindow().Position = new Vector2I((int)p.X, (int)(p.Y + Height));
	}

	public Vector2 GetPos()
	{
		return Pos;
	}
}
