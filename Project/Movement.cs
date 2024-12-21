using Godot;
using System;

public partial class Movement : Node
{
	[Export] public Sprite2D Shadow;
	[Export] private float Gravity = 100;
	
	private Vector2 Pos;
	private float VerticalVel;
	
	public override void _Ready()
	{
		base._Ready();
		Pos = Character.ScreenToWorld(GetWindow().Position);
	}

	public override void _Process(double delta)
	{
		VerticalVel -= Gravity * (float)delta;
		float orgH = GetHeight();
		float newH = Math.Max(orgH + VerticalVel, 0);
		SetHeight(newH);
		SetPos(GetPos()); 
	}

	public void SetPos(Vector2 InPos)
	{
		Pos = InPos;
		Vector2 p = Character.WorldToScreen(InPos + Vector2.Up * GetHeight());
		GetWindow().Position = new Vector2I((int)p.X, (int)(p.Y));
	}

	public Vector2 GetPos()
	{
		return Pos;
	}

	public void SetHeight(float InHeight)
	{
		if (InHeight > Shadow.Position.Y)
			VerticalVel = 0;
		Shadow.Position = Vector2.Down * InHeight;
	}

	public float GetHeight() => Shadow.Position.Y;
}
