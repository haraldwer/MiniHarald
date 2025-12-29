using Godot;
using System;

public partial class Talking : Node
{
	[Export] private Character Character;
	[Export] private double FadeDuration = 0.2;
	[Export] private double BaseDuration = 2.0;
	[Export] private double LetterDuration = 0.05;
	[Export] private float MouthMovementMin = 0.4f;
	[Export] private float MouthMovementMax = 1.0f;
	
	private double Countdown; 
	private double MouthCountdown;
	private int CurrMouth;
	
	public void Say(string InText)
	{
		Character.Label.Text = InText;
		Countdown = BaseDuration + InText.Length * LetterDuration;
		MouthCountdown = 0;
		Character.TextWindow.SetVisible(true);
		GD.Print("Saying: " + InText);
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Character.TextWindow.SetVisible(false);
		Character.Label.Text = "";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Countdown <= 0)
			return;

		Countdown -= delta;

		// Animate mouth
		MouthCountdown -= delta;
		if (MouthCountdown < 0)
		{
			int mouth = CurrMouth;
			while (mouth == CurrMouth)
				mouth = Character.Get().Rnd.RandiRange(0, 1);
			CurrMouth = mouth;
			Character.Mouth?.Set((MouthAnimator.MouthType)mouth);
			MouthCountdown = Character.Get().Rnd.RandfRange(MouthMovementMin, MouthMovementMax);
		}
		
		// Fade text
		var alpha = Math.Clamp(Countdown / FadeDuration, 0, 1);
		Character.Label.SetModulate(new Color(1, 1, 1, (float)(alpha * alpha)));
		
		if (Countdown < 0)
		{
			Character.Label.Text = "";
			Character.TextWindow.SetVisible(false);
			Character.Mouth?.Set(MouthAnimator.MouthType.SMILE);
		}
	}
}
