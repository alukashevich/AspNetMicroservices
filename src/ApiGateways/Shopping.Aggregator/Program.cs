using Shopping.Aggregator.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<ICatalogService, CatalogService>(c =>
                c.BaseAddress = new Uri(builder.Configuration["ApiSettings:CatalogUrl"]))
                //.AddHttpMessageHandler<LoggingDelegatingHandler>()
                //.AddPolicyHandler(GetRetryPolicy())
                //.AddPolicyHandler(GetCircuitBreakerPolicy())
                ;

builder.Services.AddHttpClient<IBasketService, BasketService>(c =>
    c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BasketUrl"]))
    //.AddHttpMessageHandler<LoggingDelegatingHandler>()
    //.AddPolicyHandler(GetRetryPolicy())
    //.AddPolicyHandler(GetCircuitBreakerPolicy())
    ;

builder.Services.AddHttpClient<IOrderService, OrderService>(c =>
    c.BaseAddress = new Uri(builder.Configuration["ApiSettings:OrderingUrl"]))
    //.AddHttpMessageHandler<LoggingDelegatingHandler>()
    //.AddPolicyHandler(GetRetryPolicy())
    //.AddPolicyHandler(GetCircuitBreakerPolicy())
    ;

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();