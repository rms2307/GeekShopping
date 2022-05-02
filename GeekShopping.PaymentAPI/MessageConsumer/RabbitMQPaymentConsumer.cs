using GeekShopping.PaymentApi.RabbitMQSender;
using GeekShopping.PaymentAPI.Messages;
using GeekShopping.PaymentProcessor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.PaymentAPI.MessageConsumer
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private IRabbitMQMessageSender _rabbitMQMessageSender;
        private IProcessPayment _processPayment;

        private const string HOST_NAME_RABBIT = "localhost";
        private const string USER_NAME_RABBIT = "guest";
        private const string PASSWORD = "guest";
        private const string QUEUE_NAME_PAYMENT = "orderpaymentprocessqueue";
        private const string QUEUE_NAME_PAYMENT_RESULT = "orderpaymentresultqueue";

        public RabbitMQPaymentConsumer(IProcessPayment processPayment, IRabbitMQMessageSender rabbitMQMessageSender)
        {
            _processPayment = processPayment;
            _rabbitMQMessageSender = rabbitMQMessageSender;
            Configure();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (channel, evt) =>
            {
                var content = Encoding.UTF8.GetString(evt.Body.ToArray());
                PaymentMessage vo = JsonSerializer.Deserialize<PaymentMessage>(content);
                ProcessPayment(vo);

                _channel.BasicAck(evt.DeliveryTag, false);
            };
            _channel.BasicConsume(QUEUE_NAME_PAYMENT, false, consumer);

            return Task.CompletedTask;
        }

        private void ProcessPayment(PaymentMessage vo)
        {
            var result = _processPayment.PaymentProcessor();

            UpdatePaymentResultMessage paymentResult = new()
            {
                Status = result,
                OrderId = vo.OrderId,
                Email = vo.Email,
            };

            try
            {
                _rabbitMQMessageSender.SendMessage(paymentResult);
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
                _channel.QueueDeclare(queue: QUEUE_NAME_PAYMENT, false, false, false, arguments: null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
