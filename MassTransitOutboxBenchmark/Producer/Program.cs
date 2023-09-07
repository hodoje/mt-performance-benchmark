using MassTransit;
using Microsoft.Data.SqlClient;
using Producer;
using Producer.Exceptions;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((host, log) =>
{
    log.MinimumLevel.Debug();

    log.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
    log.MinimumLevel.Override("Quartz", LogEventLevel.Information);
    log.WriteTo.Console();

});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDatabase(builder.Configuration);

builder.Services.AddMassTransit(busConfig =>
{
    busConfig.AddEntityFrameworkOutbox<ProducerContext>(outboxConfig =>
    {
        outboxConfig.UseSqlServer();
        outboxConfig.UseBusOutbox();
    });

    busConfig.SetKebabCaseEndpointNameFormatter();

    busConfig.AddConfigureEndpointsCallback((context, _, config) =>
    {
        config.UseMessageRetry(policy =>
        {
            policy.Handle<InfrastructureFailureException>();
            policy.Handle<SqlException>(x => x.Number == 11 || x.Number == 1205);
            policy.Immediate(3);
        });
        config.UseEntityFrameworkOutbox<ProducerContext>(context);
    });

    busConfig.AddDelayedMessageScheduler();
    busConfig.UsingAmazonSqs((context, config) =>
    {
        config.UseDelayedMessageScheduler();

        config.UseMessageRetry(policy =>
        {
            policy.Handle<InfrastructureFailureException>();
            policy.Handle<SqlException>(x => x.Number == 11 || x.Number == 1205);
            policy.Immediate(10);
        });

        config.LocalstackHost();

        config.ConfigureEndpoints(context);
    });

});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Producer API");
});

app.UseHttpsRedirection();

app.UseCors("allowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
