using MediatR;
using BankingService.Business.CommandHandlers;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using BankingService.Business.QueryHandlers;
using BankingService.Infrastructure.DataPersistance.Contracts;
using BankingService.Infrastructure.DataPersistance.Repositories;
using Infrastucture.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(
             Assembly.GetExecutingAssembly(),
             typeof(AccountCommandHandler).Assembly, typeof(TransactionCommandHandler).Assembly);

builder.Services.AddMediatR(
             Assembly.GetExecutingAssembly(),
             typeof(AccountQueryHandler).Assembly);

builder.Services.AddTransient<IAccountRepository, AccountRepository>();
builder.Services.AddTransient<ITransactionRepository, TransactionRepository>();
builder.Services.AddTransient<IMockDbEntities, MockDbEntities>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


//builder.Services.AddTransient<IAccountRepository, AccountRepository>();
//builder.Services.AddTransient<ITransactionRepository, TransactionRepository>();

//builder.Services.AddMediatR(cfg =>
//{
//    cfg.RegisterServicesFromAssemblyContaining(typeof(Program));
//});

var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
