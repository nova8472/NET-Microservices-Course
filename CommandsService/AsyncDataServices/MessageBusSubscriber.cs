using CommandsService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CommandsService.AsyncDataServices
{
	public class MessageBusSubscriber : BackgroundService
	{
		private readonly IConfiguration _configuration;
		private readonly IEventProcessor _eventProcessor;
		private IConnection? _connection;
		private IModel? _channel;
		private string? _queueName;

		public MessageBusSubscriber(
			IConfiguration configuration,
			IEventProcessor eventProcessor)
		{
			_configuration = configuration;
			_eventProcessor = eventProcessor;

			InitializeRabbitMq();
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			stoppingToken.ThrowIfCancellationRequested();

			var consumer = new EventingBasicConsumer(_channel);

			consumer.Received += (ModuleHandle, ea) =>
			{
				Console.WriteLine("--> Event received.");
				var body = ea.Body;
				var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

				_eventProcessor.ProcessEvent(notificationMessage);
			};

			_channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

			return Task.CompletedTask;
		}

		private void InitializeRabbitMq()
		{
			var portString = _configuration["RabbitMqPort"] ?? string.Empty;
			var port = int.Parse(portString);
			var host = _configuration["RabbitMqHost"];
			var factory = new ConnectionFactory()
			{
				HostName = host,
				Port = port
			};

			_connection = factory.CreateConnection();
			_channel = _connection.CreateModel();
			_channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
			_queueName = _channel.QueueDeclare().QueueName;
			_channel.QueueBind(queue: _queueName, exchange: "trigger", routingKey: "");

			Console.WriteLine("--> Listening on the message bus.");

			_connection.ConnectionShutdown += RabbitMq_ConnectionShutdown;
		}

		private void RabbitMq_ConnectionShutdown(object? sender, EventArgs e)
		{
			Console.WriteLine("--> Connection shut down.");
		}

		public override void Dispose()
		{
			if (_channel != null && _channel.IsOpen)
			{
				_channel.Close();
				_connection?.Close();
			}


			base.Dispose();
		}
	}
}
