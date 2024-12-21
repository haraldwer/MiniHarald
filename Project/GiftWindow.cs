using Godot;
using System.Diagnostics;
using System.IO;

public partial class GiftWindow : Window
{
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton {Pressed: true} mouseEvent)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                GD.Print("Dir: " + Directory.GetCurrentDirectory());
                string path = Directory.GetCurrentDirectory();
                string exePath1 = Path.Combine(path, "MiniHarald.exe");
                string exePath2 = Path.Combine(path, "../Publish/windows/MiniHarald.exe");
                string exePath = "";
                if (File.Exists(exePath1))
                    exePath = exePath1;
                if (File.Exists(exePath2))
                    exePath = exePath2;
                if (!File.Exists(exePath))
                    return;
                
                Process process = new Process();
                process.StartInfo.FileName = exePath;
                process.Start();
                Visible = false;
            }

            if (mouseEvent.ButtonIndex == MouseButton.Right)
                Visible = false;
        }
    }
}
