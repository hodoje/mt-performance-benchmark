using Consumer;
using Consumer.Consumers;
using Consumer.Exceptions;
using MassTransit;
using Microsoft.Data.SqlClient;
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
    busConfig.AddEntityFrameworkOutbox<ConsumerContext>(outboxConfig =>
    {
        outboxConfig.UseSqlServer();
        outboxConfig.UseBusOutbox();
    });

    busConfig.SetKebabCaseEndpointNameFormatter();

    busConfig.AddConsumer<RegularConsumer>();
    busConfig.AddConsumer<BatchConsumer, BatchConsumerDefinition>();

    busConfig.AddConfigureEndpointsCallback((context, _, config) =>
    {
        config.UseMessageRetry(policy =>
        {
            policy.Handle<InfrastructureFailureException>();
            policy.Handle<SqlException>(x => x.Number == 11 || x.Number == 1205);
            policy.Immediate(3);
        });
        config.UseEntityFrameworkOutbox<ConsumerContext>(context);
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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("allowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
