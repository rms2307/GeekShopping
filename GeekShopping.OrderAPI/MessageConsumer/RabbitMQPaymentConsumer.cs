using GeekShopping.OrderApi.RabbitMQSender;
using GeekShopping.OrderApi.Repository;
using GeekShopping.OrderAPI.Messages;
using GeekShopping.OrderAPI.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.OrderAPI.MessageConsumer
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private readonly OrderRepository _repository;
        private IConnection _connection;
        private IModel _channel;

        private const string HOST_NAME_RABBIT = "localhost";
        private const string USER_NAME_RABBIT = "guest";
        private const string PASSWORD = "guest";
        private const string QUEUE_NAME_PAYMENT_RESULT = "orderpaymentresultqueue";

        public RabbitMQPaymentConsumer(OrderRepository repository)
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
                UpdatePaymentResultVO vo = JsonSerializer.Deserialize<UpdatePaymentResultVO>(content);

                UpdatePaymentStatus(vo).GetAwaiter().GetResult();

                _channel.BasicAck(evt.DeliveryTag, false);
            };
            _channel.BasicConsume(QUEUE_NAME_PAYMENT_RESULT, false, consumer);

            return Task.CompletedTask;
        }

        private async Task UpdatePaymentStatus(UpdatePaymentResultVO vo)
        {
            try
            {
                await _repository.UpdateOrderPaymentStatus(vo.OrderId, vo.Status);
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
                _channel.QueueDeclare(queue: QUEUE_NAME_PAYMENT_RESULT, false, false, false, arguments: null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
