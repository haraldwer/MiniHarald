using Godot;
using Godot.Collections;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class Interprocedural
{
	private readonly string Pipename = "stream";
	private bool Host = false;
	private CancellationTokenSource CTS;

	// Host-side
	private readonly ConcurrentBag<NamedPipeServerStream> Clients = new();

	// Client-side
	private NamedPipeClientStream Client;
	private StreamWriter Writer;

	// Messages
	public class Message
	{
		public string receiver = "";
		public string sender = "";
		public string message = "";
		public Dictionary data = new();
	}
	public ConcurrentQueue<Message> ReceivedMessages { get; } = new();

	private static Interprocedural Instance;
	
	public static Interprocedural Get()
	{
		if (Instance == null)
			Instance = new();
		return Instance;
	}

	public void Init()
	{
		Host = CharacterSelector.GetCharacter() == "Harald";
		CTS = new CancellationTokenSource();
		if (Host) Task.Run(() => AcceptClientsAsync(CTS.Token));
		else InitClient();
	}

	public void Deinit()
	{
		CTS?.Cancel();
		Client?.Dispose();
	}

	/* =========================
	   Host (Server) logic
	   ========================= */

	private async Task AcceptClientsAsync(CancellationToken token)
	{
		while (!token.IsCancellationRequested)
		{
			var server = new NamedPipeServerStream(
				Pipename,
				PipeDirection.InOut,
				NamedPipeServerStream.MaxAllowedServerInstances,
				PipeTransmissionMode.Message,
				PipeOptions.Asynchronous);

			await server.WaitForConnectionAsync(token);
			GD.Print("Pipe server connected");

			Clients.Add(server);
			_ = Task.Run(() => HandleConnectionAsync(server, token), token);
		}
	}

	/* =========================
	   Client logic
	   ========================= */

	private void InitClient()
	{
		Client = new NamedPipeClientStream(
			".",
			Pipename,
			PipeDirection.InOut,
			PipeOptions.Asynchronous);

		Client.Connect();

		Writer = new StreamWriter(Client, Encoding.UTF8)
		{
			AutoFlush = true
		};

		GD.Print("Pipe client initialized");

		Task.Run(() => HandleConnectionAsync(Client, CTS.Token));
	}

	/* =========================
	   Shared connection handling
	   ========================= */

	private async Task HandleConnectionAsync(PipeStream pipe, CancellationToken token)
	{
		GD.Print("Starting pipe listener");
		using var reader = new StreamReader(pipe, Encoding.UTF8);

		while (!token.IsCancellationRequested && pipe.IsConnected)
		{
			var message = await reader.ReadLineAsync();
			if (message == null)
				break;

			GD.Print("Incoming message: " + message);
			var split = message.Split('|');
			if (split.Length < 3)
				continue;

			string sender = split[0];
			string target = split[1];
			if (target.Contains(CharacterSelector.GetCharacter()))
			{
				// React to message!
				string str = split[2];
				string data = split[3];
				var parsed = Json.ParseString(data).AsGodotDictionary();
				GD.Print("Enqueueing message: " + str);
				ReceivedMessages.Enqueue(new Message()
				{
					sender = sender,
					receiver = target,
					message = str,
					data = parsed == null ? new() : parsed
				});
			}
			else if (Host)
			{
				// Pass to clients
				GD.Print("Broadcasting to: " + target + " because I'm " + CharacterSelector.GetCharacter());
				BroadcastFromHost(message);
			}
			else
			{
				GD.Print("Ignoring message to: " + target + " because I'm " + CharacterSelector.GetCharacter());
			}
			// else ignore

		}
	}

	/* =========================
	   Sending messages
	   ========================= */

	public void Send(string InReciever, string InMessage, Dictionary InData)
	{
		Send(new Message()
		{
			sender = CharacterSelector.GetCharacter(),
			receiver = InReciever,
			message = InMessage,
			data = InData
		});
	}

	public void Send(Message InMsg)
	{
		if (InMsg.sender == "")
			InMsg.sender = CharacterSelector.GetCharacter();
		GD.Print("Send: " + InMsg.sender + "|" + InMsg.receiver + "|" + InMsg.message + "|" + Json.Stringify(InMsg.data));
		Send(InMsg.sender + "|" + InMsg.receiver + "|" + InMsg.message + "|" + Json.Stringify(InMsg.data));
	}

	private void Send(string message)
	{
		if (Host) BroadcastFromHost(message);
		else Writer.WriteLine(message);
	}

	private void BroadcastFromHost(string message)
	{
		foreach (var client in Clients)
		{
			if (!client.IsConnected)
				continue;

			try
			{
				var writer = new StreamWriter(client, Encoding.UTF8, leaveOpen: true) { AutoFlush = true };
				writer.WriteLine(message);
			}
			catch
			{
				// Ignore broken clients
			}
		}
	}
}
