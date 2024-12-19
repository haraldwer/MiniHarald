using Godot;
using System;

public partial class EyeAnimator : Node
{
	[Export] private float EyeOffset = 50f;
	[Export] private float LookOffset = 70f;
	[Export] private float LookSmoothing = 7.0f;
	[Export] private float EyeHeight = 50f;
	[Export] private Node2D Eyes;
	[Export] private Node2D LookRoot;
	[Export] private Movement Movement;

	public Vector2 CustomDir;
	
	public override void _Process(double delta)
	{
		base._Process(delta);

		Vector2 dir = CustomDir;
		if (dir.Length() < 0.0001)
		{
			Vector2 mp = GetWindow().GetMousePosition() * Character.ViewportScale();
			Vector2I size = GetWindow().GetSize();
			Vector2 sp = GetWindow().GetScreenTransform().Origin;
			sp += new Vector2(size.X, size.Y - EyeHeight) * 0.5f * Character.ViewportScale();
			Vector2 diff = (mp - sp) / 10.0f;
			if (diff.Length() > 1.0)
				diff = diff.Normalized();
			dir = diff;
		}
		Eyes.Position = EyeOffset * dir;
		
		Vector2 target = LookOffset * dir * new Vector2(1.0f, 0.5f);;
		Vector2 p = LookRoot.Position; 
		p.X = Mathf.Lerp(LookRoot.Position.X, target.X, LookSmoothing * (float)delta);
		p.Y = Mathf.Lerp(LookRoot.Position.Y, target.Y, LookSmoothing * (float)delta);
		LookRoot.Position = p; 
	}
}
