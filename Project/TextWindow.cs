using Godot;
using System;

public partial class TextWindow : Window
{
	[Export] private Vector2I Offset;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var w = GetWindow();
		var c = GetChild(0) as Container;
		var size = c.GetSize() + Vector2.One * 50.0f;
		w.SetSize(new Vector2I((int)size.X + 2, (int)size.Y + 2));
		
		var cw = Character.Get().GetWindow();
		if (cw == null)
			return;

		c.Position = Vector2.Right * 20.0f;
		w.Position = cw.Position + Offset;
	}
}
