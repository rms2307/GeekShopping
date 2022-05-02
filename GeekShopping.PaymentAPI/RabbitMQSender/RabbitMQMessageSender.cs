using GeekShopping.MessageBus;
using GeekShopping.PaymentAPI.Messages;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace GeekShopping.PaymentApi.RabbitMQSender
{
    public class RabbitMQMessageSender : IRabbitMQMessageSender
    {
        private readonly string _hostName;
        private readonly string _password;
        private readonly string _userName;
        private IConnection _connection;

        private const string EXCHANGE_NAME = "FanoutPaymentUpdateExchange";

        public RabbitMQMessageSender()
        {
            _hostName = "localhost";
            _password = "guest";
            _userName = "guest";
        }

        public void SendMessage(BaseMessage message)
        {
            try
            {
                CreateConnection();

                PublishMessage(message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void PublishMessage(BaseMessage message)
        {
            var channel = _connection.CreateModel();
            channel.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Fanout, durable: false);
            byte[] body = GetMessageAsByteArray(message);

            channel.BasicPublish(
                    exchange: EXCHANGE_NAME,
                    routingKey: "",
                    basicProperties: null,
                    body: body
                );
        }

        private byte[] GetMessageAsByteArray(BaseMessage message)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize((UpdatePaymentResultMessage)message, options);

            return Encoding.UTF8.GetBytes(json);
        }

        private void CreateConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password,
            };
            _connection = factory.CreateConnection();
        }
    }
}
