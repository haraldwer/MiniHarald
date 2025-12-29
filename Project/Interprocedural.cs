using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

public class Interprocedural
{
	public struct Message
	{
		public string str;
		public string data;
	}
	public static List<Message> messages = new();

	static NamedPipeServerStream server = null;
	static NamedPipeClientStream client = null;
	static StreamReader reader = null;
	static StreamWriter writer = null;
	static Task readTask;

	public static void Init()
	{
		bool isHost = CharacterSelector.GetCharacter() == "Harald";
		PipeStream stream = null;
		if (isHost)
		{
			server = new NamedPipeServerStream("stream", PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
			stream = server;
		}
		else
		{
			client = new NamedPipeClientStream(".", "stream", PipeDirection.InOut, PipeOptions.Asynchronous);
			stream = client;
		}

		_ = server?.WaitForConnectionAsync();
		_ = client?.ConnectAsync();
		
		reader = new StreamReader(stream, Encoding.UTF8);
		writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

		// Start tasks for reading and writing?
		readTask = new Task(async () =>
		{
			while (stream.IsConnected)
			{
				var message = await reader.ReadLineAsync();
				if (message == null)
					break;

				var split = message.Split('|');
				string target = split[0];
				if (target == CharacterSelector.GetCharacter())
				{
					// React to message!
					string str = split[1];
					string data = split[2];
					messages.Add(new Message()
					{
						str = str,
						data = data
					});
				}
				else if (isHost)
				{
					// Pass to clients
					writer.WriteLine(message);
				}
			}
		});
		readTask.Start();
	}

	public static void Send(string InReciever, string InMessage, string InData)
	{
		writer.WriteLine(InReciever + "|" + InMessage + "|" + InData);
	}
}
