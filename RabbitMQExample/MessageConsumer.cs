using System;
using System.Threading.Tasks;
using MassTransit;
using models;

namespace RabbitMQRevieverExample
{
    public class MessageConsumer : IConsumer<Test>
    {
        public Task Consume(ConsumeContext<Test> context)
        {
            Console.WriteLine($"Message processed: Message:{context.Message.message}");
            return Task.CompletedTask;
        }
    }
}
    