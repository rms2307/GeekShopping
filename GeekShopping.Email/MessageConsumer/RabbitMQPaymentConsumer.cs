using GeekShopping.Email.Messages;
using GeekShopping.Email.Repository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.Email.MessageConsumer
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private readonly EmailRepository _repository;
        private IConnection _connection;
        private IModel _channel;

        private string _queueName = string.Empty;

        private const string HOST_NAME_RABBIT = "localhost";
        private const string USER_NAME_RABBIT = "guest";
        private const string PASSWORD = "guest";
        private const string EXCHANGE_NAME = "FanoutPaymentUpdateExchange";

        public RabbitMQPaymentConsumer(EmailRepository repository)
        {
            _repository = repository;
            Configure();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (channel, evt) =>
            {
                var content = Encoding.UTF8.GetString(evt.Body.ToArray());
                UpdatePaymentResultMessage message = JsonSerializer.Deserialize<UpdatePaymentResultMessage>(content);

                ProcessLogs(message).GetAwaiter().GetResult();

                _channel.BasicAck(evt.DeliveryTag, false);
            };
            _channel.BasicConsume(_queueName, false, consumer);

            return Task.CompletedTask;
        }

        private async Task ProcessLogs(UpdatePaymentResultMessage message)
        {
            try
            {
                await _repository.LogEmail(message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void Configure()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = HOST_NAME_RABBIT,
                    UserName = USER_NAME_RABBIT,
                    Password = PASSWORD,
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Fanout);
                _queueName = _channel.QueueDeclare().QueueName;
                _channel.QueueBind(_queueName, EXCHANGE_NAME, "");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
