using PlatformService.DTOs;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PlatformService.AsyncDataServices
{
	public class MessageBusClient : IMessageBusClient
	{
		private readonly IConfiguration _configuration;
		private readonly IConnection? _connection;
		private readonly IModel? _channel;

		public MessageBusClient(IConfiguration configuration)
		{
			_configuration = configuration;
			var portString = _configuration["RabbitMqPort"] ?? string.Empty;
			var port = int.Parse(portString);
			var host = _configuration["RabbitMqHost"];
			var factory = new ConnectionFactory()
			{
				HostName = host,
				Port = port
			};

			try
			{
				_connection = factory.CreateConnection();
				_channel = _connection.CreateModel();
				_channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
				_connection.ConnectionShutdown += RabbitMq_ConnectionShutdown;

				Console.WriteLine("--> Connected to message bus");
			}
			catch	(Exception ex)
			{
				Console.WriteLine($"--> Could not connect to the message bus: {ex.Message}");
			}
		}

		public void PublishNewPlatform(PlatformPublishedDTO platformPublishedDTO)
		{
			var message = JsonSerializer.Serialize(platformPublishedDTO);

			if(_connection != null && _connection.IsOpen)
			{
				Console.WriteLine($"RabbitMq connection open. Sending message...");
				SendMessage(message);
			}
			else
			{
				Console.WriteLine($"RabbitMq connection is closed. Not sending message...");
			}
		}

		public void Dispose()
		{
			Console.WriteLine("--> Message bus disposed");
			if (_channel != null && _channel.IsOpen)
			{
				_channel.Close();
				_connection?.Close();
			}
		}

		private void RabbitMq_ConnectionShutdown(object? sender, EventArgs e)
		{
			Console.WriteLine("--> RabbitMq connection shut down");
		}

		private void SendMessage(string message)
		{
			var body = Encoding.UTF8.GetBytes(message);
			_channel.BasicPublish(
				exchange: "trigger",
				routingKey: "",
				basicProperties: null,
				body: body);

			Console.WriteLine($"We have sent: {message}");
		}
	}
}
