using Godot;
using Godot.Collections;
using System;

public partial class BehaviorHug : Behavior
{
	[Export] private double HugDuration = 3;
	[Export] private float Height = 120;
	[Export] private float HandDistance = 150;
	[Export] private float RaiseOffset = 90.0f;
	[Export] private float Offset = 200.0f;
	[Export] private string[] Words;

	string Target = "";
	Vector2 Direction;
	double Duration;

	public override Behavior Check(double InDelta)
	{
		return null;
	}

	public override void Enter()
	{
		if (Character.Get().Human && Target == "")
			RequestHug();
	}

	public override void Exit()
	{
		Target = "";
		Duration = 0;
	}

	public void RequestHug()
	{
		string[] targets =
		{
			"Harald",
			"Caela",
			"CatCream",
			"CatBlack",
		};
		string target = "";
		while (target == "" || target == CharacterSelector.GetCharacter())
			target = targets[Character.Get().Rnd.RandiRange(0, 3)];

		var p = Character.Get().Movement.GetPos();
		Dictionary dict = new() { 
			{ "pX",  p.X.ToString() },
			{ "pY",  p.Y.ToString() } 
		};
		GD.Print("Requestin hug from: " + target);
		Interprocedural.Get().Send(target, "RequestHug", dict);
	}

	public void HugRequested(Interprocedural.Message InMsg)
	{
		GD.Print("Hug requested from: " + InMsg.sender);

		var otherPos = new Vector2(
			float.Parse(InMsg.data["pX"].AsString()), 
			float.Parse(InMsg.data["pY"].AsString())
		);
		GD.Print("Other: " + otherPos);
		var myPos = Character.Get().Movement.GetPos();
		GD.Print("My: " + myPos);
		var middle = (otherPos + myPos) * new Vector2(0.5f, 0.5f);
		GD.Print("Middle: " + middle);

		Dictionary dict = new() {
			{ "mpX", middle.X.ToString() },
			{ "mpY", middle.Y.ToString() },
		};
		Interprocedural.Get().Send(new Interprocedural.Message()
		{
			receiver = InMsg.sender,
			message = "AcceptHug",
			data = dict,
		});
		AcceptHug(new Interprocedural.Message()
		{
			sender = InMsg.sender,
			receiver = InMsg.receiver,
			message = "AcceptHug",
			data = dict,
		});

		// TODO: Also consider network delays
	}

	public void AcceptHug(Interprocedural.Message InMsg)
	{
		var c = Character.Get();

		GD.Print("Accepting hug from: " + InMsg.sender);
		Target = InMsg.sender;

		var meetingpoint = new Vector2(
			float.Parse(InMsg.data["mpX"].AsString()),
			float.Parse(InMsg.data["mpY"].AsString())
		);
		GD.Print("Target: " + meetingpoint);
		
		Direction = (c.Movement.GetPos() - meetingpoint).Normalized();
		c.Eyes.CustomDir = Direction;

		// Begin walk
		var walk = c.Behavior.Get<BehaviorWalk>();
		walk.NextBehavior = this;
		walk.FollowMouse = false;
		walk.Target = meetingpoint + Direction * Offset;
		c.Behavior.Set(walk);
	}

	public override Behavior Update(double InDelta)
	{
		try
		{
			if (Target == "")
			{
				GD.Print("No target, cancelling hug");
				return Get<BehaviorIdle>();
			}

			var c = Character.Get();

			// Move in for a hug
			// Stay there for a while
			// Exit hug

			if (Duration <= 0)
			{
				// Start the hug
				GD.Print("Hugging: " + Target);
				if (c.Human)
					c.Talking.Say(Words[c.Rnd.RandiRange(0, Words.Length - 1)]);
				Duration = HugDuration;
				c.Eyes.CustomDir = -Direction;
				c.Walk.Correct();
			}

			Duration -= InDelta;

			if (!c.Human)
			{
				// Lift
				float h = c.Movement.GetHeight();
				h = Mathf.Lerp(h, Height, 10 * (float)InDelta);
				c.Movement.SetHeight(h);

			}
			else
			{
				// Point with both hands
				// In direction
				var dir = Vector2.Up * RaiseOffset - Direction * HandDistance;
				c.Hands?.LerpTo(dir, dir, 10, InDelta);
			}

			if (Duration <= 0)
			{
				// End the hug
				GD.Print("Hug ended: " + Target);
				Duration = 0;
				c.Eyes.CustomDir = new();
				Target = "";
				return Get<BehaviorIdle>();
			}

			return null;
		}
		catch (Exception ex) { GD.Print("Hug error: " + ex); }
		return null;
	}
}
