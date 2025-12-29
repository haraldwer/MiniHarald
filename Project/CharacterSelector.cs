using Godot;
using System;
using System.Linq;

public partial class CharacterSelector : Node2D
{
	[Export] Character Harald;
	[Export] Character Caela;
	[Export] Character CatCream;
	[Export] Character CatBlack;

	public static String GetCharacter()
	{
		var args = System.Environment.GetCommandLineArgs();
		if (args.Contains("-Caela")) return "Caela";
		if (args.Contains("-CatCream")) return "CatCream";
		if (args.Contains("-CatBlack")) return "CatBlack";
		return "Harald";
	}

	public override void _EnterTree()
	{
		// All characters are created by default 
		// Destroy all except one
		switch (GetCharacter())
		{
			case "Caela":
				Harald.QueueFree();
				CatBlack.QueueFree();
				CatCream.QueueFree();
				break;
			case "CatCream":
				Caela.QueueFree();
				Harald.QueueFree();
				CatBlack.QueueFree();
				break;
			case "CatBlack":
				Caela.QueueFree();
				Harald.QueueFree();
				CatCream.QueueFree();
				break;
			default:
				Caela.QueueFree();
				CatBlack.QueueFree();
				CatCream.QueueFree();
				break;
		}
	}
}
