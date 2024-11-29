using EmailService.API.Middlewares;
using EmailService.Application.Services;
using Confluent.Kafka;
using EmailService.Domain.Abstractions;
using EmailService.Infrastructure.Kafka;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProducer<Null, String>>(provider =>
{
    var config = new ProducerConfig()
    {
        BootstrapServers = builder.Configuration["Kafka:BootstrapServers"],
        /*Acks = Acks.Leader,
        EnableIdempotence = true,*/
    };

    return new ProducerBuilder<Null, string>(config).Build();
});
builder.Services.AddScoped<IKafkaProducer, KafkaProducer>();
builder.Services.AddScoped<IKafkaProducerService, KafkaProducerService>();

builder.Services.AddHostedService<KafkaConsumerService>();

builder.Services.AddScoped<IEmailService, EmailService.Infrastructure.Kafka.EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

/*app.UseAuthorization();*/

app.MapControllers();

app.Run();
