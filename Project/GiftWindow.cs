using Godot;
using System.Diagnostics;
using System.IO;

public partial class GiftWindow : Window
{
	public string param = "";

    public override void _EnterTree()
    {
        base._EnterTree();
		return;
        foreach (var arg in System.Environment.GetCommandLineArgs())
        {
            if (arg.StartsWith("-pos"))
			{
				var split = arg.Substring(4).Split('x');
				if (split.Length < 2)
					return;
				try
				{
					var posX = split[0].ToInt();
					var posY = split[1].ToInt();
					GetWindow().Position = new Vector2I(posX, posY);
				}
				catch { }
			}
        }
    }

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton {Pressed: true} mouseEvent)
		{
			if (mouseEvent.ButtonIndex == MouseButton.Left)
			{
				GD.Print("Dir: " + Directory.GetCurrentDirectory());
				string path = Directory.GetCurrentDirectory();
				string exePath1 = Path.Combine(path, "MiniHarald.exe");
				string exePath2 = Path.Combine(path, "../Publish/windows/MiniHarald.console.exe");
				string exePath3 = Path.Combine(path, "../Publish/windows/MiniHarald.exe");
				string exePath = "";
				if (File.Exists(exePath1))
					exePath = exePath1;
				else if (File.Exists(exePath2))
					exePath = exePath2;
				else if (File.Exists(exePath3))
					exePath = exePath3;
				else
				{
					GD.Print("Couldnt find exe");
					return;
				}

				Process process = new Process();
				process.StartInfo.FileName = exePath;
				if (param != "")
                    process.StartInfo.ArgumentList.Add("-" + param);
				process.StartInfo.ArgumentList.Add("-pos" + Position.X + "x" + Position.Y);
				process.Start();
				Visible = false;
			}

			if (mouseEvent.ButtonIndex == MouseButton.Right)
				Visible = false;
		}
	}
}
